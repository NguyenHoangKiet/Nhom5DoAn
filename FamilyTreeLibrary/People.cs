using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

namespace FamilyTreeLibrary
{
    public class ContentChangedEventArgs : EventArgs
    {
        private Person newPerson;

        public Person NewPerson
        {
            get { return newPerson; }
        }

        public ContentChangedEventArgs(Person newPerson)
        {
            this.newPerson = newPerson;
        }
    }

    [XmlRoot("Family")]
    [XmlInclude(typeof(ParentRelationship))]
    [XmlInclude(typeof(ChildRelationship))]
    [XmlInclude(typeof(SpouseRelationship))]
    [XmlInclude(typeof(SiblingRelationship))]
    public class People
    {
        #region fields and constants
        private static class Const
        {
            public const string DataFileName = "default.familytree";
        }
        private PeopleCollection peopleCollection;
        private string currentPersonId;
        private string currentPersonName;
        private string fullyQualifiedFilename;
        private string fileVersion = "1.0";

        private string OPCContentFileName = "content.xml";

        #endregion

        #region Properties
        public PeopleCollection PeopleCollection
        {
            get { return peopleCollection; }
        }

        [XmlAttribute(AttributeName = "Current")]
        public string CurrentPersonId
        {
            get { return currentPersonId; }
            set { currentPersonId = value; }
        }

        [XmlAttribute(AttributeName = "CurrentName")]
        public string CurrentPersonName
        {
            get { return currentPersonName; }
            set { currentPersonName = value; }
        }

        [XmlAttribute(AttributeName = "FileVersion")]
        public string Version
        {
            get { return fileVersion; }
            set { fileVersion = value; }
        }

        [XmlIgnore]
        public static string ApplicationFolderPath
        {
            get
            {
                return Path.Combine(
                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                  App.FolderName);
            }
        }

        [XmlIgnore]
        public static string DefaultFullyQualifiedFilename
        {
            get
            {
                string appLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    App.SubFolderName);
                if (!Directory.Exists(appLocation))
                {
                    Directory.CreateDirectory(appLocation);
                }

                return Path.Combine(appLocation, Const.DataFileName);
            }
        }
        [XmlIgnore]
        public string FullyQualifiedFilename
        {
            get { return fullyQualifiedFilename; }

            set { fullyQualifiedFilename = value; }
        }

        #endregion

        public People()
        {
            peopleCollection = new PeopleCollection();
        }

        #region Loading and saving
        public void Save()
        {
            if (PeopleCollection == null || PeopleCollection.Count == 0)
            {
                return;
            }
            CurrentPersonName = PeopleCollection.Current.FullName;
            CurrentPersonId = PeopleCollection.Current.Id;
            if (string.IsNullOrEmpty(FullyQualifiedFilename))
            {
                FullyQualifiedFilename = DefaultFullyQualifiedFilename;
            }
            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.FolderName);
            tempFolder = Path.Combine(tempFolder, App.SubFolderName);
            Directory.CreateDirectory(tempFolder);
            XmlSerializer xml = new XmlSerializer(typeof(People));
            using (Stream stream = new FileStream(Path.Combine(tempFolder, OPCContentFileName), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(stream, this);
            }
            OPCUtility.CreatePackage(FullyQualifiedFilename, tempFolder);

            PeopleCollection.IsDirty = false;
        }

