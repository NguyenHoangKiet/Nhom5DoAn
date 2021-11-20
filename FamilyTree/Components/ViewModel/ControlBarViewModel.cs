using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyTree.ViewModel
{
    public class ControlBarViewModel : BaseViewModel
    {
        public ICommand closewindowCommand { get; set; }
        public ICommand maximizedwindowCommand { get; set; }
        public ICommand minimizedwindowCommand { get; set; }
        public ControlBarViewModel()
        {
            closewindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;
                if (w != null)
                {
                    w.Close();
                }
            });
            maximizedwindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Maximized)
                    {
                        w.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        w.WindowState = WindowState.Normal;
                    }
                }
            });
            minimizedwindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                    {
                        w.WindowState = WindowState.Minimized;
                    }
                    else
                    {
                        w.WindowState = WindowState.Normal;
                    }
                }
            });

        }
        FrameworkElement GetFrameworkElement(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
