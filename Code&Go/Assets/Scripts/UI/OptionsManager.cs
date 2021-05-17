using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    void Awake()
    {
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

    IEnumerator Start()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new Dropdown.OptionData(locale.name));
        }
        languageDropdown.options = options;

        languageDropdown.value = selected;
        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownUsed);
    }

    public void OnLanguageDropdownUsed(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
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
