using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FamilyTree.ViewModel;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for ControlBarUC.xaml
    /// </summary>
    public partial class ControlBarUC : UserControl, INotifyPropertyChanged
    {
        UserDataContext userDataContext;
        public static int curentlang = 1;// 0 là TV , 1 là TA 


        public ControlBarUC()
        {
            InitializeComponent();
            this.DataContext = userDataContext = new UserDataContext();
        }

        public class UserDataContext
        {
            public List<string> listLanguage { get; set; }
            public ControlBarViewModel ViewModel { get; set; }

            public int CurrentLanguagee { get; set; }

            public UserDataContext()
            {
                ViewModel = new ControlBarViewModel();
                listLanguage = new List<string>() { "Tiếng Việt", "English", "Japanese" };
                CurrentLanguagee = curentlang;
            }
        }

        private List<string> listLanguage;
        public List<string> ListLanguage
        {
            get { return listLanguage; }
            set
            {
                listLanguage = value;
                OnPropertyChanged(nameof(ListLanguage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo.SelectedItem == null)
                return;
            bool isEN = !combo.SelectedItem.ToString().Equals("English") ? false : true;
            bool isJapanese = !combo.SelectedItem.ToString().Equals("Japanese") ? false : true;
            if (isEN)
            {
                LanguageManager.SetLanguageDictionary(ELanguage.English);
            }
            else if (isJapanese)
            {
                LanguageManager.SetLanguageDictionary(ELanguage.Japanese);
            }
            else
            {
                LanguageManager.SetLanguageDictionary(ELanguage.VietNamese);
            }
            curentlang = combo.SelectedIndex;
            this.InitializeComponent();
        }
    }

    public static class LanguageManager
    {
        public static void SetLanguageDictionary(ELanguage lang)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case ELanguage.English:
                    dict.Source = new Uri("..\\Resource\\ResourceString.en-US.xaml", UriKind.Relative);
                    break;
                case ELanguage.VietNamese:
                    dict.Source = new Uri("..\\Resource\\ResourceString.vi-VN.xaml", UriKind.Relative);
                    break;
                case ELanguage.Japanese:
                    dict.Source = new Uri("..\\Resource\\ResourceString.ja-Japan.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resource\\ResourceString.vi-VN.xaml", UriKind.Relative);
                    break;
            }
            Cons.CurrentLanguage = lang;
            //Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
    public enum ELanguage
    {
        English,
        VietNamese,
        Japanese
    }

    public static class Cons
    {
        public static ELanguage CurrentLanguage = ELanguage.VietNamese;
    }

}
