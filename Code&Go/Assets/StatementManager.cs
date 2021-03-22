using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class StatementManager : MonoBehaviour
{
    // Temporal
    [SerializeField] private RectTransform contentRect;

    public Text titlePrefab;
    public Text paragraphPrefab;
    public Image imagePrefab;

    public void Load(TextAsset textAsset)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(textAsset.text);

        XmlNodeList elements = document.SelectSingleNode("statement").ChildNodes;

        foreach (XmlElement child in elements)
        {
            if (child.Name == "h1" || child.Name == "title")
            {
                AddTitle(child.InnerText);
            }
            else if (child.Name == "p" || child.Name == "paragraph")
            {
                AddParagraph(child.InnerText);
            }
            else if (child.Name == "i" || child.Name == "image")
            {
                AddImage(child);
            }
        }
    }

    private void AddTitle(string s)
    {
        Text title = Instantiate(titlePrefab, contentRect);
        title.gameObject.SetActive(true);
        title.text = s;
    }

    private void AddParagraph(string s)
    {
        Text paragraph = Instantiate(paragraphPrefab, contentRect);
        paragraph.gameObject.SetActive(true);
        paragraph.text = s;
    }

    private void AddImage(XmlElement e)
    {
        string src = e.GetAttribute("src");
        //int width = int.Parse(e.GetAttribute("width"));
        //int height = int.Parse(e.GetAttribute("height"));

        string path = Path.Combine(Application.dataPath, src);

        Texture2D texture = new Texture2D(1,1);

        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            texture.LoadImage(fileData);
        }
        else
        {
            Debug.LogError("Cannot load image " + path);
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));

        Image image = Instantiate(imagePrefab, contentRect);
        image.gameObject.SetActive(true);
        image.sprite = sprite;
    }
}