        public void Save(string FQFilename)
        {
            fullyQualifiedFilename = FQFilename;
            Save();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void LoadVersion2()
        {
            PeopleCollection.Clear();

            try
            {
                if (string.IsNullOrEmpty(FullyQualifiedFilename))
                {
                    FullyQualifiedFilename = DefaultFullyQualifiedFilename;
                }

                XmlSerializer xml = new XmlSerializer(typeof(People));
                using (Stream stream = new FileStream(FullyQualifiedFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    People people = (People)xml.Deserialize(stream);
                    stream.Close();

                    foreach (Person person in people.PeopleCollection)
                    {
                        PeopleCollection.Add(person);
                    }
                    string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        App.FolderName);
                    tempFolder = Path.Combine(tempFolder, App.SubFolderName);
                    RecreateDirectory(tempFolder);

                    string photoFolder = Path.Combine(tempFolder, Photo.Const.PhotosFolderName);
                    RecreateDirectory(photoFolder);

                    string storyFolder = Path.Combine(tempFolder, Story.Const.StoriesFolderName);
                    RecreateDirectory(storyFolder);

                    foreach (Person person in PeopleCollection)
                    {
                        foreach (Relationship relationship in person.Relationships)
                        {
                            relationship.RelationTo = PeopleCollection.Find(relationship.PersonId);
                        }
                        foreach (Photo photo in person.Photos)
                        {
                            string photoOldPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.FolderName), photo.RelativePath);
                            if (File.Exists(photoOldPath))
                            {
                                string photoFile = Path.Combine(photoFolder, Path.GetFileName(photo.FullyQualifiedPath));
                                photoFile = photoFile.Replace(" ", "");
                                photo.RelativePath = photo.RelativePath.Replace(" ", "");
                                File.Copy(photoOldPath, photoFile, true);
                            }
                        }
                        if (person.Story != null)
                        {
                            string storyOldPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.FolderName), person.Story.RelativePath);
                            if (File.Exists(storyOldPath))
                            {
                                string storyFile = Path.Combine(storyFolder, Path.GetFileName(person.Story.AbsolutePath));
                                storyFile = ReplaceEncodedCharacters(storyFile);
                                person.Story.RelativePath = ReplaceEncodedCharacters(person.Story.RelativePath);

                                File.Copy(storyOldPath, storyFile, true);
                            }
                        }
                    }
                    CurrentPersonId = people.CurrentPersonId;
                    CurrentPersonName = people.CurrentPersonName;
                    PeopleCollection.Current = PeopleCollection.Find(CurrentPersonId);
                }

                PeopleCollection.IsDirty = false;
                return;
            }
            catch (Exception)
            {
                fullyQualifiedFilename = string.Empty;
            }
        }

        private static string ReplaceEncodedCharacters(string fileName)
        {
            fileName = fileName.Replace(" ", "");
            fileName = fileName.Replace("{", "");
            fileName = fileName.Replace("}", "");
            return fileName;
        }

        public static void RecreateDirectory(string folderToDelete)
        {
            try
            {
                if (Directory.Exists(folderToDelete))
                {
                    Directory.Delete(folderToDelete, true);
                }

                Directory.CreateDirectory(folderToDelete);
            }
            catch
            {
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void LoadOPC()
        {
            PeopleCollection.Clear();

            try
            {
                if (string.IsNullOrEmpty(FullyQualifiedFilename))
                {
                    FullyQualifiedFilename = DefaultFullyQualifiedFilename;
                }

                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.FolderName);

                tempFolder = Path.Combine(tempFolder, App.SubFolderName + @"\");

                OPCUtility.ExtractPackage(FullyQualifiedFilename, tempFolder);

                XmlSerializer xml = new XmlSerializer(typeof(People));
                using (Stream stream = new FileStream(tempFolder + OPCContentFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    People people = (People)xml.Deserialize(stream);
                    stream.Close();

                    foreach (Person person in people.PeopleCollection)
                    {
                        PeopleCollection.Add(person);
                    }
                    foreach (Person person in PeopleCollection)
                    {
                        foreach (Relationship relationqhip in person.Relationships)
                        {
                            relationqhip.RelationTo = PeopleCollection.Find(relationqhip.PersonId);
                        }
                    }
                    CurrentPersonId = people.CurrentPersonId;
                    CurrentPersonName = people.CurrentPersonName;
                    PeopleCollection.Current = PeopleCollection.Find(CurrentPersonId);
                }

                PeopleCollection.IsDirty = false;
                return;
            }
            catch
            {
                fullyQualifiedFilename = string.Empty;
            }
        }

        #endregion
    }
    [Serializable]
    public class PeopleCollection : ObservableCollection<Person>, INotifyPropertyChanged
    {
        public PeopleCollection() { }

        private Person current;
        private bool dirty;
        public Person Current
        {
            get { return current; }
            set
            {
                if (current != value)
                {
                    current = value;
                    OnPropertyChanged(nameof(Current));
                    OnCurrentChanged();
                }
            }
        }
        public bool IsDirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        public bool IsOldVersion { get; set; }
        public event EventHandler<ContentChangedEventArgs> ContentChanged;
        public void OnContentChanged()
        {
            dirty = true;
            if (ContentChanged != null)
            {
                ContentChanged(this, new ContentChangedEventArgs(null));
            }
        }
        public void OnContentChanged(Person newPerson)
        {
            dirty = true;
            if (ContentChanged != null)
            {
                ContentChanged(this, new ContentChangedEventArgs(newPerson));
            }
        }
        public event EventHandler CurrentChanged;
        protected void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(this, EventArgs.Empty);
            }
        }

        #region Add new people / relationships
        public void AddChild(Person person, Person child, ParentChildModifier parentChildType)
        {
            person.Relationships.Add(new ChildRelationship(child, parentChildType));
            child.Relationships.Add(new ParentRelationship(person, parentChildType));
            if (!Contains(child))
            {
                Add(child);
            }
        }
        public void AddSpouse(Person person, Person spouse, SpouseModifier spouseType)
        {
            person.Relationships.Add(new SpouseRelationship(spouse, spouseType));
            spouse.Relationships.Add(new SpouseRelationship(person, spouseType));
            if (!Contains(spouse))
            {
                Add(spouse);
            }
        }
        public void AddSibling(Person person, Person sibling)
        {
            person.Relationships.Add(new SiblingRelationship(sibling));
            sibling.Relationships.Add(new SiblingRelationship(person));
            if (!Contains(sibling))
            {
                Add(sibling);
            }
        }

        #endregion

        public Person Find(string id)
        {
            foreach (Person person in this)
            {
                if (person.Id == id)
                {
                    return person;
                }
            }

            return null;
        }

        #region INotifyPropertyChanged Members

        protected override event PropertyChangedEventHandler PropertyChanged;

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