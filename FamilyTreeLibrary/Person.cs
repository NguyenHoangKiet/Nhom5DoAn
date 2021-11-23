using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FamilyTreeLibrary
{
    [Serializable]
    public class Person : INotifyPropertyChanged, IEquatable<Person>, IDataErrorInfo
    {
        #region Fields and Constants
        private static class Const
        {
            public const string DefaultFirstName = "Unknown";
        }
        private string id;
        private string firstName;
        private string lastName;
        private string middleName;
        private string nickName;
        private Gender gender;
        private DateTime? birthDate;
        private string birthPlace;
        private DateTime? deathDate;
        private string deathPlace;
        private bool isLiving;
        private PhotoCollection photos;
        private Story story;
        private RelationshipCollection relationships;
        private Contact contact;

        #endregion

        #region Properties
        [XmlAttribute]
        public string Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value; 
                    OnPropertyChanged(nameof(Id));
                }
            }
        }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(nameof(LastName));
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string MiddleName
        {
            get { return middleName; }
            set
            {
                if (middleName != value)
                {
                    middleName = value;
                    OnPropertyChanged(nameof(MiddleName));
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string Name
        {
            get
            {
                string name = "";
                if (!string.IsNullOrEmpty(firstName))
                {
                    name += firstName;
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    name += " " + lastName;
                }

                return name;
            }
        }
        public string FullName
        {
            get
            {
                string fullName = string.Empty;
                if (!string.IsNullOrEmpty(firstName))
                {
                    fullName += firstName;
                }

                if (!string.IsNullOrEmpty(middleName))
                {
                    fullName += " " + middleName;
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    fullName += " " + lastName;
                }

                return fullName;
            }
        }
        public string NickName
        {
            get { return nickName; }
            set
            {
                if (nickName != value)
                {
                    nickName = value;
                    OnPropertyChanged(nameof(NickName));
                }
            }
        }
        public Gender Gender
        {
            get { return gender; }
            set
            {
                if (gender != value)
                {
                    gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }
        public int? Age
        {
            get
            {
                if (BirthDate == null)
                {
                    return null;
                }
                DateTime startDate = BirthDate.Value;
                DateTime endDate = (IsLiving || DeathDate == null) ? DateTime.Now : DeathDate.Value;
                int age = endDate.Year - startDate.Year;
                if (endDate.Month < startDate.Month || (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
                {
                    age--;
                }

                return Math.Max(0, age);
            }
        }

        [XmlIgnore]
        public AgeGroup AgeGroup
        {
            get
            {
                AgeGroup ageGroup = AgeGroup.Unknown;

                if (Age.HasValue)
                {
                    if (Age >= 0 && Age < 20)
                    {
                        ageGroup = AgeGroup.Youth;
                    }
                    else if (Age >= 20 && Age < 40)
                    {
                        ageGroup = AgeGroup.Adult;
                    }
                    else if (Age >= 40 && Age < 65)
                    {
                        ageGroup = AgeGroup.MiddleAge;
                    }
                    else
                    {
                        ageGroup = AgeGroup.Senior;
                    }
                }

                return ageGroup;
            }
        }
        public string YearOfBirth
        {
            get
            {
                if (birthDate.HasValue)
                {
                    return birthDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    return "-";
                }
            }
        }

        public string YearOfDeath
        {
            get
            {
                if (deathDate.HasValue && !isLiving)
                {
                    return deathDate.Value.Year.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    return "-";
                }
            }
        }

        public DateTime? BirthDate
        {
            get { return birthDate; }
            set
            {
                if (birthDate == null || birthDate != value)
                {
                    birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                    OnPropertyChanged(nameof(Age));
                    OnPropertyChanged(nameof(AgeGroup));
                    OnPropertyChanged(nameof(YearOfBirth));
                    OnPropertyChanged(nameof(BirthMonthAndDay));
                    OnPropertyChanged(nameof(BirthDateAndPlace));
                }
            }
        }
        public string BirthPlace
        {
            get { return birthPlace; }
            set
            {
                if (birthPlace != value)
                {
                    birthPlace = value;
                    OnPropertyChanged(nameof(BirthPlace));
                    OnPropertyChanged(nameof(BirthDateAndPlace));
                }
            }
        }

        [XmlIgnore]
        public string BirthMonthAndDay
        {
            get
            {
                if (birthDate == null)
                {
                    return null;
                }
                else
                {
                    return birthDate.Value.ToString(DateTimeFormatInfo.CurrentInfo.MonthDayPattern, CultureInfo.CurrentCulture);
                }
            }
        }

        [XmlIgnore]
        public string BirthDateAndPlace
        {
            get
            {
                if (birthDate == null)
                {
                    return null;
                }
                else
                {
                    StringBuilder returnValue = new StringBuilder();
                    returnValue.Append("Born ");
                    returnValue.Append(birthDate.Value.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, CultureInfo.CurrentCulture));

                    if (!string.IsNullOrEmpty(birthPlace))
                    {
                        returnValue.Append(", ");
                        returnValue.Append(birthPlace);
                    }

                    return returnValue.ToString();
                }
            }
        }

        public DateTime? DeathDate
        {
            get { return deathDate; }
            set
            {
                if (deathDate == null || deathDate != value)
                {
                    deathDate = value;
                    OnPropertyChanged(nameof(DeathDate));
                    OnPropertyChanged(nameof(Age));
                    OnPropertyChanged(nameof(YearOfDeath));
                }
            }
        }
        public string DeathPlace
        {
            get { return deathPlace; }
            set
            {
                if (deathPlace != value)
                {
                    deathPlace = value;
                    OnPropertyChanged(nameof(DeathPlace));
                }
            }
        }
        public bool IsLiving
        {
            get { return isLiving; }
            set
            {
                if (isLiving != value)
                {
                    isLiving = value;
                    OnPropertyChanged(nameof(IsLiving));
                }
            }
        }
        public PhotoCollection Photos
        {
            get { return photos; }
        }
        public Contact Contact
        {
            get { return contact; }
            set
            {
                if (contact != value)
                {
                    contact = value;
                    OnPropertyChanged(nameof(Contact));
                }
            }
        }

        public Story Story
        {
            get { return story; }
            set
            {
                if (story != value)
                {
                    story = value;
                    OnPropertyChanged(nameof(Story));
                }
            }
        }

        [XmlIgnore, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string Avatar
        {
            get
            {
                string avatar = "";

                if (photos != null && photos.Count > 0)
                {
                    foreach (Photo photo in photos)
                    {
                        if (photo.IsAvatar)
                        {
                            return photo.FullyQualifiedPath;
                        }
                    }
                }

                return avatar;
            }
            set
            {
                OnPropertyChanged(nameof(Avatar));
                OnPropertyChanged(nameof(HasAvatar));
            }
        }

        [XmlIgnore]
        public bool IsDeletable
        {
            get
            {
                if (relationships.Count < 3)
                {
                    if (Spouses.Count == 2)
                    {
                        return false;
                    }
                    if (Parents.Count == 1 && Children.Count == 1)
                    {
                        return false;
                    }
                    if (Parents.Count == 1 && Spouses.Count == 1)
                    {
                        return false;
                    }
                    return true;
                }
                if (Children.Count > 0 && Parents.Count == 0 && Siblings.Count == 0 && Spouses.Count == 0)
                {
                    return true;
                }
                if (Siblings.Count > 0 && Parents.Count >= 0 && Spouses.Count == 0 && Children.Count == 0)
                {
                    return true;
                }
                return false;
            }
        }
        public RelationshipCollection Relationships
        {
            get { return relationships; }
        }

        [XmlIgnore]
        public Collection<Person> Spouses
        {
            get
            {
                Collection<Person> spouses = new Collection<Person>();
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.RelationshipType == RelationshipType.Spouse)
                    {
                        spouses.Add(relationship.RelationTo);
                    }
                }

                return spouses;
            }
        }

        [XmlIgnore]
        public Collection<Person> CurrentSpouses
        {
            get
            {
                Collection<Person> spouses = new Collection<Person>();
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.RelationshipType == RelationshipType.Spouse)
                    {
                        SpouseRelationship spouseRel = relationship as SpouseRelationship;

                        if (spouseRel != null && spouseRel.SpouseModifier == SpouseModifier.Current)
                        {
                            spouses.Add(relationship.RelationTo);
                        }
                    }
                }

                return spouses;
            }
        }

        [XmlIgnore]
        public Collection<Person> PreviousSpouses
        {
            get
            {
                Collection<Person> spouses = new Collection<Person>();
                foreach (Relationship rel in relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Spouse)
                    {
                        SpouseRelationship spouseRel = rel as SpouseRelationship;

                        if (spouseRel != null && spouseRel.SpouseModifier == SpouseModifier.Former)
                        {
                            spouses.Add(rel.RelationTo);
                        }
                    }
                }

                return spouses;
            }
        }

        [XmlIgnore]
        public List<SpouseRelationship> ListSpousesRelationShip
        {
            get
            {
                var lst = new List<SpouseRelationship>();
                foreach (Relationship rel in this.Relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Spouse)
                    {
                        SpouseRelationship spouseRel = ((SpouseRelationship)rel);
                        lst.Add(spouseRel);
                    }
                }

                return lst;
            }
        }

        [XmlIgnore]
        public Collection<Person> Children
        {
            get
            {
                Collection<Person> children = new Collection<Person>();
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.RelationshipType == RelationshipType.Child)
                    {
                        children.Add(relationship.RelationTo);
                    }
                }

                return children;
            }
        }

        [XmlIgnore]
        public Collection<Person> Parents
        {
            get
            {
                Collection<Person> parents = new Collection<Person>();
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.RelationshipType == RelationshipType.Parent)
                    {
                        parents.Add(relationship.RelationTo);
                    }
                }

                return parents;
            }
        }

        [XmlIgnore]
        public Collection<Person> Siblings
        {
            get
            {
                Collection<Person> siblings = new Collection<Person>();
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.RelationshipType == RelationshipType.Sibling)
                    {
                        siblings.Add(relationship.RelationTo);
                    }
                }

                return siblings;
            }
        }

        [XmlIgnore]
        public Collection<Person> HalfSiblings
        {
            get
            {
                Collection<Person> halfSiblings = new Collection<Person>();
                Collection<Person> siblings = Siblings;
                foreach (Person parent in Parents)
                {
                    foreach (Person child in parent.Children)
                    {
                        if (child != this && !siblings.Contains(child) && !halfSiblings.Contains(child))
                        {
                            halfSiblings.Add(child);
                        }
                    }
                }

                return halfSiblings;
            }
        }

        [XmlIgnore]
        public ParentSet ParentSet
        {
            get
            {
                if (Parents.Count == 2)
                {
                    ParentSet parentSet = new ParentSet(Parents[0], Parents[1]);
                    return parentSet;
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlIgnore]
        public ParentSetCollection PossibleParentSets
        {
            get
            {
                ParentSetCollection parentSets = new ParentSetCollection();

                foreach (Person parent in Parents)
                {
                    foreach (Person spouse in parent.Spouses)
                    {
                        ParentSet parentSet = new ParentSet(parent, spouse);
                        if (!parentSets.Contains(parentSet))
                        {
                            parentSets.Add(parentSet);
                        }
                    }
                }

                return parentSets;
            }
        }

        public Person Father
        {
            get
            {
                foreach (Person item in Parents)
                {
                    if (item.Gender == Gender.Male)
                        return item;
                }
                return null;
            }
        }

        public Person Mother
        {
            get
            {
                foreach (Person item in Parents)
                {
                    if (item.Gender == Gender.Female)
                        return item;
                }
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public bool HasParents
        {
            get
            {
                return (Parents.Count >= 2);
            }
            set
            {
                OnPropertyChanged(nameof(HasParents));
                OnPropertyChanged(nameof(PossibleParentSets));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public bool HasSpouse
        {
            get
            {
                return (Spouses.Count >= 1);
            }
            set
            {
                OnPropertyChanged(nameof(HasSpouse));
                OnPropertyChanged(nameof(Spouses));
            }
        }

        [XmlIgnore]
        public bool HasAvatar
        {
            get
            {
                if (photos != null && photos.Count > 0)
                {
                    foreach (Photo photo in photos)
                    {
                        if (photo.IsAvatar)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        [XmlIgnore]
        public string ParentRelationshipText
        {
            get
            {
                if (gender == Gender.Male)
                {
                    return "Son";
                }
                else
                {
                    return "Daughter";
                }
            }
        }

        [XmlIgnore]
        public string ParentsText
        {
            get
            {
                Collection<Person> parents = Parents;

                string parentsText = string.Empty;
                if (parents.Count > 0)
                {
                    parentsText = parents[0].Name;

                    if (parents.Count == 2)
                    {
                        parentsText += " and " + parents[1].Name;
                    }
                    else
                    {
                        for (int i = 1; i < parents.Count; i++)
                        {
                            if (i == parents.Count - 1)
                            {
                                parentsText += ", and " + parents[i].Name;
                            }
                            else
                            {
                                parentsText += ", " + parents[i].Name;
                            }
                        }
                    }
                }

                return parentsText;
            }
        }

        [XmlIgnore]
        public string SiblingRelationshipText
        {
            get
            {
                if (gender == Gender.Male)
                {
                    return "Brother";
                }
                else
                {
                    return "Sister";
                }
            }
        }

        [XmlIgnore]
        public string SiblingsText
        {
            get
            {
                Collection<Person> siblings = Siblings;

                string siblingsText = string.Empty;
                if (siblings.Count > 0)
                {
                    siblingsText = siblings[0].Name;

                    if (siblings.Count == 2)
                    {
                        siblingsText += " and " + siblings[1].Name;
                    }
                    else
                    {
                        for (int i = 1; i < siblings.Count; i++)
                        {
                            if (i == siblings.Count - 1)
                            {
                                siblingsText += ", and " + siblings[i].Name;
                            }
                            else
                            {
                                siblingsText += ", " + siblings[i].Name;
                            }
                        }
                    }
                }

                return siblingsText;
            }
        }

        [XmlIgnore]
        public string SpouseRelationshipText
        {
            get
            {
                if (gender == Gender.Male)
                {
                    return "Husband";
                }
                else
                {
                    return "Wife";
                }
            }
        }

        [XmlIgnore]
        public string SpousesText
        {
            get
            {
                Collection<Person> spouses = Spouses;

                string spousesText = string.Empty;
                if (spouses.Count > 0)
                {
                    spousesText = spouses[0].Name;

                    if (spouses.Count == 2)
                    {
                        spousesText += " and " + spouses[1].Name;
                    }
                    else
                    {
                        for (int i = 1; i < spouses.Count; i++)
                        {
                            if (i == spouses.Count - 1)
                            {
                                spousesText += ", and " + spouses[i].Name;
                            }
                            else
                            {
                                spousesText += ", " + spouses[i].Name;
                            }
                        }
                    }
                }

                return spousesText;
            }
        }

        [XmlIgnore]
        public string ChildRelationshipText
        {
            get
            {
                if (gender == Gender.Male)
                {
                    return "Father";
                }
                else
                {
                    return "Mother";
                }
            }
        }

        [XmlIgnore]
        public string ChildrenText
        {
            get
            {
                Collection<Person> children = Children;

                string childrenText = string.Empty;
                if (children.Count > 0)
                {
                    childrenText = children[0].Name;

                    if (children.Count == 2)
                    {
                        childrenText += " and " + children[1].Name;
                    }
                    else
                    {
                        for (int i = 1; i < children.Count; i++)
                        {
                            if (i == children.Count - 1)
                            {
                                childrenText += ", and " + children[i].Name;
                            }
                            else
                            {
                                childrenText += ", " + children[i].Name;
                            }
                        }
                    }
                }

                return childrenText;
            }
        }
        #endregion

        #region Constructors
        public Person()
        {
            id = Guid.NewGuid().ToString();
            relationships = new RelationshipCollection();
            photos = new PhotoCollection();
            firstName = Const.DefaultFirstName;
            isLiving = true;
        }

        public Person(string firstName, string lastName) : this()
        {
            if (!string.IsNullOrEmpty(firstName))
            {
                this.firstName = firstName;
            }

            this.lastName = lastName;
        }

        public Person(string firstName, string lastName, Gender gender) : this(firstName, lastName)
        {
            this.gender = gender;
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

        #region IEquatable Members
        public bool Equals(Person other)
        {
            return (Id == other.Id);
        }

        #endregion

        #region Methods
        public SpouseRelationship GetSpouseRelationship(Person spouse)
        {
            foreach (Relationship relationship in relationships)
            {
                SpouseRelationship spouseRelationship = relationship as SpouseRelationship;
                if (spouseRelationship != null)
                {
                    if (spouseRelationship.RelationTo.Equals(spouse))
                    {
                        return spouseRelationship;
                    }
                }
            }

            return null;
        }
        public ParentSetCollection MakeParentSets()
        {
            ParentSetCollection parentSets = new ParentSetCollection();

            foreach (Person spouse in Spouses)
            {
                ParentSet ps = new ParentSet(this, spouse);
                if (!parentSets.Contains(ps))
                {
                    parentSets.Add(ps);
                }
            }

            return parentSets;
        }
        public void DeletePhotos()
        {
            foreach (Photo photo in photos)
            {
                photo.Delete();
            }
        }
        public void DeleteStory()
        {
            if (story != null)
            {
                story.Delete();
                story = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "BirthDate")
                {
                    if (BirthDate == DateTime.MinValue)
                    {
                        result = "This does not appear to be a valid date.";
                    }
                }

                if (columnName == "DeathDate")
                {
                    if (DeathDate == DateTime.MinValue)
                    {
                        result = "This does not appear to be a valid date.";
                    }
                }

                return result;
            }
        }

        #endregion
    }
}