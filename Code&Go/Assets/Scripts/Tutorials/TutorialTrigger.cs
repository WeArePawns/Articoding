using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour, IComparable<TutorialTrigger>
{
    public int priority;
    public PopUpData info;
    public Func<bool> condition;

    public bool highlightObject = true;
    public bool destroyOnShowed = false;
    public bool isSaveCheckpoint = false;

    public UnityEvent OnShowed;

    private RectTransform mRectTransform;
    private Renderer mRenderer;
    private Collider mCollider;
    private bool waitColliderSync = false;
    private bool waitCanvasSync = false;

    private void Awake()
    {
        if ((mRectTransform = GetComponent<RectTransform>()) != null) { waitCanvasSync = true; return; }
        if ((mRenderer = GetComponent<Renderer>()) != null) return;
        if ((mCollider = GetComponent<Collider>()) != null) { waitColliderSync = true; return; }
    }

    public void Start()
    {
        if(waitColliderSync)
        {
            StartCoroutine(AsyncColliderStart());
            return;
        }

        if (waitCanvasSync)
        {
            StartCoroutine(AsyncCanvasStart());
            return;
        }

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.AddTutorialTrigger(this, true);
    }

    public IEnumerator AsyncColliderStart()
    {
        yield return new WaitForFixedUpdate();

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.AddTutorialTrigger(this, true);
    }

    public IEnumerator AsyncCanvasStart()
    {
        yield return new WaitForEndOfFrame();

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.AddTutorialTrigger(this, true);
    }

    public Rect GetRect()
    {
        if (mRectTransform != null) return RectUtils.RectTransformToScreenSpace(mRectTransform);
        if (mRenderer != null) return RectUtils.RendererToScreenSpace(mRenderer);
        if (mCollider != null) return RectUtils.ColliderToScreenSpace(mCollider);
        return new Rect(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f, 0.0f); ;
    }

    public int CompareTo(TutorialTrigger other)
    {
        if (other.condition != null && condition != null)
        {
            bool mCondition = condition.Invoke();
            bool oCondition = other.condition.Invoke();

            if (mCondition && !oCondition) return -1;
            if (!mCondition && oCondition) return 1;
        }
        return priority - other.priority;
    }

    public string GetHash()
    {
        return Hash.ToHash(info.title + info.content, "TutorialTrigger");
    }

    // Returns true if hash is the same as GetHash()
    public bool CompareHash(string hash)
    {
        return hash == GetHash();
    }
}
