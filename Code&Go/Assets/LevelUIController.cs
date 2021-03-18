using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelUIController : MonoBehaviour
{

    public void Open(RectTransform panel)
    {
        if (panel.gameObject.activeSelf) return;

        UnityEvent<float> mEvent = new UnityEvent<float>();
        Tween slideTween = new Tween(AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f), mEvent, 0.2f);

        float width = panel.rect.width;
        slideTween.OnStart.AddListener(() => {
            panel.gameObject.SetActive(true);
            panel.anchoredPosition = new Vector2(width, 0.0f);
        });

        slideTween.Function.AddListener((float k) => {
            panel.anchoredPosition = new Vector2(width * (1.0f - k), 0.0f);
        });

        slideTween.OnFinished.AddListener(() => {
            panel.anchoredPosition = new Vector2(0.0f, 0.0f);
        });
        TweenManager.Instance.AddTween(slideTween);
    }

    public void Close(RectTransform panel)
    {
        if (!panel.gameObject.activeSelf) return;

        UnityEvent<float> mEvent = new UnityEvent<float>();
        Tween slideTween = new Tween(AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f), mEvent, 0.2f);

        float width = panel.rect.width;

        slideTween.OnStart.AddListener(() => {
            panel.anchoredPosition = new Vector2(0.0f, 0.0f);
        });

        slideTween.Function.AddListener((float k) => {
            panel.anchoredPosition = new Vector2(width * k, 0.0f);
        });

        slideTween.OnFinished.AddListener(() => {
            panel.anchoredPosition = new Vector2(width, 0.0f);
            panel.gameObject.SetActive(false);
        });

        TweenManager.Instance.AddTween(slideTween);
    }
}
