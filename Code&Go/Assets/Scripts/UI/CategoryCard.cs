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
        stars.text = "32/45"; //category.description;
        image.sprite = category.icon;
    }

    public void ConfigureCategory(Category category)
    {
        this.category = category;
        Configure();
    }
}
