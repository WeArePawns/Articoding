﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    [SerializeField] private UnityEvent onDisable;
    [SerializeField] private GameObject panel;

    private void Start()
    {
        if (panel == null)
            panel = gameObject;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            HideIfClickedOutside();
    }

    private void HideIfClickedOutside()
    {
        if (Input.GetMouseButton(0) &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
        {
            panel.SetActive(false);
            onDisable.Invoke();
        }
    }

    public void Appear()
    {
        panel.SetActive(!gameObject.activeSelf);
    }
}
