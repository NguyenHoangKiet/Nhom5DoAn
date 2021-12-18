using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary
{
    class GedcomLine
    {
        #region fields
        private int level;
        private string tag;
        private string data;
        private readonly Regex regexToSplit = new Regex(
            @"(?<level>\d+)\s+(?<tag>[\S]+)(\s+(?<data>.+))?");
        private readonly Regex regexToClean = new Regex(@"[^\x20-\x7e]");
        private readonly Regex regexForTag = new Regex(@"[^\w.-]");

        #endregion

        #region properties
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool Parse(string text)
        {
            try
            {
                Clear();
                if (string.IsNullOrEmpty(text))
                {
                    return false;
                }
                text = regexToClean.Replace(text, "");
                Match match = regexToSplit.Match(text);
                level = Convert.ToInt32(match.Groups["level"].Value, CultureInfo.InvariantCulture);
                tag = match.Groups["tag"].Value.Trim();
                data = match.Groups["data"].Value.Trim();
                if (tag[0] == '@')
                {
                    string temp = tag;
                    tag = data;
                    data = temp;
                    int pos = tag.IndexOf(' ');
                    if (pos != -1)
                    {
                        tag = tag.Substring(0, pos);
                    }
                }
                tag = regexForTag.Replace(tag, "");

                return true;
            }
            catch
            {
                Clear();
                return false;
            }
        }
        private void Clear()
        {
            Level = 0;
            Tag = "";
            Data = "";
        }
    }
}