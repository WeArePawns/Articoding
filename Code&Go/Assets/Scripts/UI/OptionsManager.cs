using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    void Start()
    {
        // TODO: Initialize dropdown using Locale (Localizacion)
        // ...

        fullscreenToggle.isOn = Screen.fullScreen;

    }

    public void OnLanguageDropdownUsed()
    {
        // TODO: Change app locale

        // Maybe change flag image
    }

    public void OnFullscreenToggleUsed()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }


}
