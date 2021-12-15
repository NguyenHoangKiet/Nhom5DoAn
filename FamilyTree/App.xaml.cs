using FamilyTree.Components;
using FamilyTreeLibrary;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FamilyTree
{
    public partial class App : Application
    {
        #region fields

        internal const string ApplicationFolderName = "Family.Tree";

        // Class people chua co 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static People FamilyCollection = new People();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static PeopleCollection Family = FamilyCollection.PeopleCollection;

        private const int NumberOfRecentFiles = 10;

        private readonly static string RecentFilesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine(ApplicationFolderName, "RecentFiles.xml"));

        private static StringCollection recentFiles = new StringCollection();

        #endregion

        #region overrides

        // load khi chuong trinh vua bat dau
        // tao. 2 theme trong skin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnStartup(StartupEventArgs e)
        {
            InstallSampleFiles();

            LoadRecentFiles();

            //Properties.Settings appSettings = FamilyTree.Properties.Settings.Default;

            //if (!string.IsNullOrEmpty(appSettings.Skin))
            //{
            //    try
            //    {
            //        ResourceDictionary resourceDictionary = new ResourceDictionary();
            //        resourceDictionary.MergedDictionaries.Add(LoadComponent(new Uri(appSettings.Skin, UriKind.Relative)) as ResourceDictionary);
            //        Current.Resources = resourceDictionary;
            //    }
            //    catch
            //    {
            //    }
            //}

            WelcomeWindow w = new WelcomeWindow();
            w.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveRecentFiles();
            base.OnExit(e);
        }
        #endregion

        #region methods

        public static StringCollection RecentFiles
        {
            get { return recentFiles; }
        }

        public static NameValueCollection Skins
        {
            get
            {
                NameValueCollection skins = new NameValueCollection();

                foreach (string folder in Directory.GetDirectories(FamilyTree.Properties.Resources.Skins))
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (string.Compare(fileInfo.Extension, FamilyTree.Properties.Resources.XamlExtension, true, CultureInfo.InvariantCulture) == 0)
                        {
                            skins.Add(fileInfo.Name.Remove(fileInfo.Name.IndexOf(FamilyTree.Properties.Resources.ResourcesString)), Path.Combine(folder, fileInfo.Name));
                        }
                    }
                }

                return skins;
            }
        }

        public static TimeSpan GetAnimationDuration(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(Keyboard.IsKeyDown(Key.F12) ? milliseconds * 5 : milliseconds);
        }

        public static void LoadRecentFiles()
        {
            if (File.Exists(RecentFilesFilePath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(StringCollection));
                using (TextReader reader = new StreamReader(RecentFilesFilePath))
                {
                    recentFiles = (StringCollection)ser.Deserialize(reader);
                }

                for (int i = 0; i < recentFiles.Count; i++)
                {
                    if (!File.Exists(recentFiles[i]))
                    {
                        recentFiles.RemoveAt(i);
                    }
                }

                while (recentFiles.Count > NumberOfRecentFiles)
                {
                    recentFiles.RemoveAt(NumberOfRecentFiles);
                }
            }
        }

        public static void SaveRecentFiles()
        {
            XmlSerializer ser = new XmlSerializer(typeof(StringCollection));
            using (TextWriter writer = new StreamWriter(RecentFilesFilePath))
            {
                ser.Serialize(writer, recentFiles);
            }
        }

        private static void InstallSampleFiles()
        {
            // ...
        }

        private static void CreateSampleFile(string location, string fileName, byte[] fileContent)
        {
            string path = Path.Combine(location, fileName);

            if (File.Exists(path))
            {
                return;
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(fileContent);
            }
        }

        private static void CreateSampleFile(string location, string fileName, string fileContent)
        {
            string path = Path.Combine(location, fileName);

            if (File.Exists(path))
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(fileContent);
            }
        }

        private static void CreateSampleFile(string location, string fileName, Bitmap image)
        {
            string path = Path.Combine(location, fileName);

            if (File.Exists(path))
            {
                return;
            }

            image.Save(path);
        }

        internal static DateTime StringToDate(string dateString)
        {
            if (dateString.Length == 4)
            {
                dateString = "1/1/" + dateString;
            }

            DateTime.TryParse(dateString, out DateTime date);

            return date;
        }

        #endregion
    }
}