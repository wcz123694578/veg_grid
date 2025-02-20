# Vegas Grid布局器
- 本项目是模仿wpf的grid布局方式，大战chatgpt实现的vegas网格布局器，目前还有很多bug，xml还没做语法检查，只能一次性写对。~~并且还没做前端，只能引用VegGridLayouter.Core.dll类库自己写布局代码直接加到vegas脚本目录里~~。
- 因为项目新建文件夹时的目标是写一个完整的布局框架，所以把很多东西按自己的使用习惯都封装了，结果最后怎么也写不出来。
- ~~目前生成时是直接拉伸轨道，以后会改成用蒙版~~
- 编译前需要给VegGridLayouter.UI项目安装如下nuget包：
    1 Prism.Wpf
    2 AvalonEdit
    3 Microsoft.Xaml.Behavior

TODO:
    - [x] XML解析以及简单的UI功能
    - [ ] 简单的预设，比如输入n*n就可以生成这么多网格 
## 使用方式
目前是通过解析xml字符串填充到网格对象里再生成。
```xml
<?xml version="1.0" encoding="utf-8" ?>
<VegGrid>
  <RowDefinitions> <!-- 定义行 -->
    <RowDefinition Type="Fixed" Value="200"/>
    <RowDefinition Type="Star" Value="1"/>
    <RowDefinition Type="Fixed" Value="100"/>
    <RowDefinition Type="Star" Value="1"/>
  </RowDefinitions>
  <ColumnDefinitions> <!-- 定义列 -->
    <ColumnDefinition Type="Star" Value="1"/>
    <ColumnDefinition Type="Star" Value="2"/>
    <ColumnDefinition Type="Star" Value="1"/>
  </ColumnDefinitions>
  <Children>
    <GridChild Row="0"/>
    <GridChild Row="1" Column="1" ColumnSpan="2"/>
    <GridChild Row="2" Column="2"/>
    <GridChild Row="3" Column="1"/>
  </Children>
</VegGrid>
```
点击“生成”按钮就可以关闭窗口查看效果了