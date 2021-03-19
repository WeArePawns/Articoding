using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] private KeyCode leftKeyCode;
    [SerializeField] private KeyCode rightKeyCode;

    [SerializeField] private Text leftText;
    [SerializeField] private Text rightText;

    public Tab[] tabs;
    public GameObject[] panels;

    public int tabIndex;

    void Start()
    {
        ConfigureTabs();
        ConfigureTexts();

        tabs[tabIndex].Select();
        //panels[tabIndex].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(leftKeyCode))
        {
            // Desactivamos el tab actual
            tabs[tabIndex].Deselect();

            tabIndex = (tabIndex + tabs.Length - 1) % tabs.Length;
            tabs[tabIndex].Select();
        }
        else if (Input.GetKeyDown(rightKeyCode))
        {
            // Desactivamos el tab actual
            tabs[tabIndex].Deselect();

            tabIndex = (tabIndex + 1) % tabs.Length;
            tabs[tabIndex].Select();
        }
    }

    private void SelectCallback(int index)
    {
        if (index >= tabs.Length || index < 0) return;

        // Desactivamos el tab actual
        if(tabIndex != index) tabs[tabIndex].Deselect();

        // Activamos el nuevo tab seleccionado
        tabIndex = index;
        panels[tabIndex].SetActive(true);
    }
    private void DeselectCallback(int index)
    {
        if (index >= tabs.Length || index < 0) return;

        panels[index].SetActive(false);
    }

    private void ConfigureTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;

            tabs[i].callbacks.OnSelected.AddListener(() =>
            {
                SelectCallback(index);
            });

            tabs[i].callbacks.OnDeselected.AddListener(() =>
            {
                DeselectCallback(index);
            });

            tabs[i].Deselect();
        }
    }

    private void ConfigureTexts()
    {
        rightText.text = rightKeyCode.ToString();
        leftText.text = leftKeyCode.ToString();
    }
}
