using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [SerializeField] private Category[] categories;

    [SerializeField] private GameObject categoriesParent;
    [SerializeField] private CategoryCard categoryCardPrefab;

    public Text categoryName;
    public Text categoryDescription;

    public int currentCategory;

    private void Start()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;

            CategoryCard card = Instantiate(categoryCardPrefab, categoriesParent.transform);
            card.ConfigureCategory(categories[i]);
            card.button.onClick.AddListener(() =>
            {
                SelectCategory(index);
            });
        }
    }

    private void SelectCategory(int index)
    {
        if (index >= 0 && index < categories.Length)
        {
            currentCategory = index;

            categoryName.text = categories[currentCategory].name_id;
            categoryDescription.text = categories[currentCategory].description;
        }
    }
}
