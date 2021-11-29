using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FamilyTreeLibrary
{
    #region Relationship classes

    [Serializable]
    public abstract class Relationship
    {
        private RelationshipType relationshipType;

        private Person relationTo;
        private string personId;
        private string personFullname;

        public RelationshipType RelationshipType
        {
            get { return relationshipType; }
            set { relationshipType = value; }
        }

        [XmlIgnore]
        public Person RelationTo
        {
            get { return relationTo; }
            set
            {
                relationTo = value;
                personId = ((Person)value).Id;
                personFullname = ((Person)value).FullName;
            }
        }

        public string PersonId
        {
            get { return personId; }
            set { personId = value; }
        }

        public string PersonFullname
        {
            get { return personFullname; }
            set { personFullname = value; }
        }
    }

    [Serializable]
    public class ParentRelationship : Relationship
    {
        private ParentChildModifier parentChildModifier;
        public ParentChildModifier ParentChildModifier
        {
            get { return parentChildModifier; }
            set { parentChildModifier = value; }
        }
        public ParentRelationship() { }

        public ParentRelationship(Person personId, ParentChildModifier parentChildType)
        {
            RelationshipType = RelationshipType.Parent;
            RelationTo = personId;
            parentChildModifier = parentChildType;
        }

        public override string ToString()
        {
            return RelationTo.Name;
        }
    }

    [Serializable]
    public class ChildRelationship : Relationship
    {
        private ParentChildModifier parentChildModifier;
        public ParentChildModifier ParentChildModifier
        {
            get { return parentChildModifier; }
            set { parentChildModifier = value; }
        }
        public ChildRelationship() { }

        public ChildRelationship(Person person, ParentChildModifier parentChildType)
        {
            RelationshipType = RelationshipType.Child;
            RelationTo = person;
            parentChildModifier = parentChildType;
        }
    }

    [Serializable]
    public class SpouseRelationship : Relationship
    {
        private SpouseModifier spouseModifier;
        private DateTime? marriageDate;
        private DateTime? divorceDate;
        private String marriagePlace;

        public SpouseModifier SpouseModifier
        {
            get { return spouseModifier; }
            set { spouseModifier = value; }
        }

        public DateTime? MarriageDate
        {
            get { return marriageDate; }
            set { marriageDate = value; }
        }

        public DateTime? DivorceDate
        {
            get { return divorceDate; }
            set { divorceDate = value; }
        }
        public String MarriagePlace
        {
            get { return marriagePlace; }
            set { marriagePlace = value; }
        }
        public SpouseRelationship() { }

        public SpouseRelationship(Person person, SpouseModifier spouseType)
        {
            RelationshipType = RelationshipType.Spouse;
            spouseModifier = spouseType;
            RelationTo = person;
        }
    }

    [Serializable]
    public class SiblingRelationship : Relationship
    {
        public SiblingRelationship() { }

        public SiblingRelationship(Person person)
        {
            RelationshipType = RelationshipType.Sibling;
            RelationTo = person;
        }
    }

    #endregion

    #region Relationships collection
    [Serializable]
    public class RelationshipCollection : ObservableCollection<Relationship> { }

    #endregion

    #region Relationship Type/Modifier Enums
    public enum RelationshipType
    {
        Child,
        Parent,
        Sibling,
        Spouse
    }

    public enum SpouseModifier
    {
        Current,
        Former
    }

    public enum ParentChildModifier
    {
        Natural,
        Adopted,
        Foster
    }

    #endregion
}