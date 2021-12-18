using System.Collections.Generic;
using System.Globalization;

namespace FamilyTreeLibrary
{
    class GedcomIdMap
    {
        #region fields
        private Dictionary<string, string> map = new Dictionary<string, string>();
        private int nextId;

        #endregion
        public string Get(string guid)
        {
            if (map.ContainsKey(guid))
            {
                return map[guid];
            }
            string id = string.Format(CultureInfo.InvariantCulture, "I{0}", nextId++);
            map[guid] = id;
            return id;
        }
    }
}