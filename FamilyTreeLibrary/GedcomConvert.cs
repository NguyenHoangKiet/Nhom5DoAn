using System.IO;
using System.Text;
using System.Xml;

namespace FamilyTreeLibrary
{
    static class GedcomConvert
    {
        static public void ConvertToXml(string gedcomFilePath,
            string xmlFilePath, bool combineSplitValues)
        {
            int prevLevel = -1;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(xmlFilePath, settings))
            {
                writer.WriteStartElement("root");
                using (StreamReader sr = new StreamReader(gedcomFilePath))
                {
                    string text;
                    GedcomLine line = new GedcomLine();
                    while ((text = sr.ReadLine()) != null)
                    {
                        text = text.Trim();
                        if (line.Parse(text))
                        {
                            if (line.Level <= prevLevel)
                            {
                                int count = prevLevel - line.Level + 1;
                                for (int i = 0; i < count; i++)
                                {
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteStartElement(line.Tag);
                            writer.WriteAttributeString("Value", line.Data);

                            prevLevel = line.Level;
                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();

                if (combineSplitValues)
                {
                    CombineSplitValues(xmlFilePath);
                }
            }
        }
        static private void CombineSplitValues(string xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);  
            XmlNodeList list = doc.SelectNodes("//CONT/.. | ");
            foreach (XmlNode node in list)
            {
                AppendValues(node);
            }
            doc.Save(xmlFilePath);
        }
        static private void AppendValues(XmlNode node)
        {
            StringBuilder sb = new StringBuilder(node.Attributes["Value"].Value);
            XmlNodeList list = node.SelectNodes("CONT | CONC");
            foreach (XmlNode childNode in list)
            {
                switch (childNode.Name)
                {
                    case "CONC":
                        sb.Append(childNode.Attributes["Value"].Value);
                        break;
                    case "CONT":
                        sb.AppendFormat("\r{0}", childNode.Attributes["Value"].Value);
                        break;
                }
                node.RemoveChild(childNode);
            }
            node.Attributes["Value"].Value = sb.ToString();
        }
    }
}