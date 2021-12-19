using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public static int curentlang = 0;// 0 là TV , 1 là TA 


        public ControlBarUC()
        {
            InitializeComponent(); LoadStaticSource();
            this.DataContext = userDataContext = new UserDataContext();
        }

        public class UserDataContext
        {
            public List<string> listLanguage { get; set; }
            public ControlBarViewModel ViewModel { get; set; }
            public int CurrentLanguagee { get; set; }
            public ICommand closewindowCommand { get; set; }
            public ICommand maximizedwindowCommand { get; set; }
            public ICommand minimizedwindowCommand { get; set; }
            public ICommand dragwindowCommand { get; set; }

            public UserDataContext()
            {
                ViewModel = new ControlBarViewModel();
                listLanguage = new List<string>() { "Tiếng Việt", "English", "Japanese" };
                CurrentLanguagee = curentlang;
                dragwindowCommand = ViewModel.dragwindowCommand;
                minimizedwindowCommand = ViewModel.minimizedwindowCommand;
                maximizedwindowCommand = ViewModel.maximizedwindowCommand;
                closewindowCommand=ViewModel.closewindowCommand;
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
            SetStaticSource();
            this.InitializeComponent();
        }

        string fileName = "StaticSource.txt";
        void SetStaticSource()
        {
            try
            {
                List<string> lines = new List<string>();
                int i = 0;
                foreach (string line in System.IO.File.ReadLines(fileName))
                {
                    if (line.StartsWith("LANGUAGE"))
                    {
                        lines.Add("LANGUAGE" + curentlang.ToString());
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }

                var st = new FileStream(fileName, FileMode.Create);
                st.Close();

                foreach (var line in lines)
                {
                    TextWriter tw = new StreamWriter(fileName, true);

                    tw.WriteLine(line);

                    tw.Close();
                }
            }
            catch
            {
                using (var st = new FileStream(fileName, FileMode.Create))
                {
                    st.Close();
                    TextWriter tw = new StreamWriter(fileName, true);

                    tw.WriteLine("LANGUAGE" + curentlang.ToString());

                    tw.Close();
                }
            }
            
        }

        void LoadStaticSource()
        {
            try
            {
                foreach (string line in System.IO.File.ReadLines(fileName))
                {
                    if (line.StartsWith("LANGUAGE"))
                    {
                        string[] arrListStr = line.Split('E');

                        string input = arrListStr[1];
                        try
                        {
                            int result = Int32.Parse(input);
                            curentlang = result;

                            if (curentlang == 1)
                            {
                                LanguageManager.SetLanguageDictionary(ELanguage.English); return;
                            }
                            else if (curentlang == 2)
                            {
                                LanguageManager.SetLanguageDictionary(ELanguage.Japanese); return;
                            }
                            else
                            {
                                LanguageManager.SetLanguageDictionary(ELanguage.VietNamese); return;
                            }
                        }
                        catch (FormatException)
                        {

                        }
                        this.InitializeComponent();
                    }
                }
            }
            catch
            {

            }
            
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
