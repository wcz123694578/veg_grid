using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;

namespace VegGridLayouter.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set {
                _code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }

        public MainWindowViewModel()
        {
            GenerateCommand = new DelegateCommand(generate);
        }

        public DelegateCommand GenerateCommand { get; set; }

        private void generate()
        {
            VegGrid grid = new VegGrid();
            // FileStream fs = File.Open("test.xml", FileMode.Open);
            try
            {
                grid = VegXmlDeserializer.DeserializeXmlString(this._code);

                int width = grid.CurProject.Video.Width;
                int height = grid.CurProject.Video.Height;

                grid.CalculateLayout(width, height);

                grid.Generate();

                MessageBox.Show("生成完毕！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
