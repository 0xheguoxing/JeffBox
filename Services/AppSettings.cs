using System.IO;
using System.Text.Json;

namespace TodoApp.Services;

/// <summary>应用设置，保存在 %APPDATA%\TodoApp\settings.json</summary>
public class AppSettings
{
    /// <summary>点关闭时是最小化到托盘（true）还是直接退出（false）</summary>
    public bool MinimizeToTray { get; set; } = true;

    /// <summary>全局快捷键（显示/隐藏窗口），空串表示停用</summary>
    public string Hotkey { get; set; } = "Ctrl+Alt+T";

    /// <summary>直达待办页的快捷键，空串表示停用</summary>
    public string HotkeyTodo { get; set; } = "";

    /// <summary>界面语言：zh / en</summary>
    public string Language { get; set; } = "zh";

    /// <summary>主题：light / dark</summary>
    public string Theme { get; set; } = Services.Theme.Light;

    /// <summary>窗口位置与尺寸（关闭时记住）</summary>
    public double WinX { get; set; } = double.NaN;
    public double WinY { get; set; } = double.NaN;
    public double WinW { get; set; } = double.NaN;
    public double WinH { get; set; } = double.NaN;
    public bool WinMax { get; set; }

    /// <summary>开机自启动</summary>
    public bool AutoStart { get; set; }

    /// <summary>启动 / 快捷方式呼出时窗口出现的位置：remember(默认) / screen(鼠标屏居中) / near(鼠标附近)</summary>
    public string OpenPosition { get; set; } = "remember";

    /// <summary>详情浮层宽度（用户拖拽后记住；≤0 回退默认）</summary>
    public double DetailW { get; set; } = 382;

    /// <summary>详情浮层高度（0 = 随内容自适应；拖拽后固定）</summary>
    public double DetailH { get; set; } = 0;

    static string Dir =>
        AppPaths.DataDir;

    static string FilePath => Path.Combine(Dir, "settings.json");

    static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch
        {
            // 设置损坏时回退默认值
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, JsonOpts));
        }
        catch
        {
            // 磁盘异常不崩溃
        }
    }
}
