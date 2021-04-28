using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemaryManager : MonoBehaviour
{
    private static Dictionary<TutorialType, List<PopUpData>> shownTemary;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Category currentCategory;

    private static TutorialType lastTutorialType;
    private static PopUpData lastData;

    [Space]
    [SerializeField] private RectTransform categoryList;
    [SerializeField] private Button categoryButton;
    [Space]
    [SerializeField] private Text titlePrefab;
    [SerializeField] private Text paragraphPrefab;
    [SerializeField] private RectTransform contentRect;

    private void Awake()
    {
        /*if (GameManager.Instance != null)
            currentCategory = GameManager.Instance.GetCurrentCategory();*/
    }

    private void CreateCategoryList()
    {
        int count = Enum.GetValues(typeof(TutorialType)).Length;

        for(int i = 0; i < count; i++)
        {
            TutorialType type = (TutorialType)i;
            Button button = Instantiate(categoryButton, categoryList);
            button.onClick.AddListener(() => ShowCategory(type));
        }
    }

    private void ShowCategory(TutorialType type)
    {
        // Deactivate tutorial category list
        categoryList.gameObject.SetActive(false);

        // Delete previous content
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        // Show all tutorials
        List<PopUpData> tutorials = shownTemary[type];
        PopUpData lastData = null;
        foreach(PopUpData data in tutorials)
        {
            if(data == null || data.title != lastData.title)
                AddTitle(data.title);
            AddParagraph(data.content);
            lastData = data;
        }
    }

    private void ShowTutorialsCategoryList()
    {
        // Delete previous content
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        // Show tutorials category list
        categoryList.gameObject.SetActive(true);
    }

    public static void AddTemary(TutorialType id, PopUpData data)
    {
        if (!shownTemary.ContainsKey(id))
            shownTemary.Add(id, new List<PopUpData>());

        List<PopUpData> list = shownTemary[id];
        if (!list.Contains(data))
            list.Add(data);
        else return;

        lastTutorialType = id;
        lastData = data;
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
}
