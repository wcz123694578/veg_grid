using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VegGridLayouter.UI.ViewModels
{
    public static class WindowsManager
    {
        private static Hashtable _registerWindow = new Hashtable();

        public static void Register<T>(string key)
        {
            if (!_registerWindow.Contains(key))
            {
                _registerWindow.Add(key, typeof(T));
            }
        }

        public static void Register(string key, Type T)
        {
            if (!_registerWindow.Contains(key))
            {
                _registerWindow.Add(key, T);
            }
        }

        public static void Remove(string key)
        {
            if (_registerWindow.ContainsKey(key))
            {
                _registerWindow.Remove(key);
            }
        }

        public static void Show(string key, object VM)
        {
            if (_registerWindow.ContainsKey(key))
            {
                var win = (Window)Activator.CreateInstance((Type)_registerWindow[key]);
                win.DataContext = VM;
                win.ShowDialog();
            }
        }
    }
}
