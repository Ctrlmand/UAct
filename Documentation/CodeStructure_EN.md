# UAct Project Code Structure Documentation

## Project Overview

UAct is a Unity Editor extension tool designed to simplify and automate the process of creating and editing assets in Unity projects.

- **Version**: v0.1.0 (Aug 2025)
- **Main Features**: Provides batch processing tools and code generators to improve game development efficiency
- **Design Pattern**: Adopts command pattern design to make functions modular and easily extensible

## Directory Structure

The UAct project adopts a clear modular structure design, separating the core framework from functional modules. The overall architecture follows Unity Editor extension best practices, making it easy to maintain and extend.

```text
├── .gitignore
├── Documentation\         # Project documentation
│   ├── CodeStructure.md
│   └── CodeStructure_EN.md
├── EditorFramework\        # Core framework code
│   ├── Class\              # Editor window base class
│   │   └── EditorWindowBase.cs
│   └── Command\            # Command pattern implementation
│       ├── CommandCache.cs
│       └── ICommand.cs
├── LICENSE
├── Module\                 # Functional modules
│   ├── Batch\              # Batch processing related commands
│   │   ├── Batch.cs
│   │   └── Commands\       # Batch command implementations
│   ├── Generator\          # Generator related functions
│   │   ├── Commands\       # Generator command implementations
│   │   └── Generator.cs
│   ├── Libs\               # Third-party libraries
│   │   ├── EPPlus.dll
│   │   └── EPPlusInfo.md
│   └── Utility\            # Common utility classes
│       ├── AssetMethod.cs
│       ├── ExcelRead.cs
│       ├── GenerateCode.cs
│       └── SerializeData.cs
├── Presets\                # Preset configuration files
│   └── DefaultMap.json
└── README.md               # Project documentation
```

### Directory Responsibility Description

| Directory/File          | Main Responsibility                            | File Location                                         |
|------------------------|------------------------------------------------|------------------------------------------------------|
| EditorFramework        | Provides basic framework structure, including window base classes and command system | `EditorWindowBase.cs` (Path: `UAct\EditorFramework\Class\EditorWindowBase.cs`) |
| Module/Batch           | Provides asset batch processing functions      | `Batch.cs` (Path: `UAct\Module\Batch\Batch.cs`) |
| Module/Batch/Commands  | Contains specific batch command implementations | (Path: `UAct\Module\Batch\Commands\`) |
| Module/Generator       | Provides code generation functions             | `Generator.cs` (Path: `UAct\Module\Generator\Generator.cs`) |
| Module/Generator/Commands | Contains specific generator command implementations | (Path: `UAct\Module\Generator\Commands\`) |
| Module/Libs            | Stores third-party libraries                   | `EPPlus.dll` (Path: `UAct\Module\Libs\EPPlus.dll`) |
| Module/Utility         | Provides common utility methods, such as resource operations and code generation assistance | `AssetMethod.cs` (Path: `UAct\Module\Utility\AssetMethod.cs`) |
| Presets                | Stores preset configuration files              | `DefaultMap.json` (Path: `UAct\Presets\DefaultMap.json`) |
| Documentation          | Contains project documentation                 | `CodeStructure.md` (Path: `UAct\Documentation\CodeStructure.md`) |

## Core Framework Details

UAct adopts command pattern design, encapsulating functions as independent command objects and managing command instances through command cache. This design allows functions to be developed and tested independently while keeping the interface separate from business logic.

### 1. Command System

#### 1.1 ICommand Interface

The command interface is the foundation of the entire command system, and all commands must implement this interface.

```csharp
public interface ICommand
{
    public void Execute(ICommandContext context);
}
```

Each command executes specific functions through the `Execute` method, passing required data through the `ICommandContext` parameter.

File Location: `ICommand.cs` (Path: `UAct\EditorFramework\Command\ICommand.cs`)

#### 1.2 CommandCache Command Cache

Command cache is implemented using the singleton pattern, responsible for creating and managing command instances to avoid repeatedly creating command objects of the same type.

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

File Location: `CommandCache.cs` (Path: `UAct\EditorFramework\Command\CommandCache.cs`)

#### 1.3 ICommandContext and BaseCommandContext

Command context is used to pass data during command execution, supporting generic types to ensure type safety.

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
    // Interface method implementation...
}
```