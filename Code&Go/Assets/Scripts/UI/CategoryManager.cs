using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [SerializeField] private Category[] categories;

    [SerializeField] private GameObject categoriesParent;
    [SerializeField] private CategoryCard categoryCardPrefab;

    [SerializeField] private GameObject levelsParent;
    [SerializeField] private LevelCard levelCardPrefab;

    [SerializeField] private GameObject levelsCreatedParent;
    [SerializeField] private Button createLevelButton;
    [SerializeField] private Category levelsCreatedCategory;

    public Text categoryName;
    public Text categoryDescription;

    public Text levelName;
    public Image levelPreview;

    public Text levelCreatedName;

    public GameObject categoriesPanel;
    public GameObject levelsPanel;

    public GameObject currentCategoryPanel;
    public GameObject currentLevelPanel;

    public GameObject currentLevelCreatedPanel;

    public Text currentCategoryLevelsText;

    public int currentCategory;
    private int currentLevel;
    private int levelCreatedIndex = -1000;

    public Color deactivatedCategoryColor;
    public Sprite deactivatedImage;

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

            //If it's not unlocked it can't be selected
            if (!ProgressManager.Instance.IsCategoryUnlocked(index))
            {
                card.button.enabled = false;
                card.image.sprite = deactivatedImage;
                card.button.image.color = deactivatedCategoryColor;
            }
            //TODO: Poner icono de candado o algo
        }
        //currentLevelCreatedPanel.SetActive(false);

        HideLevels();

        SelectCategory(currentCategory);

        CreateUserLevelsCards();
    }

    private void CreateUserLevelsCards()
    {
        for (int i = 0; i < levelsCreatedCategory.levels.Count; i++)
        {
            int index = i;
            LevelData levelData = levelsCreatedCategory.levels[i];
            LevelCard levelCard = Instantiate(levelCardPrefab, levelsCreatedParent.transform);
            levelCard.ConfigureLevel(levelData, levelsCreatedCategory, i + 1);
            levelCard.DeactivateStars();
            levelCard.button.onClick.AddListener(() =>
            {
                //currentLevelCreatedPanel.SetActive(true);
                levelCreatedIndex = index;
                levelCreatedName.text = levelData.levelName;
            });
        }
        createLevelButton.gameObject.SetActive(true);
        createLevelButton.onClick.AddListener(() =>
        {
            levelCreatedName.text = "Crear nivel";
            levelCreatedIndex = -1; // Reserved for creator mode
        });
        createLevelButton.transform.SetParent(levelsCreatedParent.transform);

        createLevelButton.onClick.Invoke();
    }

    private void SelectCategory(int index)
    {
        if (!ProgressManager.Instance.IsCategoryUnlocked(index)) return;

        if (index >= 0 && index < categories.Length)
        {
            currentCategory = index;

            categoryName.text = categories[currentCategory].name_id;
            categoryDescription.text = categories[currentCategory].description;
        }
    }

    public void ShowLevels()
    {
        if (!ProgressManager.Instance.IsCategoryUnlocked(currentCategory)) return;

        Category category = categories[currentCategory];
        currentCategoryLevelsText.text = "Levels - " + category.name_id;

        categoriesPanel.SetActive(false);
        levelsPanel.SetActive(true);

        currentCategoryPanel.SetActive(false);
        currentLevelPanel.SetActive(true);

        for (int i = 0; i < category.levels.Count; i++)
        {
            int index = i;
            LevelData levelData = category.levels[i];
            LevelCard levelCard = Instantiate(levelCardPrefab, levelsParent.transform);
            levelCard.ConfigureLevel(levelData, category, i + 1);
            if (ProgressManager.Instance.IsLevelUnlocked(currentCategory, i))
            {
                levelCard.button.onClick.AddListener(() =>
                {
                    currentLevel = index;
                    levelName.text = levelData.levelName;
                    levelPreview.sprite = levelData.levelPreview;
                });
                levelCard.button.onClick.Invoke();
            }
            else
                levelCard.DeactivateCard();
        }
    }

    public void HideLevels()
    {
        currentCategoryLevelsText.text = "Categories";

        categoriesPanel.SetActive(true);
        levelsPanel.SetActive(false);

        currentCategoryPanel.SetActive(true);
        currentLevelPanel.SetActive(false);

        while (levelsParent.transform.childCount != 0)
            DestroyImmediate(levelsParent.transform.GetChild(0).gameObject);
    }

    public void PlaySelectedLevel()
    {
        GameManager.Instance.LoadLevel(categories[currentCategory], currentLevel);
    }

    public void PlayLevelCreated()
    {
        if (levelCreatedIndex == -1)
            GameManager.Instance.LoadLevelCreator();
        else
            GameManager.Instance.LoadLevel(levelsCreatedCategory, levelCreatedIndex);
    }
}
