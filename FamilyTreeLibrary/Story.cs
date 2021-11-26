using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace FamilyTreeLibrary
{
    [Serializable]
    public class Story : INotifyPropertyChanged
    {
        #region Fields and Constants
        public static class Const
        {
            public const string StoriesFolderName = "Stories";
        }

        private string relativePath;

        #endregion

        #region Properties
        public string RelativePath
        {
            get { return relativePath; }
            set
            {
                if (relativePath != value)
                {
                    relativePath = value;
                    OnPropertyChanged(nameof(relativePath));
                }
            }
        }
        [XmlIgnore]
        public string AbsolutePath
        {
            get
            {
                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    App.FolderName);
                tempFolder = Path.Combine(tempFolder, App.SubFolderName);

                if (relativePath != null)
                {
                    return Path.Combine(tempFolder, relativePath);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion

        #region Constructors
        public Story() { }

        #endregion

        #region Methods
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.FamilyShowLib.Story.Save(System.String,System.String):System.Void")]
        public void Save(TextRange storyText, string storyFileName)
        {
            string dataFormat = DataFormats.Rtf;

            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.FolderName);
            tempFolder = Path.Combine(tempFolder, App.SubFolderName);
            string storiesLocation = Path.Combine(tempFolder, Const.StoriesFolderName);
            storyFileName = GetSafeFileName(storyFileName);
            string storyAbsolutePath = Path.Combine(storiesLocation, storyFileName);

            try
            {
                if (!Directory.Exists(storiesLocation))
                    Directory.CreateDirectory(storiesLocation);
                using (FileStream stream = File.Create(storyAbsolutePath))
                {
                    relativePath = Path.Combine(Const.StoriesFolderName, storyFileName);
                    if (storyText.CanSave(dataFormat))
                    {
                        storyText.Save(stream, dataFormat);
                    }
                }
            }
            catch
            {
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Load(TextRange storyText)
        {
            string dataFormat = DataFormats.Rtf;

            if (File.Exists(AbsolutePath))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(AbsolutePath))
                    {
                        if (storyText.CanLoad(dataFormat))
                        {
                            storyText.Load(stream, dataFormat);
                        }
                    }
                }
                catch
                {
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.FamilyShowLib.Story.Save(System.String,System.String):System.Void")]
        public void Save(string storyText, string storyFileName)
        {
            string dataFormat = DataFormats.Rtf;

            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.FolderName);
            tempFolder = Path.Combine(tempFolder, App.SubFolderName);
            string storiesLocation = Path.Combine(tempFolder, Const.StoriesFolderName);
            storyFileName = GetSafeFileName(storyFileName);
            string storyAbsolutePath = Path.Combine(storiesLocation, storyFileName);
            TextBlock block = new TextBlock();
            block.Text = storyText;
            TextRange range = new TextRange(block.ContentStart, block.ContentEnd);

            try
            {
                if (!Directory.Exists(storiesLocation))
                {
                    Directory.CreateDirectory(storiesLocation);
                }
                using (FileStream stream = File.Create(storyAbsolutePath))
                {
                    relativePath = Path.Combine(Const.StoriesFolderName, storyFileName);
                    if (range.CanSave(dataFormat))
                    {
                        range.Save(stream, dataFormat);
                    }
                }
            }
            catch
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Delete()
        {
            if (File.Exists(AbsolutePath))
            {
                try
                {
                    File.Delete(AbsolutePath);
                }
                catch
                {
                }
            }
        }
        private static string GetSafeFileName(string fileName)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            int pos;
            while ((pos = fileName.IndexOfAny(invalid)) != -1)
            {
                fileName = fileName.Remove(pos, 1);
            }

            return fileName;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}