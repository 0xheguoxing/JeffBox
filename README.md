# JeffBox

**[中文说明](README.zh-CN.md)**

> A ~430 KB single-file Windows desktop todo app — task tree, priorities, tags & markdown details.

![License](https://img.shields.io/badge/License-Apache--2.0-blue)

![Todo](docs/screenshots/todo-light.png)

## Why JeffBox

- **One exe, double-click and go.** Built natively on .NET 10 + WPF (Fluent controls), zero external dependencies, ~430 KB as a single-file publish.
- **Your data never leaves your PC.** Everything is stored in a `data\` folder next to the exe (portable mode). No account, no cloud sync, no telemetry.
- **Always one hotkey away.** Tray-resident with two global hotkey slots — one master hotkey to summon/hide the window, plus one direct hotkey for your todo list.
- **Performance first.** Long todo lists are UI-virtualized; task details preview renders instantly.
- **Dark / light themes + English / 简体中文 UI**, polished with rounded cards, overlay scrollbars and smooth animations.

## Features

### ✅ Todo

- Task tree: unlimited nesting of subtasks, expanded by default, collapse/expand-all in one click
- Priority (none/low/med/high with color bars) and multiple tags per task
- Markdown-lite details with `Ctrl+V` image paste (blank-clipboard detection included), edit ⇄ preview inline
- Filters (all/active/done), progress display, hover card preview

### Global

- Tray-resident (switchable to "exit on close"), optional auto-start, single instance
- **Fully customizable hotkeys**: click the box and just press the combo. On failure, three-layer conflict diagnosis — a knowledge base of common defaults (WeChat/QQ/DingTalk/NetEase/Snipaste…), **live response forensics** (names the app that actually reacted to your keypress), then generic guidance. Reserved combos (Alt+F4 etc.) are blocked up front
- Window state memory: position/size/maximized, multi-monitor safe
- **Open where your mouse is**: summon the window by hotkey, tray icon or shortcut and it appears on the mouse's monitor (centered) or right beside the cursor — configurable in Settings, off by default
- **Backup export / import**: zip the whole data folder and restore it anywhere, with a safety copy before import

## Getting started

**Option 1 — grab a release**

`JeffBox.exe` (~430 KB) — single-file build that uses the locally installed .NET Desktop Runtime 10.x. For a fully standalone exe, add `--self-contained true` to the publish command below.

**Option 2 — build from source**

```bash
git clone https://github.com/Jeffrey56400/JeffBox.git
cd JeffBox

# run for debugging
dotnet run

# publish a single-file exe (~430 KB)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish
```

Requires .NET SDK 10.0+, Windows 10 1809+.

## Default shortcuts

| Action | Default | Notes |
| --- | --- | --- |
| Show / hide window | `Ctrl+Alt+T` | Master hotkey, fully customizable |
| Direct to Todo | unbound | Press any combo in Settings, e.g. `Ctrl+Alt+D` |
| Submit task from multi-line details | `Ctrl+Enter` | In the create card |

## Data & privacy

- All data lives in a `data\` folder next to the exe (`todos.json` / `settings.json` / `attachments\`). Delete that folder to wipe everything. Existing `%APPDATA%\JeffBox` data is migrated there automatically on first run.
- No network calls, no telemetry, no auto-update. The only outbound behavior is opening links you click.
- Saves are coalesced (400 ms debounce), serialized off the UI thread and written atomically (temp file + replace); corrupt files get a `.corrupt-*` backup and writes are halted instead of overwriting.

## Project layout

```
JeffBox/
├── MainWindow.xaml(.cs)      # shell: custom title bar, tray, hotkeys, settings overlay
├── TodoViewModel.cs          # todo-tree viewmodel (subtask sync, filtering, breadcrumbs)
├── MarkdownLite.cs           # Markdown-lite renderer for task details (streaming blocks)
├── Models/TodoItem.cs        # task-tree model (legacy format auto-migration)
├── Services/
│   ├── TodoStorage.cs        # todo persistence (atomic writes, corruption guard)
│   ├── AppSettings.cs        # settings persistence (window state, hotkeys, theme)
│   ├── AppPaths.cs           # data directory + legacy migration
│   ├── Theme.cs / Loc.cs     # light/dark palettes + Fluent ThemeMode / i18n dictionaries
│   ├── HotkeyInfo.cs         # hotkey parsing, reserved combos, conflict KB
│   ├── Attachments.cs        # todo image attachments (path-traversal guard)
│   └── DataBackup.cs         # full-data zip export / import
└── Views/
    ├── HotkeyBox.*           # reusable hotkey capture box (low-level keyboard hook)
    └── ThemeDialog.cs        # themed in-app dialogs
```

## Technical highlights

- **Markdown-lite details** — task details render block by block with inline images; multi-MB text stays responsive
- **Three-layer hotkey conflict diagnosis** — Windows offers no API to query hotkey ownership, so JeffBox combines a defaults knowledge base (checked against running processes) with **live response forensics**: a low-level keyboard hook snapshots the foreground before dispatch and names the app that reacts to your keypress
- **Custom title bar done right** — WindowChrome maximization is compensated with `SM_CXSIZEFRAME + SM_CXPADDEDBORDER` metrics, avoiding the classic 8-px clipped-content bug
- **Clipboard image defense chain** — raw PNG bytes first, DIB alpha-loss detection for fake-transparency images, blank-image warnings
- **Fluent + Mica on .NET 10** — WPF's built-in Fluent theme (`Application.ThemeMode`) follows the app's light/dark palette, and the Win11 system Mica backdrop (`DWMWA_SYSTEMBACKDROP_TYPE`) lets the desktop wallpaper tint show through the window (solid fallback on older systems) — all BCL/P-Invoke, zero extra packages
- **Zero dependencies** — tray, hotkeys, Markdown rendering and encoding handling are all BCL/P-Invoke; no third-party packages

Feature requests and feedback are welcome in Issues (中文/English).

## Contributing

PRs welcome. Please attach screenshots or repro steps with changes, and keep the two ground rules: zero external dependencies, all data local.

## License

[Apache-2.0](LICENSE) © 2026 Jeffrey56400
