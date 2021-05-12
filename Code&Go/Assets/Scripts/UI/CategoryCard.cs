using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CategoryCard : MonoBehaviour
{
    [SerializeField] private Category category;

    [SerializeField] private Text title;
    [SerializeField] private Text stars;

    [SerializeField] private ProgressBar progressBar; // TODO: hacer algo con esto

    [HideInInspector] public Button button;
    public Image image;

#if !UNITY_EDITOR
    private void Awake()
    {
        Configure();
    }
#else
    private void Update()
    {
        //Configure();
    }
#endif
    private void Configure()
    {
        if (category == null) return;

        button = GetComponent<Button>();

        title.text = category.name_id;
        stars.text = ProgressManager.Instance.GetCategoryTotalStars(category).ToString() + "/" + (category.levels.Count * 3).ToString(); ; //category.description;
        progressBar.minimum = 0.0f;
        progressBar.maximum = category.levels.Count;
        progressBar.current = ProgressManager.Instance.GetCategoryCurrentProgress(category);
        progressBar.Configure();
        image.sprite = category.icon;
    }

    public void ConfigureCategory(Category category)
    {
        this.category = category;
        Configure();
    }
}
