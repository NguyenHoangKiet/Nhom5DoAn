using System.Collections.Generic;

namespace FamilyTreeLibrary
{
    class Family
    {
        #region fields

        private Person parentLeft;
        private Person parentRight;
        private SpouseRelationship relationship;
        private List<Person> children = new List<Person>();

        #endregion
        public Person ParentLeft
        {
            get { return parentLeft; }
        }
        public Person ParentRight
        {
            get { return parentRight; }
        }
        public SpouseRelationship Relationship
        {
            get { return relationship; }
            set { relationship = value; }
        }
        public List<Person> Children
        {
            get { return children; }
        }

        public Family(Person parentLeft, Person parentRight)
        {
            this.parentLeft = parentLeft;
            this.parentRight = parentRight;
        }
    }
}