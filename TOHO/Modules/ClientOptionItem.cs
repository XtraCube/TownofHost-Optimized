using BepInEx.Configuration;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TOHO;

//来源：https://github.com/tukasa0001/TownOfHost/pull/1265
public class ClientOptionItem
{
    public ConfigEntry<bool> Config;
    public ToggleButtonBehaviour ToggleButton;

    public static SpriteRenderer CustomBackground;
    private static int numOptions = 0;

    private ClientOptionItem(
        string name,
        ConfigEntry<bool> config,
        OptionsMenuBehaviour optionsMenuBehaviour,
        Action additionalOnClickAction = null)
    {
        try
        {
            Config = config;

            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;

            // 1つ目のボタンの生成時に背景も生成
            if (CustomBackground == null)
            {
                numOptions = 0;
                CustomBackground = Object.Instantiate(optionsMenuBehaviour.Background, optionsMenuBehaviour.transform);
                CustomBackground.name = "CustomBackground";
                CustomBackground.transform.localScale = new(0.9f, 0.9f, 1f);
                CustomBackground.transform.localPosition += Vector3.back * 8;
                CustomBackground.gameObject.SetActive(false);

                var closeButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
                closeButton.transform.localPosition = new(1.3f, -2.3f, -6f);
                closeButton.name = "Back";
                closeButton.Text.text = Translator.GetString("Back");
                closeButton.Background.color = Palette.DisabledGrey;
                var closePassiveButton = closeButton.GetComponent<PassiveButton>();
                closePassiveButton.OnClick = new();
                closePassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    CustomBackground.gameObject.SetActive(false);
                }));

                UiElement[] selectableButtons = optionsMenuBehaviour.ControllerSelectable.ToArray();
                PassiveButton leaveButton = null;
                PassiveButton returnButton = null;
                foreach (var button in selectableButtons)
                {
                    if (button == null) continue;

                    if (button.name == "LeaveGameButton")
                        leaveButton = button.GetComponent<PassiveButton>();
                    else if (button.name == "ReturnToGameButton")
                        returnButton = button.GetComponent<PassiveButton>();
                }
                var generalTab = mouseMoveToggle.transform.parent.parent.parent;

                var modOptionsButton = Object.Instantiate(mouseMoveToggle, generalTab);
                modOptionsButton.transform.localPosition = new(1.2f, -1.8f, 1f);
                modOptionsButton.name = "TOHOOptions";
                modOptionsButton.Text.text = Translator.GetString("TOHOOptions");
                modOptionsButton.Background.color = new Color32(180, 126, 222, byte.MaxValue);
                var modOptionsPassiveButton = modOptionsButton.GetComponent<PassiveButton>();
                modOptionsPassiveButton.OnClick = new();
                modOptionsPassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    CustomBackground.gameObject.SetActive(true);
                }));

                if (leaveButton != null)
                    leaveButton.transform.localPosition = new(-1.35f, -2.411f, -1f);
                if (returnButton != null)
                    returnButton.transform.localPosition = new(1.35f, -2.411f, -1f);
            }

            // ボタン生成
            ToggleButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
            ToggleButton.transform.localPosition = new Vector3(
                // 現在のオプション数を基に位置を計算
                numOptions % 2 == 0 ? -1.3f : 1.3f,
                2.2f - (0.5f * (numOptions / 2)),
                -6f);
            ToggleButton.name = name;
            ToggleButton.Text.text = Translator.GetString(name);
            var passiveButton = ToggleButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                if (config != null) config.Value = !config.Value;
                UpdateToggle();
                additionalOnClickAction?.Invoke();
            }));
            UpdateToggle();
        }
        finally { numOptions++; }
    }

    public static ClientOptionItem Create(
        string name,
        ConfigEntry<bool> config,
        OptionsMenuBehaviour optionsMenuBehaviour,
        Action additionalOnClickAction = null)
    {
        return new(name, config, optionsMenuBehaviour, additionalOnClickAction);
    }

    public void UpdateToggle()
    {
        if (ToggleButton == null) return;

        var color = (Config != null && Config.Value) ? new Color32(180, 126, 222, byte.MaxValue) : new Color32(77, 77, 77, byte.MaxValue);
        ToggleButton.Background.color = color;
        ToggleButton.Rollover?.ChangeOutColor(color);
    }
}

