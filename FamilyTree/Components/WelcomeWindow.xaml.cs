using System;
using System.Windows;
using FamilyTreeLibrary;
using System.Windows.Controls;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        People familyCollection = App.FamilyCollection;
        PeopleCollection family = App.Family;
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            //MainWindow mainwindow = new MainWindow();
            //mainwindow.Show();
            App.mainWindow.Show();
            NewUser newUser = new NewUser();
            newUser.ShowDialog();
            this.Close();
        }

        private void ControlBarUC_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportGedcom();
            App.mainWindow.DetailsControl.Visibility = Visibility.Visible;
            App.mainWindow.Tree.Visibility = Visibility.Visible;
            App.mainWindow.Show();
            this.Close();
        }

        public void ImportGedcom()
        {
            PromptToSave();

            Dialog dialog = new Dialog();
            dialog.InitialDirectory = People.ApplicationFolderPath;
            dialog.Filter.Add(new FilterEntry(Properties.Resources.GedcomFiles, Properties.Resources.GedcomExtension));
            dialog.Filter.Add(new FilterEntry(Properties.Resources.AllFiles, Properties.Resources.AllExtension));
            dialog.Title = Properties.Resources.Import;
            dialog.ShowOpen();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    GedcomImport ged = new GedcomImport();
                    ged.Import(family, dialog.FileName);
                    familyCollection.FullyQualifiedFilename = string.Empty;

                    family.IsDirty = false;
                }
                catch
                {
                    MessageBox.Show(this, Properties.Resources.GedcomFailedMessage,
                        Properties.Resources.GedcomFailed, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
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

        private void btnOpen_Click(object sender, RoutedEventArgs e)
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

            App.mainWindow.DetailsControl.Visibility = Visibility.Visible;
            App.mainWindow.Tree.Visibility = Visibility.Visible;
            App.mainWindow.Show();
            this.Close();
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

    }
}
