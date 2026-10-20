using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TodoApp.Services;

/// <summary>
/// 轻量国际化。XAML 中用 {Binding [Key], Source={x:Static svc:Loc.Instance}} 绑定词条，
/// 代码中用 Loc.Get / Loc.F；切换语言后触发 Item[] 通知刷新所有绑定。
/// </summary>
public sealed class Loc : INotifyPropertyChanged
{
    public static Loc Instance { get; } = new();

    Loc() { }

    public static string Lang { get; private set; } = "zh";

    public static void SetLang(string lang)
    {
        if (Lang == lang) return;
        Lang = lang;
        Instance.OnLangChanged();
    }

    /// <summary>初始化为设置中的语言</summary>
    public static void Init(string lang) => Lang = string.IsNullOrEmpty(lang) ? "zh" : lang;

    public string this[string key] => Get(key);

    static readonly Dictionary<string, string> Zh = new()
    {
        // 通用
        ["Add"] = "添加",
        ["Cancel"] = "取消",
        ["Save"] = "保存",
        ["Done"] = "完成",
        ["Delete"] = "删除",
        ["Off"] = "停用",
        ["ImageAlt"] = "图片",
        ["ImageMissing"] = "图片已缺失",
        ["DetailToggle"] = "详情",
        ["More"] = "更多",
        ["Collapse"] = "收起",
        ["ExpandAll"] = "全部展开",
        ["CollapseAll"] = "全部收起",
        ["NewDetailsPlaceholder"] = "添加详细内容（可选，支持 Markdown，可直接粘贴图片）",
        ["BreadcrumbFmt"] = "位于：{0}",
        // 主界面
        ["HeaderTitle"] = "今日待办",
        ["InputPlaceholder"] = "想做点什么？按 Enter 快速添加",
        ["PriorityTooltip"] = "点击切换优先级：无 → 低 → 中 → 高",
        ["FilterAll"] = "全部",
        ["FilterActive"] = "待办",
        ["FilterDone"] = "已完成",
        ["DoneFmt"] = "已完成 {0} / {1}",
        ["ClearDone"] = "清除已完成",
        ["EmptyTitle"] = "这里空空如也",
        ["EmptyHint"] = "在上方输入一条待办，开始规划今天",
        ["EmptyHint2"] = "单击卡片可设置截止时间、提醒与子任务",
        ["ClickToView"] = "点击查看详情",
        ["ChevronTipSubs"] = "展开 / 收起子任务（单击卡片区域查看详情）",
        ["ChevronTipDetail"] = "查看详情：备注、截止时间、提醒、子任务",
        // 卡片/预览
        ["NoDetails"] = "未添加详细内容",
        ["MdRenderFail"] = "预览渲染出错（原文已保留，可继续编辑）",
        ["SubProgressFmt"] = "子任务 {0}/{1}",
        ["MetaPriorityFmt"] = "{0}优先级",
        ["MetaImagesFmt"] = "图片 {0} 张",
        ["MetaCreatedFmt"] = "创建于 {0}",
        ["PriorityNone"] = "无",
        ["PriorityLow"] = "低",
        ["PriorityMid"] = "中",
        ["PriorityHigh"] = "高",
        // 详情浮层
        ["DetailTitle"] = "待办详情",
        ["DetailsLabel"] = "详细内容（可选）· 支持 Markdown",
        ["InsertImage"] = "插入图片",
        ["ImportMd"] = "导入 MD",
        ["Preview"] = "预览",
        ["Edit"] = "编辑",
        ["PriorityLabel"] = "优先级",
        ["TagsLabel"] = "标签",
        ["TagAddTip"] = "输入标签后按 Enter 添加",
        ["ResizeTip"] = "拖动调整浮层大小",
        ["SubtasksLabel"] = "子任务",
        ["AddSubtaskPlaceholder"] = "添加子任务，按 Enter 确认",
        ["CreatedAtFmt"] = "创建于 {0}",
        ["ImportFailed"] = "导入失败",
        ["FilterImage"] = "图片|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp",
        ["FilterMd"] = "Markdown|*.md;*.markdown;*.txt",
        ["Minimize"] = "最小化",
        ["DataCorruptTitle"] = "数据文件读取失败",
        ["DataCorruptHint"] = "todos.json 已损坏，原文件已备份为 .corrupt-*，本次改动不会写盘，避免覆盖你的数据",
        ["PasteBlankTitle"] = "剪贴板里是空白图片",
        ["PasteBlankHint"] = "已插入，但图片内容为空白——来源应用可能没有提供有效图像数据，建议重新截图后再粘贴",
        // 提醒
        // 托盘
        ["TrayOpen"] = "打开清单",
        ["TrayMinToggle"] = "关闭时最小化到托盘",
        ["TrayHotkey"] = "快捷键",
        ["TrayHotkeyOff"] = "停用快捷键",
        ["TrayExit"] = "退出",
        ["TrayRunningTitle"] = "JeffBox 仍在运行",
        ["TrayRunningFmt"] = "已最小化到系统托盘，双击图标或按 {0} 恢复",
        ["HotkeyFailTitle"] = "快捷键注册失败",
        ["HotkeyFailFmt"] = "「{0}」可能被其他程序占用，请换一个",
        ["HotkeyPillTip"] = "全局快捷键：显示 / 隐藏窗口（设置中可修改）",
        ["CloseTipTray"] = "关闭 · 最小化到托盘",
        ["CloseTipExit"] = "退出",
        ["SettingsTip"] = "设置",
        // 设置
        ["SettingsTitle"] = "设置",
        ["LangLabel"] = "语言",
        ["LangZh"] = "简体中文",
        ["LangEn"] = "English",
        ["ThemeLabel"] = "外观",
        ["ThemeLight"] = "浅色",
                ["ThemeDark"] = "深色",
        ["AppTitle"] = "JeffBox",
        ["NavTodo"] = "待办",
        ["ConfirmDelete"] = "确定删除「{0}」？",
        ["CloseBehaviorLabel"] = "关闭方式",
        ["MinimizeToTray"] = "最小化到托盘",
        ["ExitDirectly"] = "直接退出",
        ["AutoStartLabel"] = "开机自启动",
        ["AutoStartHint"] = "开机后自动在系统托盘静默运行",
        ["OpenPosLabel"] = "打开位置",
        ["OpenPosRemember"] = "记住上次位置",
        ["OpenPosScreen"] = "鼠标所在屏幕居中",
        ["OpenPosNear"] = "鼠标附近",
        ["Ok"] = "确定",
        ["DontSave"] = "不保存",
        ["BackupLabel"] = "数据备份",
        ["BackupHint"] = "导出全部数据（待办/设置/附件）为 zip；导入会覆盖当前数据并自动重启",
        ["BackupExport"] = "导出备份",
        ["BackupImport"] = "导入恢复",
        ["BackupDoneFmt"] = "已备份到 {0}",
        ["BackupInvalid"] = "这不是有效的 JeffBox 备份文件",
        ["BackupImportConfirm"] = "导入将用备份内容覆盖当前全部数据（待办/设置/附件），完成后自动重启。继续？",
        ["HotkeyLabel"] = "全局快捷键",
        ["HotkeyHint"] = "随时呼出 / 隐藏窗口",
        ["HotkeyCapture"] = "按下组合键",
        ["HotkeyCaptureTip"] = "点击输入框，直接按下想要的组合键（Esc 取消）",
        ["HotkeyCancelHint"] = "正在录入…直接按下组合键；Esc 取消，停用请点右侧按钮",
        ["HotkeyOk"] = "已生效：随时呼出 / 隐藏窗口",
        ["HotkeyOffStatus"] = "已停用全局快捷键",
        ["HotkeyBadKey"] = "这个按键不支持作为全局快捷键，请换一个",
        ["HotkeyNoMod"] = "请至少包含 Ctrl 或 Alt（Shift、Win 只能作为附加修饰键）",
        ["HotkeyWinOnly"] = "仅 Win+按键会覆盖系统功能（如 Win+E 资源管理器），请再加上 Ctrl 或 Alt",
        ["HotkeyReservedFmt"] = "「{0}」是 Windows 保留组合（{1}），不能注册",
        ["HotkeyConflictRunningFmt"] = "「{0}」注册失败：与正在运行的 {1} 的{2}冲突。可换一个组合，或在 {1} 的设置里改掉它的热键",
        ["HotkeyConflictDefaultFmt"] = "「{0}」注册失败：这是 {1} 的默认{2}。请换一个组合",
        ["HotkeyConflictUnknown"] = "注册失败：该组合已被其他程序占用，Windows 不提供占用方信息。常见于截图/录屏、音乐、聊天、显卡工具等带全局热键的软件，请换一个组合",
        ["HotkeyMainLabel"] = "主热键",
        ["HotkeyTodoLabel"] = "直达 · 待办",
        ["HotkeySlotDupFmt"] = "与「{0}」重复，同一个组合不能绑定多个功能",
        ["HotkeyResponderFmt"] = "刚才按下时「{0}」做出了响应——它很可能就是占用方",
        ["AboutFmt"] = "JeffBox v{0} · 数据仅保存在本机",
    };

