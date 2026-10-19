using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>JSON 文件持久化，保存在程序运行目录 data\todos.json（树形结构，含旧数据迁移）</summary>
public static class TodoStorage
{
    static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    static string Dir =>
        AppPaths.DataDir;

    static string FilePath => Path.Combine(Dir, "todos.json");

    /// <summary>是否处于「上次加载失败」状态：为 true 时禁止孤儿清理与覆盖保存，保护用户数据</summary>
    public static bool LastLoadFailed { get; private set; }

    public static List<TodoItem> Load()
    {
        LastLoadFailed = false;
        try
        {
            if (!File.Exists(FilePath)) return new List<TodoItem>();
            var json = File.ReadAllText(FilePath);
            var items = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            foreach (var item in items)
                MigrateLegacy(item);
            return items;
        }
        catch when (File.Exists(FilePath))
        {
            // 文件存在但读不了/解析失败：备份原文件，绝不覆盖，也绝不当成空数据跑清理
            LastLoadFailed = true;
            try
            {
                var backup = FilePath + ".corrupt-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                File.Copy(FilePath, backup, true);
            }
            catch { }
            return new List<TodoItem>();
        }
        catch
        {
            return new List<TodoItem>();
        }
    }

    /// <summary>把 v2.0 的扁平 SubTasks 迁移为 Children 树（递归处理所有层级）</summary>
    static void MigrateLegacy(TodoItem item)
    {
        if (item.SubTasks is { Count: > 0 })
        {
            foreach (var sub in item.SubTasks)
                item.Children.Add(new TodoItem
                {
                    Id = sub.Id,
                    Text = sub.Text,
                    IsDone = sub.IsDone,
                    CreatedAt = item.CreatedAt,
                });
        }
        item.SubTasks = null;
        foreach (var child in item.Children)
            MigrateLegacy(child);
    }

    /// <summary>序列化为 JSON（在 UI 线程拍快照，文件 IO 交给 <see cref="WriteAtomic"/> 后台执行）</summary>
    public static string Serialize(IEnumerable<TodoItem> items) =>
        JsonSerializer.Serialize(items, JsonOpts);

    /// <summary>原子写盘（.tmp + Replace）。写一半崩溃/磁盘满不会留下坏 JSON；建议在后台线程调用</summary>
    public static void WriteAtomic(string json)
    {
        if (LastLoadFailed) return; // 数据没安全读回来之前，绝不覆盖磁盘
        try
        {
            Directory.CreateDirectory(Dir);
            var tmp = FilePath + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(FilePath))
                File.Replace(tmp, FilePath, null);
            else
                File.Move(tmp, FilePath);
        }
        catch
        {
            // 磁盘异常时不让应用崩溃，下次改动会再尝试保存
        }
    }

    public static void Save(IEnumerable<TodoItem> items) => WriteAtomic(Serialize(items));
}
