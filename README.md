# UAct

UAct is a powerful and flexible Unity Editor extension tool designed to simplify and automate asset creation and editing workflows in Unity projects. It use command pattern design to provide modular, extensible functionality that enhances development efficiency.

## v0.1.0 - Aug 2025 Update

Initial release featuring core framework and two main functional modules.

## Features

### Batch Processing Module

Efficiently handle multiple assets simultaneously with these tools:

- **Extract Material**: Extract materials from 3D models and save them as separate assets
- **Assign Texture**: Batch assign textures to materials based on naming conventions or configuration files
- **Invert Textures G Channel**: Process texture green channel in bulk (commonly used for normal map adjustments)

### Code Generator Module

Automate code and asset creation to reduce manual work:

- **Excel to ScriptableObject**: Generate ScriptableObject classes and instances from Excel spreadsheets
- **Mermaid to C#**: Convert Mermaid class diagrams in Markdown files into properly structured C# class files
- **JSON to ScriptableObject**: Create ScriptableObject instances from JSON configuration files

## Installation

1. Download the UAct Unity Package or source code from the GitHub repository
2. Import the package into your Unity project (Unity 2021.3 or newer recommended)
3. The UAct menu will appear in the Unity Editor menu bar

## Requirements

- Unity 2021.3 or newer
- .NET Standard 2.1 compatible
- For Excel functionality: EPPlus library (included in Module/Libs)