    static readonly Dictionary<string, string> En = new()
    {
        ["Add"] = "Add",
        ["Cancel"] = "Cancel",
        ["Save"] = "Save",
        ["Done"] = "Done",
        ["Delete"] = "Delete",
        ["Off"] = "Off",
        ["ImageAlt"] = "image",
        ["ImageMissing"] = "image missing",
        ["DetailToggle"] = "Notes",
        ["More"] = "More",
        ["Collapse"] = "Less",
        ["ExpandAll"] = "Expand all",
        ["CollapseAll"] = "Collapse all",
        ["NewDetailsPlaceholder"] = "Add details (optional, Markdown, paste images directly)",
        ["BreadcrumbFmt"] = "In: {0}",
        ["HeaderTitle"] = "Today",
        ["InputPlaceholder"] = "What needs doing? Press Enter",
        ["PriorityTooltip"] = "Click to cycle priority: None / Low / Med / High",
        ["FilterAll"] = "All",
        ["FilterActive"] = "Active",
        ["FilterDone"] = "Done",
        ["DoneFmt"] = "{0}/{1} done",
        ["ClearDone"] = "Clear done",
        ["EmptyTitle"] = "Nothing here yet",
        ["EmptyHint"] = "Add a todo above to plan your day",
        ["EmptyHint2"] = "Click a card for details, due date, reminder & subtasks",
        ["ClickToView"] = "Click for details",
        ["ChevronTipSubs"] = "Expand / collapse subtasks (click the card itself for details)",
        ["ChevronTipDetail"] = "Details: notes, due time, reminder, subtasks",
        ["NoDetails"] = "No details yet",
        ["MdRenderFail"] = "Preview rendering failed (text kept, you can keep editing)",
        ["SubProgressFmt"] = "Subtasks {0}/{1}",
        ["MetaPriorityFmt"] = "{0} priority",
        ["MetaImagesFmt"] = "{0} image(s)",
        ["MetaCreatedFmt"] = "Created {0}",
        ["PriorityNone"] = "None",
        ["PriorityLow"] = "Low",
        ["PriorityMid"] = "Med",
        ["PriorityHigh"] = "High",
        ["DetailTitle"] = "Task details",
        ["DetailsLabel"] = "Details (optional) · Markdown supported",
        ["InsertImage"] = "Image",
        ["ImportMd"] = "Import MD",
        ["Preview"] = "Preview",
        ["Edit"] = "Edit",
        ["PriorityLabel"] = "Priority",
        ["TagsLabel"] = "Tags",
        ["TagAddTip"] = "Type a tag and press Enter",
        ["ResizeTip"] = "Drag to resize",
        ["SubtasksLabel"] = "Subtasks",
        ["AddSubtaskPlaceholder"] = "Add a subtask, press Enter",
        ["CreatedAtFmt"] = "Created {0}",
        ["ImportFailed"] = "Import failed",
        ["FilterImage"] = "Images|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp",
        ["FilterMd"] = "Markdown|*.md;*.markdown;*.txt",
        ["Minimize"] = "Minimize",
        ["DataCorruptTitle"] = "Failed to read data file",
        ["DataCorruptHint"] = "todos.json is corrupt; a .corrupt-* backup was kept and writing is disabled this session to protect your data",
        ["PasteBlankTitle"] = "Blank image in clipboard",
        ["PasteBlankHint"] = "Inserted, but the image is blank — the source app may not have provided valid image data. Try taking a new screenshot",
        ["TrayOpen"] = "Open",
        ["TrayMinToggle"] = "Minimize to tray on close",
        ["TrayHotkey"] = "Hotkey",
        ["TrayHotkeyOff"] = "Disable hotkey",
        ["TrayExit"] = "Exit",
        ["TrayRunningTitle"] = "JeffBox is still running",
        ["TrayRunningFmt"] = "Minimized to tray. Double-click the icon or press {0}",
        ["HotkeyFailTitle"] = "Hotkey failed",
        ["HotkeyFailFmt"] = "\"{0}\" may be in use; pick another",
        ["HotkeyPillTip"] = "Global hotkey: show / hide window (change in Settings)",
        ["CloseTipTray"] = "Close · to tray",
        ["CloseTipExit"] = "Exit",
        ["SettingsTip"] = "Settings",
        ["SettingsTitle"] = "Settings",
        ["LangLabel"] = "Language",
        ["LangZh"] = "简体中文",
        ["LangEn"] = "English",
        ["ThemeLabel"] = "Theme",
        ["ThemeLight"] = "Light",
                ["ThemeDark"] = "Dark",
        ["AppTitle"] = "JeffBox",
        ["NavTodo"] = "Todo",
        ["ConfirmDelete"] = "Delete \"{0}\"?",
        ["CloseBehaviorLabel"] = "On close",
        ["MinimizeToTray"] = "Minimize to tray",
        ["ExitDirectly"] = "Exit",
        ["AutoStartLabel"] = "Start with Windows",
        ["AutoStartHint"] = "Run silently in the tray after boot",
        ["OpenPosLabel"] = "Open position",
        ["OpenPosRemember"] = "Last position",
        ["OpenPosScreen"] = "Center on mouse screen",
        ["OpenPosNear"] = "Near mouse pointer",
        ["Ok"] = "OK",
        ["DontSave"] = "Don't save",
        ["BackupLabel"] = "Backup",
        ["BackupHint"] = "Export all data (todos/settings/attachments) to a zip; importing overwrites current data and restarts the app",
        ["BackupExport"] = "Export",
        ["BackupImport"] = "Import",
        ["BackupDoneFmt"] = "Backed up to {0}",
        ["BackupInvalid"] = "Not a valid JeffBox backup file",
        ["BackupImportConfirm"] = "Importing overwrites ALL current data (todos/settings/attachments) and restarts the app. Continue?",
        ["HotkeyLabel"] = "Global hotkey",
        ["HotkeyHint"] = "Show or hide the window anytime",
        ["HotkeyCapture"] = "Press a combo",
        ["HotkeyCaptureTip"] = "Click the box and press the combination directly (Esc to cancel)",
        ["HotkeyCancelHint"] = "Recording… press a combo now; Esc to cancel, button to disable",
        ["HotkeyOk"] = "Active — show or hide the window anytime",
        ["HotkeyOffStatus"] = "Global hotkey disabled",
        ["HotkeyBadKey"] = "This key can't be used as a global hotkey; try another",
        ["HotkeyNoMod"] = "Include Ctrl or Alt (Shift/Win only as extra modifiers)",
        ["HotkeyWinOnly"] = "Win+key alone overrides system combos (e.g. Win+E); add Ctrl or Alt",
        ["HotkeyReservedFmt"] = "\"{0}\" is reserved by Windows ({1}) and can't be registered",
        ["HotkeyConflictRunningFmt"] = "\"{0}\" failed: it conflicts with {1} (running) — {2}. Change the combo or edit the hotkey in {1}'s settings",
        ["HotkeyConflictDefaultFmt"] = "\"{0}\" failed: it's {1}'s default {2}. Pick another combo",
        ["HotkeyConflictUnknown"] = "Failed: the combo is taken by another app and Windows doesn't reveal the owner — commonly screen capture, music, chat, or GPU tools. Try another combo",
        ["HotkeyMainLabel"] = "Main hotkey",
        ["HotkeyTodoLabel"] = "Direct · Todo",
        ["HotkeySlotDupFmt"] = "Same as \"{0}\" — one combo can't bind multiple actions",
        ["HotkeyResponderFmt"] = "\"{0}\" just reacted to this combo — it's very likely the owner",
        ["AboutFmt"] = "JeffBox v{0} · Data stays on this PC",
    };

    public static string Get(string key)
    {
        var dict = Lang == "en" ? En : Zh;
        return dict.TryGetValue(key, out var v) ? v : key;
    }

    public static string F(string key, params object[] args) => string.Format(Get(key), args);

    public static string PriorityName(int p) => p switch
    {
        1 => Get("PriorityLow"),
        2 => Get("PriorityMid"),
        3 => Get("PriorityHigh"),
        _ => Get("PriorityNone"),
    };

    void OnLangChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));

    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
