using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class StatementManager : MonoBehaviour
{
    // Temporal
    public Text title;
    public Text paragraph;

    public void Load(TextAsset textAsset)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(textAsset.text);

        XmlNodeList elements = document.SelectSingleNode("statement").ChildNodes;

        foreach (XmlElement child in elements)
        {
            if (child.Name == "title")
            {
                SetTitle(child.InnerText);
            }
            else if (child.Name == "p")
            {
                SetParagraph(child.InnerText);
            }
        }
    }

    // Maybe do an AddTitle, AddSubTitle, etc
    private void SetTitle(string s)
    {
        title.text = s;
    }

    private void SetParagraph(string s)
    {
        paragraph.text = s;
    }
}
