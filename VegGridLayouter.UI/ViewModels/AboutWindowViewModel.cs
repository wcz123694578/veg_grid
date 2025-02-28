using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VegGridLayouter.UI.ViewModels
{
    public class AboutWindowViewModel
    {
        public ObservableCollection<ScriptInfoItem> ScriptInfos { get; set; }
        public string VersionString { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        

        public AboutWindowViewModel()
        {
            ScriptInfos = new ObservableCollection<ScriptInfoItem>()
            {
                new ScriptInfoItem("作者", "wcz123694578"),
                new ScriptInfoItem("仓库地址", "https://github.com/wcz123694578/veg_grid") { IsUrl = true },
                new ScriptInfoItem("作者的话", "本项目是模仿wpf的grid布局方式，大战chatgpt实现的vegas网格布局器，目前还有很多bug，xml还没做语法检查，只能一次性写对。\n嵌套网格生成过程中会有TempChild这样的临时轨道生成，删掉就行了。"),
                new ScriptInfoItem("版本号", VersionString)
            };
        }
    }

    public class ScriptInfoItem
    {
        public ScriptInfoItem(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUrl { get; set; } = false;
    }
}
