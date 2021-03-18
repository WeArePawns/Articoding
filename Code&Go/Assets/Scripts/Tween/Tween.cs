using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using BezierCurve = UnityEngine.AnimationCurve;

public class Tween : MonoBehaviour
{
    [SerializeField] private BezierCurve curve = BezierCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [Min(0.0f)]
    [SerializeField] private float duration;
    [Space]
    [SerializeField] private UnityEvent<float> function;

    [System.Serializable]
    public struct TweenCallbacks
    {
        [Tooltip("Se llama justo antes de empezar la interpolacion")]
        public UnityEvent OnStart;
        [Tooltip("Se llama justo despues de terminar la interpolacion")]
        public UnityEvent OnFinished;
    }

    [SerializeField] private TweenCallbacks callbacks;

    public Tween()
    {
        function = new UnityEvent<float>();
        OnStart = new UnityEvent();
        OnFinished = new UnityEvent();
        duration = 1.0f;
    }

    public Tween(UnityEvent<float> func, float duration)
    {
        function = func;
        this.duration = duration;
        OnStart = new UnityEvent();
        OnFinished = new UnityEvent();
    }

    public Tween(BezierCurve curve, UnityEvent<float> func, float duration)
    {
        this.curve = curve;
        function = func;
        this.duration = duration;
        OnStart = new UnityEvent();
        OnFinished = new UnityEvent();
    }
    public BezierCurve Curve
    {
        get { return curve; }
        set { curve = value; }
    }

    public UnityEvent<float> Function
    {
        get { return function; }
        set { function = value; }
    }

    public float Duration
    {
        get { return duration; }
        set { duration = value; }
    }

    public UnityEvent OnStart {
        get { return callbacks.OnStart; }
        set { callbacks.OnStart = value; }
    }
    public UnityEvent OnFinished
    {
        get { return callbacks.OnFinished; }
        set { callbacks.OnFinished = value; }
    }
}
