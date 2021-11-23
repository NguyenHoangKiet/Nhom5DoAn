using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace FamilyTreeLibrary
{
    [Serializable]
    public class Photo : INotifyPropertyChanged
    {
        #region Fields and Constants
        public static class Const
        {
            public const string PhotosFolderName = "Images";
        }

        private string relativePath;
        private bool isAvatar;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string FullyQualifiedPath
        {
            get
            {
                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    App.FolderName);
                tempFolder = Path.Combine(tempFolder, App.SubFolderName);

                return Path.Combine(tempFolder, relativePath);
            }
            set
            {
            }
        }
        public bool IsAvatar
        {
            get { return isAvatar; }
            set
            {
                if (isAvatar != value)
                {
                    isAvatar = value;
                    OnPropertyChanged(nameof(IsAvatar));
                }
            }
        }

        #endregion

        #region Constructors
        public Photo() { }
        public Photo(string photoPath)
        {
            if (!string.IsNullOrEmpty(photoPath))
            {
                relativePath = Copy(photoPath);
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return FullyQualifiedPath;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string Copy(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            string appLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.FolderName);
            appLocation = Path.Combine(appLocation, App.SubFolderName);
            string photoLocation = Path.Combine(appLocation, Const.PhotosFolderName);
            string photoFullPath = Path.Combine(photoLocation, fileInfo.Name);
            string photoRelLocation = Path.Combine(Const.PhotosFolderName, fileInfo.Name);
            if (!Directory.Exists(appLocation))
            {
                Directory.CreateDirectory(appLocation);
            }
            if (!Directory.Exists(photoLocation))
            {
                Directory.CreateDirectory(photoLocation);
            }
            try
            {
                fileInfo.CopyTo(photoFullPath, true);
            }
            catch
            {
            }

            return photoRelLocation;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Delete()
        {
            try
            {
                File.Delete(FullyQualifiedPath);
            }
            catch
            {
            }
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
    [Serializable]
    public class PhotoCollection : ObservableCollection<Photo>
    {
    }
}