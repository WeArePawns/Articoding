using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelCard : MonoBehaviour
{
    private LevelData level;
    private Category category;

    [SerializeField] private Text title;
    [SerializeField] private Image[] stars;
    [SerializeField] private Image cardImage;

    [HideInInspector] public Button button;

    private Color starsColor;
    public Color deactivatedColor;

    int numLevel = 1;

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
        if (level == null) return;

        button = GetComponent<Button>();
        starsColor = stars[0].color;
        starsColor.a = 0.2f;

        title.text = numLevel.ToString();

        int levelStars = ProgressManager.Instance.GetLevelStars(category, numLevel - 1);

        //cambia el color de las estrellas que ha conseguido el jugador en el nivel
        for (int i = 0; i < 3; i++)
            if (i >= levelStars)
                stars[i].color = starsColor;
    }

    public void ConfigureLevel(LevelData level, Category category, int numLevel)
    {
        this.level = level;
        this.numLevel = numLevel;
        this.category = category;
        Configure();
    }

    public void DeactivateStars()
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = false;
    }

    public void DeactivateCard()
    {
        cardImage.color = deactivatedColor;
        button.interactable = false;
    }
}
