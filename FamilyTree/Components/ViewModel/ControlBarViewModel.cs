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
using FamilyTree.Components;

namespace FamilyTree.ViewModel
{
    public class ControlBarViewModel : BaseViewModel
    {
        public ICommand closewindowCommand { get; set; }
        public ICommand maximizedwindowCommand { get; set; }
        public ICommand minimizedwindowCommand { get; set; }
        public ICommand dragwindowCommand { get; set; }
        public ControlBarViewModel()
        {
            closewindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;
                if (w != null)
                {
                    try
                    {
                        var mainWD = (MainWindow)w;
                        if (mainWD != null) Application.Current.Shutdown();
                    }
                    catch
                    {

                    }

                    try
                    {
                        var welcomeWD = (WelcomeWindow)w;
                        if (welcomeWD != null) Application.Current.Shutdown();
                    }
                    catch
                    {

                    }

                    w.Close();
                }
            });
            maximizedwindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;

                if (w != null)
                {
                    try
                    {
                        var userWD = (NewUser)w;
                        if (userWD != null) return;
                    }
                    catch
                    {

                    }

                    try
                    {
                        var welcomeWD = (WelcomeWindow)w;
                        if (welcomeWD != null) return;
                    }
                    catch
                    {

                    }

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
                    try
                    {
                        var userWD = (NewUser)w;
                        if (userWD != null) return;
                    }
                    catch
                    {

                    }

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
            dragwindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetFrameworkElement(p);
                var w = (Window)window;
                if (w != null)
                {
                    try
                    {
                        var newUser = (NewUser)w;
                        if (newUser != null) return;
                    }
                    catch
                    {

                    }

                    w.DragMove();
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
