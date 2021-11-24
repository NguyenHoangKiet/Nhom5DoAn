using System;
using System.Collections.ObjectModel;

namespace FamilyTreeLibrary
{
    public class ParentSet : IEquatable<ParentSet>
    {
        private Person firstParent;

        private Person secondParent;

        public Person FirstParent
        {
            get { return firstParent; }
            set { firstParent = value; }
        }

        public Person SecondParent
        {
            get { return secondParent; }
            set { secondParent = value; }
        }

        public ParentSet(Person firstParent, Person secondParent)
        {
            this.firstParent = firstParent;
            this.secondParent = secondParent;
        }

        public string Name
        {
            get
            {
                string name = string.Empty;
                name += $"{firstParent.Name} + {secondParent.Name}";
                return name;
            }
        }
        public ParentSet() { }

        #region IEquatable<ParentSet> Members
        public bool Equals(ParentSet other)
        {
            if (other != null)
            {
                if (firstParent.Equals(other.firstParent) && secondParent.Equals(other.secondParent))
                {
                    return true;
                }

                if (firstParent.Equals(other.secondParent) && secondParent.Equals(other.firstParent))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    public class ParentSetCollection : Collection<ParentSet> { }
}