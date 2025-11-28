# UAct

Unity编辑器扩展。

## 功能

### 资产处理

- 提取材质：从3D模型中提取材质,将其保存至模型同目录下的Materials文件夹。
- 分配纹理：根据命名规则或配置文件为材质分配纹理。
- FBX生成Prefab：按fbx中网格名称，将FBX模型转换为若干Prefab资产，存储到指定位置。预制体中所有网格的变换将会归0。

### 代码生成

- Excel转ScriptableObject：从Excel电子表格生成ScriptableObject类和实例
- Mermaid转C#：将Markdown文件中的Mermaid类图转换为同结构的C#类文件
- JSON转ScriptableObject：从JSON配置文件创建ScriptableObject实例

## 使用方法

- 1.从GitHub存储库下载UAct Unity包或源代码
- 2.将包导入到Unity项目中（建议使用Unity 2021.3或更新版本）
- 3.UAct菜单将出现在Unity编辑器菜单栏中
