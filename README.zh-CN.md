> **中文说明** · [English](README.md)

# JeffBox

> 一个约 430KB 的单文件 Windows 桌面待办应用 —— 任务树、优先级、标签与 Markdown 详情。

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4) ![WPF](https://img.shields.io/badge/UI-WPF%20%2B%20Fluent-0078D4) ![License](https://img.shields.io/badge/License-Apache--2.0-blue) ![Platform](https://img.shields.io/badge/Platform-Windows-0078D6) ![Dependencies](https://img.shields.io/badge/dependencies-zero-green)

![Todo](docs/screenshots/todo-light.png)

## 为什么是 JeffBox

- **一个 exe，双击即用**：.NET 10 + WPF（Fluent 控件）原生开发，零外部依赖，单文件发布约 430KB
- **数据 100% 留在本机**：所有内容保存在 exe 旁的 `data\` 目录（便携模式，跟随程序），无账号、无云同步、无遥测
- **常驻后台，随叫随到**：托盘常驻 + 双全局热键槽位（主热键呼出/隐藏窗口，另可为待办页绑一个直达热键）
- **性能优先**：长待办列表 UI 虚拟化，任务详情即时渲染，空闲内存占用低
- **深色 / 浅色主题 + 中英双语**，界面细节按现代桌面规范打磨（圆角卡片、覆盖式细滚动条、流畅动画）

## 功能一览

### ✅ 待办 Todo

- 任务树：子任务无限嵌套，默认展开，一键全部展开/收起
- 优先级（无/低/中/高，列表彩条标识）与多标签（卡片直接展示，详情内编辑）
- 详情支持 Markdown-lite，可直接 `Ctrl+V` 粘贴图片（自动检测剪贴板空白图并提示），编辑 ⇄ 预览同屏切换
- 筛选（全部/待办/已完成）、显示完成进度、悬停卡片预览

### 全局能力

- 托盘常驻（可切换"关闭即退出"）、开机自启（可选）、单实例
- **可自定义全局热键**：设置中点输入框直接按组合即录入；注册失败时三层冲突识别——常见软件默认热键知识库（微信/QQ/钉钉/网易云/Snipaste 等，运行中的给强提示）→ **按键响应取证**（按下时哪个程序抢了前台就点名谁）→ 通用指引；系统保留组合（Alt+F4 等）自动拦截
- 窗口状态记忆：位置/尺寸/最大化，多显示器拔插不丢窗口
- **在鼠标处打开**：热键、托盘或快捷方式呼出窗口时，出现在鼠标所在屏幕居中 / 鼠标指针旁边（设置可选，默认关闭）
- **数据备份导出 / 导入**：整个数据目录打包成 zip，跨机器恢复，导入前自动留安全副本

## 快速开始

**方式一：发布产物**

`JeffBox.exe`（约 430 KB）——单文件轻量版，直接使用本机已安装的 .NET Desktop Runtime 10.x。如需完全独立 exe，在下方发布命令加 `--self-contained true`。

**方式二：从源码构建**

```bash
git clone https://github.com/Jeffrey56400/JeffBox.git
cd JeffBox

# 调试运行
dotnet run

# 发布单文件 exe（约 430KB）
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish
```

要求：.NET SDK 10.0+，Windows 10 1809+。

## 默认快捷键

| 操作 | 默认 | 说明 |
| --- | --- | --- |
| 呼出 / 隐藏窗口 | `Ctrl+Alt+T` | 主热键，可在设置中自定义任意组合 |
| 直达待办 | 未绑定 | 设置中按组合即录入，如 `Ctrl+Alt+D` |
| 多行详情中直接提交任务 | `Ctrl+Enter` | 创建卡内 |

## 数据与隐私

- 全部数据保存在 exe 旁的 `data\` 目录（`todos.json` / `settings.json` / `attachments\`），删除该目录即完全卸载数据；旧版 `%APPDATA%\JeffBox` 的数据会在首次运行时自动搬过来
- 无网络请求、无遥测、无自动更新；唯一的外部行为是你主动打开链接
- 保存合并写入（400ms 防抖）、序列化在后台线程、临时文件 + 原子替换，断电/崩溃不会损坏数据；检测到损坏时自动备份 `.corrupt-*` 并停止覆盖写入

## 项目结构

```
JeffBox/
├── MainWindow.xaml(.cs)      # 壳：自定义标题栏、托盘、热键、设置浮层
├── TodoViewModel.cs          # 待办树 ViewModel（子任务同步、筛选、面包屑）
├── MarkdownLite.cs           # 任务详情 Markdown-lite 渲染器（分块产出）
├── Models/TodoItem.cs        # 任务树数据模型（自动迁移旧格式）
├── Services/
│   ├── TodoStorage.cs        # 待办持久化（原子写、损坏保护、迁移）
│   ├── AppSettings.cs        # 设置持久化（窗口状态、热键、主题语言）
│   ├── AppPaths.cs           # 数据目录统一入口 + 旧版迁移
│   ├── Theme.cs / Loc.cs     # 深浅主题调色板 + Fluent ThemeMode / 中英双语词条
│   ├── HotkeyInfo.cs         # 热键解析、保留组合校验、冲突知识库
│   ├── Attachments.cs        # 待办图片附件（防路径逃逸、孤儿清理）
│   └── DataBackup.cs         # 全量数据 zip 导出 / 导入
└── Views/
    ├── HotkeyBox.*           # 可复用热键捕获框（低级键盘钩子）
    └── ThemeDialog.cs        # 应用内主题化对话框
```

## 技术亮点

- **Markdown-lite 详情渲染**：按块产出 UI 元素、支持内联图片，大段文字依旧流畅
- **热键系统的三层防御**：`RegisterHotKey` 失败 → 知识库匹配（常见软件默认热键 × 运行进程佐证）→ **按键响应取证**（低级键盘钩子记录按键分发前后的前台变化，真实点名占用者）——Windows 不提供热键占用方查询 API，这是无侵入的近似定位方案
- **WPF 自定义标题栏的边角打磨**：WindowChrome 最大化时按系统指标（`SM_CXSIZEFRAME + SM_CXPADDEDBORDER`）动态补偿边距，避免内容被裁 8px 的经典问题
- **剪贴板图像防御链**：PNG 原始字节优先 → DIB 丢 alpha 检测（假透明图丢弃 alpha 通道）→ 全空白图用户提示
- **.NET 10 上的 Fluent + 云母**：WPF 内置 Fluent 主题（`Application.ThemeMode`）跟随深浅色切换；Win11 系统云母材质（`DWMWA_SYSTEMBACKDROP_TYPE`）让桌面壁纸色调透进窗口，老系统自动回退实色；全部 BCL/P-Invoke，零第三方包
- **零依赖单文件**：所有功能（托盘、热键、Markdown 渲染、编码处理）均由 BCL/P-Invoke 实现，无第三方 NuGet 包

欢迎在 Issues 提需求与反馈（中文/English）。

## 贡献

欢迎 PR。建议：改动附截图/复现步骤；新功能保持"零依赖 + 全本地"两条底线。

## License

[Apache-2.0](LICENSE) © 2026 Jeffrey56400
