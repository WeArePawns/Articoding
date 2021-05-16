using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    void Awake()
    {
        // TODO: Initialize dropdown using Locale (Localizacion)
        // ...

        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolutions[resolutions.Length - i - 1].ToString()));
            if(resolutions[resolutions.Length - i - 1].Equals(Screen.currentResolution))
            {
                resolutionDropdown.value = i;
            }
        }


        fullscreenToggle.isOn = Screen.fullScreen;

    }

    public void OnLanguageDropdownUsed()
    {
        // TODO: Change app locale

        // Maybe change flag image
    }

    public void OnResolutionDropdownUsed()
    {
        Resolution res = Screen.resolutions[Screen.resolutions.Length - resolutionDropdown.value - 1];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void OnFullscreenToggleUsed()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }


}
