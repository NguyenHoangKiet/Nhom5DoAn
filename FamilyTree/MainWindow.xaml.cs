using FamilyTree.Components;
using FamilyTreeLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml;

namespace FamilyTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields

        People familyCollection = App.FamilyCollection;
        PeopleCollection family = App.Family;
        static string exportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private Properties.Settings appSettings = Properties.Settings.Default;

        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ThemeDefaul_Click(object sender, RoutedEventArgs e)
        {
            //ResourceDictionary dict = new ResourceDictionary();
            //dict.Source = new Uri("..\\Resource\\Theme_Defaul.xaml", UriKind.Relative);
            //Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void ThemeLight_Click(object sender, RoutedEventArgs e)
        {
            //ResourceDictionary dict = new ResourceDictionary();
            //dict.Source = new Uri("..\\Resource\\Theme_Light.xaml", UriKind.Relative);
            //Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            string text = "Xin chào!\n" +
                "Team phát triển phần mềm gia phả này bao gồm:\n" +
                "   -Trần Quốc Thắng\n" +
                "   -Nguyễn Hoàng Kiệt\n" +
                "   -Ngô Quang Vũ\n" +
                "   -Lê Minh Quân\n" +
                "   -Hồng Trường Vinh\n" +
                "\n Cảm ơn các bạn rất nhiều vì đã sử sụng phần mềm của chúng tôi.\n" +
                "\n\n Phiên bản 1.0.0";

            MessageBox.Show(text);

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // new
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewFamily();
        }

        private void NewFamily()
        {
            App.mainWindow.DetailsControl.Visibility = Visibility.Hidden;
            App.mainWindow.Tree.Visibility = Visibility.Hidden;
            PromptToSave();

            family.Clear();
            familyCollection.FullyQualifiedFilename = null;
            family.OnContentChanged();

            NewUser addUser = new NewUser();
            addUser.ShowDialog();
            family.IsDirty = false;
        }

        public void PromptToSave()
        {
            if (!family.IsDirty)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(Properties.Resources.NotSavedMessage, Properties.Resources.NotSaved, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Dialog dialog = new Dialog();
                dialog.InitialDirectory = People.ApplicationFolderPath;
                dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyFiles, Properties.Resources.FamilyV3Extension));
                dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
                dialog.Title = Properties.Resources.SaveAs;
                dialog.DefaultExtension = Properties.Resources.DefaultFamilyExtension;
                dialog.ShowSave();

                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    familyCollection.Save(dialog.FileName);
                }
            }
        }

        // open
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            PromptToSave();

            Dialog dialog = new Dialog();
            dialog.InitialDirectory = People.ApplicationFolderPath;
            dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyFiles, Properties.Resources.FamilyExtensions));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyV3Files, Properties.Resources.FamilyV3Extension));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyV2Files, Properties.Resources.FamilyV2Extension));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
            dialog.Title = Properties.Resources.Open;
            dialog.ShowOpen();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                LoadFamily(dialog.FileName);

                family.OnContentChanged();

                if (familyCollection.FullyQualifiedFilename.EndsWith(Properties.Resources.DefaultFamilyExtension))
                {
                    family.IsDirty = false;
                }
            }

        }

        private void LoadFamily(string fileName)
        {
            familyCollection.FullyQualifiedFilename = fileName;

            if (fileName.EndsWith(Properties.Resources.DefaultFamilyExtension))
            {
                familyCollection.LoadOPC();
            }
            else
            {
                family.IsOldVersion = true;
                familyCollection.LoadVersion2();
            }
        }

        // save
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(familyCollection.FullyQualifiedFilename) || family.IsOldVersion)
            {
                Dialog dialog = new Dialog();
                dialog.InitialDirectory = People.ApplicationFolderPath;
                dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyV3Files, Properties.Resources.FamilyV3Extension));
                dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
                dialog.Title = Properties.Resources.SaveAs;
                dialog.DefaultExtension = Properties.Resources.DefaultFamilyExtension;
                dialog.ShowSave();

                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    familyCollection.Save(dialog.FileName);
                }
            }
            else
            {
                familyCollection.Save();
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new Dialog();
            dialog.InitialDirectory = People.ApplicationFolderPath;
            dialog.Filter.Add(new FilterEntry(Properties.Resources.FamilyFiles, Properties.Resources.FamilyV3Extension));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
            dialog.Title = Properties.Resources.SaveAs;
            dialog.DefaultExtension = Properties.Resources.DefaultFamilyExtension;
            dialog.ShowSave();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                familyCollection.Save(dialog.FileName);
            }
        }

        // xuat gedcom
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new Dialog();
            dialog.InitialDirectory = People.ApplicationFolderPath;
            dialog.Filter.Add(new FilterEntry(Properties.Resources.GedcomFiles, Properties.Resources.GedcomExtension));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
            dialog.Title = Properties.Resources.Export;
            dialog.DefaultExtension = Properties.Resources.DefaultGedcomExtension;
            dialog.ShowSave();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                GedcomExport ged = new GedcomExport();
                ged.Export(family, dialog.FileName);
            }
        }

        // ged com la gi
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/GEDCOM");
        }
    }
}
