﻿using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;
using VegGridLayouter.UI.Models;

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
                Task.Run(() => LoadXmlToTreeView());
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

        private bool _canDisplay;

        public bool CanDisplay
        {
            get { return _canDisplay; }
            set {
                _canDisplay = value;
                RaisePropertyChanged(nameof(CanDisplay));
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set {
                _errorMessage = value;
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        private TreeViewItemModel _selectedItem;
        public TreeViewItemModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }


        private ObservableCollection<TreeViewItemModel> _treeItems;

        public ObservableCollection<TreeViewItemModel> TreeItems
        {
            get { return _treeItems; }
            set {
                _treeItems = value;
                RaisePropertyChanged(nameof(TreeItems));
            }
        }

        private VegGrid _internalGrid = new VegGrid();

        private void LoadXmlToTreeView()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this._code))
                {
                    CanDisplay = true;
                    ErrorMessage = "XML代码为空或无效";
                    return;
                }

                XDocument xdoc = XDocument.Parse(this._code);

                TreeItems = new ObservableCollection<TreeViewItemModel>(ParseXml(xdoc.Root));

                ErrorMessage = string.Empty;
                CanDisplay = false;
            }
            catch (Exception ex)
            {
                CanDisplay = true;
                ErrorMessage = ex.ToString();
            }
        }

        private List<TreeViewItemModel> ParseXml(XElement rootElement)
        {
            var items = new List<TreeViewItemModel>();



            foreach (var element in rootElement.Elements())
            {
                string header = "";
                switch (element.Name.LocalName) 
                {
                    case "RowDefinitions":
                        header = "行定义";
                        break;
                    case "ColumnDefinitions":
                        header = "列定义";
                        break;
                    case "RowDefinition":
                        header = "行";
                        break;
                    case "ColumnDefinition":
                        header = "列";
                        break;
                    case "Children":
                        header = "嵌套网格";
                        break;
                    case "VegGrid":
                        header = "网格";
                        break;
                    default:
                        break;
                }
                var item = new TreeViewItemModel(this)
                {
                    Header = header,
                    Name = element.Name.LocalName,
                    Type = element.Attribute("Type")?.Value,
                    Value = element.Attribute("Value")?.Value,
                };

                item.SetAttributes(element);

                if (element.Name.LocalName == "Children")
                {
                    item.IsGridRoot = true;
                }

                if (element.Name.LocalName == "VegGrid")
                {
                    item.IsGrid = true;
                }

                if (element.HasElements)
                {
                    item.Children = new ObservableCollection<TreeViewItemModel>(ParseXml(element));
                }

                items.Add(item);
            }
            return items;
        }

        public XDocument UpdateXml()
        {
            XElement rootElement = new XElement("VegGrid");
            foreach (var item in TreeItems)
            {
                rootElement.Add(SerializeTreeItem(item));
            }

            return new XDocument(rootElement);
        }

        private XElement SerializeTreeItem(TreeViewItemModel item)
        {
            XElement element = new XElement(item.Name);
            if (item.Attributes != null && item.Attributes.Count > 0)
            {
                foreach (var attribute in item.Attributes)
                {
                    element.SetAttributeValue(attribute.Type, attribute.Value);
                }
            }

            if (item.Children != null && item.Children.Count > 0)
            {
                XElement childrenElement = new XElement(item.Name);
                foreach (var child in item.Children)
                {
                    element.Add(SerializeTreeItem(child));
                }
                // element.Add(childrenElement);
            }

            return element;
        }

        public MainWindowViewModel()
        {
            GenerateCommand = new DelegateCommand(generate);
            LoadCommand = new DelegateCommand(load);
            SaveCommand = new DelegateCommand(save);

            

            LoadLog();
        }

        private string logFileName = $@"{Environment.GetEnvironmentVariable("AppData")}\Vegas Pro\layouter_log.txt";

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
            // FileStream fs = File.Open("test.xml", FileMode.Open);
            try
            {
                debugXml("解析XML...");

                _internalGrid = VegXmlDeserializer.DeserializeXmlString(this._code);

                int width = _internalGrid.CurProject.Video.Width;
                int height = _internalGrid.CurProject.Video.Height;

                _internalGrid.Generate();
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
