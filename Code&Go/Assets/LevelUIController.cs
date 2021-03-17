using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private RectTransform temaryPanel;
    [SerializeField] private RectTransform sentencePanel;

    void Start()
    {
        temaryPanel.gameObject.SetActive(false);
        sentencePanel.gameObject.SetActive(false);
    }


    public void TriggerPanel(RectTransform panel)
    {
        RectTransform other = panel != temaryPanel ? temaryPanel : sentencePanel;

        if(panel.gameObject.activeSelf)
        {
            ClosePanel(panel);
        }
        else
        {
            OpenPanel(panel);
            ClosePanel(other);
        }
    }

    public void OpenPanel(RectTransform panel)
    {
        panel.gameObject.SetActive(true);
    }
    
    public void ClosePanel(RectTransform panel)
    {
        panel.gameObject.SetActive(false);
    }

}
