using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace TodoApp.Services;

/// <summary>
/// Win11 系统背景材质（DWMWA_SYSTEMBACKDROP_TYPE）：主窗口云母 Mica。
/// 设不上（Win10 / 旧 Win11 / 远程桌面）时 Enabled 保持 false，调色板维持实色，无需额外回退逻辑；
/// 生效时 Theme.Apply 会把 Bg 换成半透明色调，让云母从窗口背景透出来。
/// </summary>
public static class Backdrop
{
    const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    const int DWMSBT_MAINWINDOW = 2; // 云母

    static IntPtr _hwnd;
    static bool? _dark;

    /// <summary>系统材质是否已生效（生效时 Bg 使用半透明色调）</summary>
    public static bool Enabled { get; private set; }

    /// <summary>在窗口句柄创建后调用。返回是否成功启用云母</summary>
    public static bool TryEnableMica(IntPtr hwnd)
    {
        try
        {
            // 玻璃框架延伸到整个客户区，背景材质才能铺满窗口
            var margins = new MARGINS { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
            if (DwmExtendFrameIntoClientArea(hwnd, ref margins) != 0) return false;
            var type = DWMSBT_MAINWINDOW;
            if (DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref type, sizeof(int)) != 0) return false;
            // WPF 渲染目标默认不透明清屏，会把材质挡住；置为透明后云母才能从窗口背景透出
            if (HwndSource.FromHwnd(hwnd)?.CompositionTarget is HwndTarget target)
                target.BackgroundColor = Colors.Transparent;
            _hwnd = hwnd;
            Enabled = true;
            if (_dark.HasValue) SetDark(_dark.Value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>云母深浅色随应用主题切换（未启用时仅记录，启用后自动补设）</summary>
    public static void SetDark(bool dark)
    {
        _dark = dark;
        if (!Enabled || _hwnd == IntPtr.Zero) return;
        try
        {
            var v = dark ? 1 : 0;
            _ = DwmSetWindowAttribute(_hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref v, sizeof(int));
        }
        catch { }
    }

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int sizeOfValue);

    [DllImport("dwmapi.dll")]
    static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    struct MARGINS { public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight; }
}
