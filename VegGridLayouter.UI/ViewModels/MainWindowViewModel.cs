using ICSharpCode.AvalonEdit.Folding;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ScriptPortal.Vegas;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using VegGridLayouter.Core;
using VegGridLayouter.Core.Attributes;
using VegGridLayouter.Parser;
using VegGridLayouter.UI.Events;

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

        private ObservableCollection<TreeViewItemViewModel> _treeItems;

        public ObservableCollection<TreeViewItemViewModel> TreeItems
        {
            get { return _treeItems; }
            set {
                _treeItems = value;
                RaisePropertyChanged(nameof(TreeItems));
            }
        }

        private TreeViewItemViewModel _selectedTreeItem;

        public TreeViewItemViewModel SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set
            {
                _selectedTreeItem = value;
                RaisePropertyChanged(nameof(SelectedTreeItem));
            }
        }

        // private VegGrid _internalGrid = new VegGrid();
        private enum treeViewUpdateStateType
        {
            updating,
            finish
        }

        private ObservableCollection<TreeViewItemViewModel> _root;

        public ObservableCollection<TreeViewItemViewModel> Root
        {
            get { return _root; }
            set
            {
                _root = value;
                RaisePropertyChanged(nameof(Root));
            }
        }


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

                var rootItems = ParseXml(xdoc.Root);
                TreeItems = new ObservableCollection<TreeViewItemViewModel>(rootItems);

                Root = new ObservableCollection<TreeViewItemViewModel>()
                {
                    new TreeViewItemViewModel(this, _aggregator)
                    {
                        Children = TreeItems,
                        Header = "网格",
                        Name = "VegGrid"
                    }
                };

                ErrorMessage = string.Empty;
                CanDisplay = false;
            }
            catch (Exception ex)
            {
                CanDisplay = true;
                ErrorMessage = ex.ToString();
            }
        }

        internal ObservableCollection<TreeViewItemViewModel> ParseXml(XElement rootElement, TreeViewItemViewModel parent = null)
        {
            var items = new ObservableCollection<TreeViewItemViewModel>();

            var elementName = rootElement.Name.LocalName;

            Assembly assembly = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "VegGridLayouter.Core");

            var typeInfo = assembly?
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == elementName);

            if (StaticVariable.CollectionDictionary.Contains(elementName))
            {
                foreach (var element in rootElement.Elements())
                {
                    var propertiesControlDescriptionAttributeInfo = assembly?.GetTypes()
                        .FirstOrDefault(t => t.Name == element.Name.LocalName)
                        .GetCustomAttribute<PropertiesControlDescriptionAttribute>();

                    var item = new TreeViewItemViewModel(this, _aggregator)
                    {
                        Name = element.Name.LocalName,
                        Parent = parent,
                        Header = propertiesControlDescriptionAttributeInfo != null ? propertiesControlDescriptionAttributeInfo.Name : element.Name.LocalName,
                    };

                    item.SetAttributes(element);

                    var childItems = ParseXml(element, item);
                    item.Children = new ObservableCollection<TreeViewItemViewModel>(childItems);
                    items.Add(item);
                }

                return items;
            }

            foreach (var property in typeInfo.GetProperties())
            {
                var elements = rootElement.Elements().ToList();

                var elementNames = elements
                                   .Select(element => element.Name);

                if (property.GetCustomAttribute<XmlIgnoreAttribute>() != null) continue;
                if (property.GetCustomAttribute<XmlAttributeAttribute>() != null) continue;

                TreeViewItemViewModel item = new TreeViewItemViewModel(this, _aggregator)
                {
                    Name = property.Name,
                    Parent = parent,
                };

                if (elementNames.Contains(property.Name))
                {
                    var element = elements.FirstOrDefault(e => e.Name.LocalName == property.Name);
                    var propertiesControlAttributeInfo = property.GetCustomAttribute<PropertiesControlDescriptionAttribute>();

                    item.Header = propertiesControlAttributeInfo != null ? propertiesControlAttributeInfo.Name : property.Name;
                    item.IsCollection = propertiesControlAttributeInfo != null ? propertiesControlAttributeInfo.IsCollection : false;

                    if (element.HasElements)
                    {
                        item.Children = new ObservableCollection<TreeViewItemViewModel>(ParseXml(element, item));
                    }
                }

                items.Add(item);
            }

            //foreach (var element in rootElement.Elements())
            //{
            //    string header = "";

            //    switch (element.Name.LocalName) 
            //    {
            //        case "RowDefinitions":
            //            header = "行定义";
            //            break;
            //        case "ColumnDefinitions":
            //            header = "列定义";
            //            break;
            //        case "RowDefinition":
            //            header = "行";
            //            break;
            //        case "ColumnDefinition":
            //            header = "列";
            //            break;
            //        case "Children":
            //            header = "嵌套网格";
            //            break;
            //        case "VegGrid":
            //            header = "网格";
            //            break;
            //        default:
            //            break;
            //    }
            //    var item = new TreeViewItemViewModel(this, _aggregator)
            //    {
            //        Header = header,
            //        Name = element.Name.LocalName,
            //        Parent = parent
            //    };

            //    item.SetAttributes(element);

            //    if (element.Name.LocalName == "Children"
            //        || element.Name.LocalName == "RowDefinitions"
            //        || element.Name.LocalName == "ColumnDefinitions")
            //    {
            //        item.IsCollection = true;
            //        item.IsNotDefaultSet = false;
            //    }

            //    if (element.Name.LocalName == "Children")
            //    {
            //        item.IsGridRoot = true;
            //    }

            //    if (element.Name.LocalName == "VegGrid"
            //        || element.Name.LocalName == "RowDefinition"
            //        || element.Name.LocalName == "ColumnDefinition")
            //    {
            //        item.IsChild = true;
            //    }

            //    if (element.HasElements)
            //    {
            //        item.Children = new ObservableCollection<TreeViewItemViewModel>(ParseXml(element, item));
            //    }



            //    items.Add(item);
            //}
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

        private XElement SerializeTreeItem(TreeViewItemViewModel item)
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

        private IEventAggregator _aggregator;

        public MainWindowViewModel()
        {

        }

        internal MainWindowViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            this._dialogService = dialogService;
            this._aggregator = eventAggregator;
        }

        public MainWindowViewModel(IEventAggregator eventAggregator, IDialogService dialogService, Vegas vegas)
        {
            this._vegas = vegas;
            this._dialogService = dialogService;
            
            GenerateCommand = new DelegateCommand(OnGenerate);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave);
            OpenAboutWindowCommand = new DelegateCommand(() => _dialogService.ShowDialog("About"));
            TreeViewItemSelectedCommand = new DelegateCommand<TreeViewItemViewModel>(OnTreeViewItemSelected);

            Code = "<VegGrid/>";

            this._aggregator = eventAggregator;

            SelectedTreeItem = new TreeViewItemViewModel(this, eventAggregator);

            _aggregator.GetEvent<UpdateXmlEvent>().Subscribe(UpdateXmlEventProcesser);
            _aggregator.GetEvent<LoadXmlToTreeViewEvent>().Subscribe(LoadXmlToTreeViewEventProcesser);

            LoadConfigFile();

            LoadLog();
        }

        private void OnTreeViewItemSelected(TreeViewItemViewModel item)
        {
            _aggregator.GetEvent<PropertiesLoadEvent>().Publish(new PropertiesLoadEventModel() { Item = item });
        }

        private void LoadXmlToTreeViewEventProcesser(LoadXmlToTreeViewEventModel obj)
        {
            LoadXmlToTreeView();
        }

        private void UpdateXmlEventProcesser(UpdateXmlEventModel obj)
        {
            //if (treeViewUpdateState == treeViewUpdateStateType.finish)
            Code = UpdateXml().ToString();
        }

        private string _logFileName = $@"{Environment.GetEnvironmentVariable("AppData")}\Vegas Pro\layouter_log.txt";

        private void LoadLog()
        {
            string filename = _logFileName;
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
                PrintLog(ex.ToString());
            }
        }

        private readonly Vegas _vegas;
        private readonly IDialogService _dialogService;

        public DelegateCommand GenerateCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand OpenAboutWindowCommand { get; set; }
        public DelegateCommand<TreeViewItemViewModel> TreeViewItemSelectedCommand { get; set; }



        public FoldingManager foldingManager { get; set; }
        public XmlFoldingStrategy foldingStrategy { get; set; }

        public string FilePath { get; set; } = "";

        private void OnGenerate()
        {
            // FileStream fs = File.Open("test.xml", FileMode.Open);
            VegElement element = new VegElement();

            try
            {
                DebugXml("解析XML...");

                element = VegXmlDeserializer.DeserializeXmlString(this._code);

                StaticVariable.CurrentDispatcher.Invoke(() =>
                {
                    using (var ub = new UndoBlock("Generate grids"))
                    {
                        element.Generate();
                    }
                });

                DebugXml("生成完毕！");
                
                
            }
            catch (Exception ex)
            {
                PrintLog(ex.ToString());
            }
        }

        private void DebugXml(string message)
        {
            PrintLog(Path.GetFileName(FilePath) + ": " + message);
        }

        private void PrintLog(string message)
        {
            message += "\n";
            using (FileStream fs = new FileStream(_logFileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                fs.Write(buffer, 0, buffer.Length);
            }
            
            LoadLog();
        }

        private void OnLoad()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "XML文件|*.xml";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Title = "打开xml文件";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = openFileDialog.FileName;
                OperateConfigFile oc = new OperateConfigFile();
                string file = $@"{StaticVariable.FilePath}\{StaticVariable.ConfigFileName}";
                oc.ConfigWrite("Xml", "InitialFilePath", FilePath, file);
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
                PrintLog(ex.ToString());
            }
        }

        private void OnSave()
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
                    DebugXml("保存成功");
                }
            }
            catch (Exception ex)
            {
                PrintLog(ex.ToString());
            }
            
        }

        private void LoadConfigFile()
        {
            string file = $@"{StaticVariable.FilePath}\{StaticVariable.ConfigFileName}";
            OperateConfigFile oc = new OperateConfigFile();
            FilePath = oc.ConfigRead("Xml", "InitialFilePath", file);
            if (!string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(FilePath))
                    {
                        Code = sr.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    PrintLog(ex.ToString());
                }
            }
        }
    }

    class OperateConfigFile
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        public void ConfigWrite(string section, string key, string value, string path)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        public string ConfigRead(string section, string key, string path)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }

        public void ConfigDelete(string FilePath)
        {
            File.Delete(FilePath);
        }


    }
}
