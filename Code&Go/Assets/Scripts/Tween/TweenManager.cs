using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
    public static TweenManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void AddTween(Tween tween)
    {
        StartCoroutine(StartTweening(tween));
    }

    private IEnumerator StartTweening(Tween tween)
    {
        if(tween.OnStart != null)
            tween.OnStart.Invoke();

        float timer = 0.0f;

        while(timer < tween.Duration)
        {
            float value = tween.Curve.Evaluate(timer / tween.Duration);
            tween.Function.Invoke(value);
            timer += Time.deltaTime;
            yield return null;
        }
        tween.Function.Invoke(tween.Curve.Evaluate(1.0f));

        if (tween.OnFinished != null)
            tween.OnFinished.Invoke();

        yield return null;
    }

    public void ExampleTween(float k)
    {
        gameObject.transform.localScale = Vector3.one + new Vector3(k, k, k);
    }

}
