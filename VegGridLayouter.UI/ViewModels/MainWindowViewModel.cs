using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Text;
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

        private string _log;

        public string Log
        {
            get { return _log; }
            set {
                _log = value;
                RaisePropertyChanged(nameof(Log));
            }
        }


        public MainWindowViewModel()
        {
            GenerateCommand = new DelegateCommand(generate);
            LoadCommand = new DelegateCommand(load);
            SaveCommand = new DelegateCommand(save);

            LoadLog();
        }

        private string logFileName = "layouter_log.txt";

        private void LoadLog()
        {
            string filename = logFileName;
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    Log = Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception ex)
            {
                printLog(ex.ToString());
            }
        }

        public DelegateCommand GenerateCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        public FoldingManager foldingManager { get; set; }
        public XmlFoldingStrategy foldingStrategy { get; set; }

        public string FilePath { get; set; } = "";

        private void generate()
        {
            VegGrid grid = new VegGrid();
            // FileStream fs = File.Open("test.xml", FileMode.Open);
            try
            {
                debugXml("解析XML...");

                grid = VegXmlDeserializer.DeserializeXmlString(this._code);

                int width = grid.CurProject.Video.Width;
                int height = grid.CurProject.Video.Height;

                grid.Generate();
                debugXml("生成完毕！");
                MessageBox.Show("生成完毕！");
            }
            catch (Exception ex)
            {
                printLog(ex.ToString());
            }
        }

        private void debugXml(string message)
        {
            printLog(Path.GetFileName(FilePath) + ": " + message);
        }

        private void printLog(string message)
        {
            message += "\n";
            using (FileStream fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                fs.Write(buffer, 0, buffer.Length);
            }
            
            LoadLog();
        }

        private void load()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "XML文件|*.xml";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Title = "打开xml文件";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = openFileDialog.FileName;
            }

            try
            {
                using (StreamReader sr = new StreamReader(FilePath))
                {
                    Code = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                printLog(ex.ToString());
            }
        }

        private void save()
        {
            string path;

            if (FilePath == "")
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "保存xml";
                saveFileDialog.InitialDirectory = @"C:\";
                saveFileDialog.Filter = "XML文件|*.xml";
                saveFileDialog.ShowDialog();

                path = saveFileDialog.FileName;
            }
            else
            {
                path = FilePath;
            }
            
            if (path == "")
            {
                System.Windows.Forms.MessageBox.Show("文件名不可为空！");
            }

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(_code);
                    fs.Write(buffer, 0, buffer.Length);
                    debugXml("保存成功");
                }
            }
            catch (Exception ex)
            {
                printLog(ex.ToString());
            }
            
        }
    }
}
