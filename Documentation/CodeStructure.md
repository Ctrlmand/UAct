# UAct 项目代码结构文档

## 项目概览

UAct 是一个 Unity 编辑器扩展工具，旨在简化和自动化 Unity 项目中资源的创建与编辑流程。

- **版本**: v0.1.0 (Aug 2025)
- **主要功能**: 提供批处理工具和代码生成器，以提高游戏开发效率
- **设计模式**: 采用命令模式设计，使功能模块化且易于扩展

## 目录结构

UAct 项目采用清晰的模块化结构设计，将核心框架与功能模块分离。整体架构遵循 Unity 编辑器扩展的最佳实践，便于维护和扩展。

```text
├── .gitignore
├── Documentation\         # 项目文档
│   ├── CodeStructure.md
│   └── CodeStructure_EN.md
├── EditorFramework\        # 核心框架代码
│   ├── Class\              # 编辑器窗口基类
│   │   └── EditorWindowBase.cs
│   └── Command\            # 命令模式实现
│       ├── CommandCache.cs
│       └── ICommand.cs
├── LICENSE
├── Module\                 # 功能模块
│   ├── Batch\              # 批处理相关命令
│   │   ├── Batch.cs
│   │   └── Commands\       # 批处理命令实现
│   ├── Generator\          # 生成器相关功能
│   │   ├── Commands\       # 生成器命令实现
│   │   └── Generator.cs
│   ├── Libs\               # 第三方库
│   │   ├── EPPlus.dll
│   │   └── EPPlusInfo.md
│   └── Utility\            # 通用工具类
│       ├── AssetMethod.cs
│       ├── ExcelRead.cs
│       ├── GenerateCode.cs
│       └── SerializeData.cs
├── Presets\                # 预设配置文件
│   └── DefaultMap.json
└── README.md               # 项目说明文档
```

### 目录职责说明

| 目录/文件              | 主要职责                                       | 文件位置                                           |
|----------------------|----------------------------------------------|--------------------------------------------------|
| EditorFramework      | 提供框架基础结构，包括窗口基类和命令系统           | `EditorWindowBase.cs` (路径: `UAct\EditorFramework\Class\EditorWindowBase.cs`) |
| Module/Batch         | 提供资源批处理功能                              | `Batch.cs` (路径: `UAct\Module\Batch\Batch.cs`) |
| Module/Batch/Commands  | 包含具体的批处理命令实现                         | (路径: `UAct\Module\Batch\Commands\`) |
| Module/Generator     | 提供代码生成功能                                | `Generator.cs` (路径: `UAct\Module\Generator\Generator.cs`) |
| Module/Generator/Commands | 包含具体的生成器命令实现                         | (路径: `UAct\Module\Generator\Commands\`) |
| Module/Libs          | 存储第三方库                                    | `EPPlus.dll` (路径: `UAct\Module\Libs\EPPlus.dll`) |
| Module/Utility       | 提供通用工具方法，如资源操作和代码生成辅助         | `AssetMethod.cs` (路径: `UAct\Module\Utility\AssetMethod.cs`) |
| Presets              | 存储预设配置文件                                | `DefaultMap.json` (路径: `UAct\Presets\DefaultMap.json`) |
| Documentation        | 包含项目文档                                    | `CodeStructure.md` (路径: `UAct\Documentation\CodeStructure.md`) |

## 核心框架详解

UAct 采用命令模式设计，将功能封装为独立的命令对象，通过命令缓存管理命令实例。这种设计使得功能可以独立开发和测试，同时保持界面与业务逻辑的分离。

### 1. 命令系统

#### 1.1 ICommand 接口

命令接口是整个命令系统的基础，所有命令都必须实现此接口。

```csharp
public interface ICommand
{
    public void Execute(ICommandContext context);
}
```

每个命令通过 `Execute` 方法执行具体功能，通过 `ICommandContext` 参数传递所需数据。

文件位置: `ICommand.cs` (路径: `UAct\EditorFramework\Command\ICommand.cs`)

#### 1.2 CommandCache 命令缓存

命令缓存采用单例模式实现，负责创建和管理命令实例，避免重复创建相同类型的命令对象。

```csharp
public static class CommandCache
{
    private static Dictionary<Type, ICommand> m_Commands = new();

    public static T GetCommand<T>() where T : ICommand, new()
    {
        if (!m_Commands.TryGetValue(typeof(T), out var command))
        {
            command = new T();
            m_Commands[typeof(T)] = command;
        }
        return (T)command;
    }
}
```

文件位置: `CommandCache.cs` (路径: `UAct\EditorFramework\Command\CommandCache.cs`)

#### 1.3 ICommandContext 与 BaseCommandContext

命令上下文用于在命令执行过程中传递数据，支持泛型类型以确保类型安全。

```csharp
public interface ICommandContext
{
    T GetData<T>();
    void SetData<T>(T data);
    bool HasData<T>();
}

public class BaseCommandContext : ICommandContext
{
    private Dictionary<Type, object> _data = new();
    // 实现接口方法...
}
```