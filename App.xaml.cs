using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace TodoApp;

public partial class App : System.Windows.Application
{
    const string MutexName = @"Local\JeffBox.SingleInstance";
    const string SignalName = @"Local\JeffBox.ShowSignal";

    public static bool StartMinimized;

    static string? _lastCrashSig;
    static int _crashRepeat;

    internal static void LogCrash(string source, Exception? ex)
    {
        try
        {
            var msg = ex == null ? "(null)" : ex.ToString();
            var sig = source + "|" + ex?.GetType().FullName + "|" + ex?.StackTrace?.Split('\n').FirstOrDefault();
            var path = System.IO.Path.Combine(Services.AppPaths.DataDir, "crash.log");
            // 相同异常连发只计数，不整段刷屏（布局/渲染类异常常被布局循环反复触发）
            if (sig != null && sig == _lastCrashSig)
            {
                _crashRepeat++;
                System.IO.File.AppendAllText(path, "x" + _crashRepeat + "\n");
                return;
            }
            _lastCrashSig = sig;
            _crashRepeat = 0;
            if (ex?.InnerException != null) msg += "\n-- Inner --\n" + ex.InnerException;
            System.IO.File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}\n{msg}\n---\n");
        }
        catch { }
    }

    Mutex? _mutex;
    EventWaitHandle? _showSignal;
    System.Threading.CancellationTokenSource? _signalCts;
    bool _ownsMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        Services.AppPaths.EnsureMigrated(); // v1.0 更名 JeffBox：首启迁移旧 TodoApp 数据
        // 崩溃捕获：未处理异常写日志而不是静默闪退，便于定位（%APPDATA%\JeffBox\crash.log）
        System.Windows.Application.Current.DispatcherUnhandledException += (_, ex) =>
        {
            LogCrash("UI", ex.Exception);
            ex.Handled = true; // 记录后吞掉，避免输入过程闪退丢内容
        };
        AppDomain.CurrentDomain.UnhandledException += (_, ex) =>
            LogCrash("Domain", ex.ExceptionObject as Exception);
        try { System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); } catch { }

        // 单实例：重复启动时唤醒已有窗口，避免托盘图标和全局热键重复注册
        _mutex = new Mutex(true, MutexName, out var createdNew);
        if (!createdNew)
        {
            try { EventWaitHandle.OpenExisting(SignalName).Set(); } catch { }
            Shutdown();
            return;
        }
        _ownsMutex = true;

        Services.Theme.EnsureRegistered(); // 主题画刷必须先于窗口创建注册

        _showSignal = new EventWaitHandle(false, EventResetMode.AutoReset, SignalName);
        StartMinimized = e.Args.Contains("--minimized", StringComparer.OrdinalIgnoreCase);

        base.OnStartup(e);

        _signalCts = new System.Threading.CancellationTokenSource();
        var ct = _signalCts.Token;
        new Thread(() =>
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    if (_showSignal!.WaitOne(500))
                        Dispatcher.BeginInvoke(() =>
                        {
                            var win = MainWindow as MainWindow;
                            win?.ShowFromTray();
                        });
                }
            }
            catch (ObjectDisposedException) { }
        })
        { IsBackground = true }.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _signalCts?.Cancel();
        // 不 Dispose 句柄/互斥体：后台线程可能仍在 WaitOne，进程退出时由系统回收更安全
        base.OnExit(e);
    }
}
