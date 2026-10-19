using System.IO;

namespace TodoApp.Services;

/// <summary>
/// 数据目录统一入口：默认便携模式，保存在程序运行目录 data\ 下（跟随 exe）。
/// 首次启动时若 %APPDATA%\JeffBox 已有旧数据，会自动整体搬过来（一次性，原目录保留可回退）。
/// 设置环境变量 JEFFBOX_DATA 可重定向数据目录（自动化测试专用沙箱，避免碰真实数据）。
/// </summary>
public static class AppPaths
{
    public static string DataDir
    {
        get
        {
            var sandbox = Environment.GetEnvironmentVariable("JEFFBOX_DATA");
            if (!string.IsNullOrWhiteSpace(sandbox))
                return sandbox;
            // 便携模式：数据跟随 exe 所在目录
            return Path.Combine(AppContext.BaseDirectory, "data");
        }
    }

    static string RoamingDir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeffBox");

    /// <summary>确保数据目录存在；首次运行时把 %APPDATA%\JeffBox 的旧数据整体搬过来。测试沙箱（JEFFBOX_DATA）不迁移，保持纯净</summary>
    public static void EnsureMigrated()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("JEFFBOX_DATA"))) return;
            Directory.CreateDirectory(DataDir);
            if (File.Exists(Path.Combine(DataDir, "todos.json"))) return; // 已有数据，不覆盖
            if (!Directory.Exists(RoamingDir)) return;
            if (Path.GetFullPath(DataDir).Equals(Path.GetFullPath(RoamingDir),
                    StringComparison.OrdinalIgnoreCase)) return;

            foreach (var entry in Directory.GetFileSystemEntries(RoamingDir))
            {
                var target = Path.Combine(DataDir, Path.GetFileName(entry));
                if (Directory.Exists(entry)) CopyDir(entry, target);
                else if (!File.Exists(target)) File.Copy(entry, target);
            }
        }
        catch
        {
            // 迁移失败不致命：以空数据启动即可
        }
    }

    static void CopyDir(string src, string dst)
    {
        Directory.CreateDirectory(dst);
        foreach (var f in Directory.GetFiles(src))
            File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), overwrite: false);
        foreach (var d in Directory.GetDirectories(src))
            CopyDir(d, Path.Combine(dst, Path.GetFileName(d)));
    }
}