public class ThemeOptionItem
{
    public ConfigEntry<bool> Config;
    public ToggleButtonBehaviour modOptionsButton;
    public static int ThemeID = 1;
    
    public static SpriteRenderer CustomBackground;

    private ThemeOptionItem(
        ConfigEntry<int> config,
        OptionsMenuBehaviour optionsMenuBehaviour
        )
    {
        var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;
        var generalTab = mouseMoveToggle.transform.parent.parent.parent;
        PassiveButton leaveButton = null;
        foreach (var button in optionsMenuBehaviour.ControllerSelectable.ToArray())
        {
            if (button == null) continue;

            if (button.name == "LeaveGameButton")
                leaveButton = button.GetComponent<PassiveButton>();
        }        
        modOptionsButton = Object.Instantiate(mouseMoveToggle, generalTab);

        modOptionsButton.transform.localPosition = new(-1.2f, -1.8f, 1f);
        modOptionsButton.name = "TOHOTheme";

        var theme = "None";
        switch (ThemeID)
        {
            case 1:
                theme = "Classic";
                break;
            case 2:
                theme = "Dark";
                break;
            case 3:
                theme = "Mars Red";
                break;
            case 4:
                theme = "Golden Yellow";
                break;
            case 5:
                theme = "Forest Green";
                break;
            case 6:
                theme = "Deep Sea Blue";
                break;
        }
        modOptionsButton.Text.text = "Theme: " + theme;
        modOptionsButton.Background.color = new Color32(180, 126, 222, byte.MaxValue);
        var modOptionsPassiveButton = modOptionsButton.GetComponent<PassiveButton>();
        modOptionsPassiveButton.OnClick = new();
        modOptionsPassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            if (ThemeID >= 6) ThemeID = 0;
            else ThemeID++;
            UpdateToggle();
        }));
        if (leaveButton != null)
            leaveButton.transform.localPosition = new(-1.35f, -2.411f, -1f);
        UpdateToggle();
    }

    public static ThemeOptionItem Create(
        ConfigEntry<int> config,
        OptionsMenuBehaviour optionsMenuBehaviour)
    {
        return new(config, optionsMenuBehaviour);
    }

    public void UpdateToggle()
    {
        if (modOptionsButton == null) return;

        var color = new Color(0, 0, 0);
        
        switch (ThemeID)
        {
            case 1: 
                color = new Color32(225, 225, 225, byte.MaxValue);
                break;

            case 2: 
                color = new Color32(55, 55, 55, byte.MaxValue);
                break;

            case 3: 
                color = new Color32(112, 33, 25, byte.MaxValue);
                break;

            case 4: 
                color = new Color32(117, 83, 11, byte.MaxValue);
                break;

            case 5: 
                color = new Color32(36, 69, 25, byte.MaxValue);
                break;

            case 6: 
                color = new Color32(6, 13, 56, byte.MaxValue);
                break;
        }
        modOptionsButton.Background.color = color;
        modOptionsButton.Rollover?.ChangeOutColor(color);
        var theme = "None";
        switch (ThemeID)
        {
            case 1:
                theme = "Classic";
                break;
            case 2:
                theme = "Dark";
                break;
            case 3:
                theme = "Mars Red";
                break;
            case 4:
                theme = "Golden Yellow";
                break;
            case 5:
                theme = "Forest Green";
                break;
            case 6:
                theme = "Deep Sea Blue";
                break;
        }
        modOptionsButton.Text.text = "Theme: " + theme;
    }
}
