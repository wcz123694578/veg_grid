# Vegas Grid布局器
- 本项目是模仿wpf的grid布局方式，大战chatgpt实现的vegas网格布局器，目前还有很多bug，**并且还没做前端，只能引用VegGridLayouter.Core.dll类库自己写布局代码直接加到vegas脚本目录里**。
- 因为项目新建文件夹时的目标是写一个完整的布局框架，所以把很多东西按自己的使用习惯都封装了，结果最后怎么也写不出来。
- **目前生成时是直接拉伸轨道，以后会改成用蒙版**
## 使用方式
在FromVegas入口添加如下代码来将vegas对象添加到VegasManager中，这样VegGrid最终渲染的时候才能访问到vegas对象。
```csharp
void FromVegas(Vegas vegas) {
    VegasManager.Instance = vegas;
}
```
接下来就是实例化VegGrid了，而且布局方式和wpf里动态生成grid的方式类似（不会wpf的小盆友有难了，具体咋搞去学一下吧，本来就没做前端，以后会做得\(\*\^_\^*)），不过Auto类尺寸实现还不完善，先注释掉了。
VegGrid继承自VegElement类，里面有一个CurVegas和CurProject来维护当前指向的工程。
```csharp
VegGrid grid = new VegGrid();
grid.AddRow(new RowDefinition { Type = GridSizeType.Fixed, Value = 50 });
grid.AddRow(new RowDefinition { Type = GridSizeType.Star, Value = 1 });
grid.AddRow(new RowDefinition { Type = GridSizeType.Star, Value = 2 });

grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Fixed, Value = 100 });
grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Star, Value = 2 });
grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Star, Value = 1 });

grid.AddChild(new GridChild { Row = 0, Column = 0 });
grid.AddChild(new GridChild { Row = 1, Column = 1, ColumnSpan = 2 });
grid.AddChild(new GridChild { Row = 2, Column = 2 });
```
最后就是计算并生成布局，这样就可以自己使用了
```csharp
grid.Generate();
```