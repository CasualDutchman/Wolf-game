using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Screens { Hud, Settings, Alpha }

[System.Serializable]
public class SkillHolder {
    public Image skillRadial;
    public Text skillName;
}

public class UIManager : MonoBehaviour {

    Screens currentScreen = Screens.Hud;

    public static UIManager instance;

    public UIComponents components;

    Dictionary<string, LocalizedItem> textRegistry = new Dictionary<string, LocalizedItem>();

    SkillHolder[] skillHolder = new SkillHolder[5];

    [Header("Adio")]
    UnityEngine.Audio.AudioMixer mixer;

    [Header("HUD")]
    public string unlocalizedExperience;

    [Header("Alpha")]
    public string unlocalizedAlphaTitle;

    [Header("Settings")]
    bool audioSettings = true;
    bool graphicalSettings = false;
    bool settingsLoaded = false;
    public string unlocalizedSettingsTitle;
    public string unlocalizedGraphics;
    public string unlocalizedGraphicsOnOff;
    public string unlocalizedAudio;
    public string unlocalizedAudioOnOff;

    void Awake() {
        instance = this;
    }

    void Start() {
        RegisterTexts();
        LoadSettings();
    }

    void Update() {
        
    }

    void RegisterTexts() {
        //HUD
        RegisterText(components.textExperience, unlocalizedExperience);
        RegisterButton(components.buttonSettings, () => ChangeScreen(Screens.Settings));

        //Alpha
        RegisterSkill(components.skillA1, 0);
        RegisterSkill(components.skillA2, 1);
        RegisterSkill(components.skillB1, 2);
        RegisterSkill(components.skillB2, 3);
        RegisterSkill(components.skillB3, 4);

        //Settings
        RegisterButton(components.buttonBackSettings, () => ChangeScreen(Screens.Hud));
        RegisterText(components.textSettingsTitle, unlocalizedSettingsTitle);
        RegisterText(components.textGraphical, unlocalizedGraphics);
        RegisterText(components.textGraphicalOnOff, unlocalizedGraphicsOnOff);
        RegisterText(components.textAudio, unlocalizedAudio);
        RegisterText(components.textAudioOnOff, unlocalizedAudioOnOff);
        RegisterToggle(components.buttonGraphics, (b) => { OnToggleGraphical(b);});
        RegisterToggle(components.buttonAudio, (b) => { OnToggleAudio(b); });
    }

    #region Registry
    void RegisterButton(Transform t, UnityEngine.Events.UnityAction action) {
        Button b = t.GetComponent<Button>();
        b.onClick.AddListener(action);
    }

    void RegisterToggle(Transform t, UnityEngine.Events.UnityAction<bool> action) {
        Toggle tog = t.GetComponent<Toggle>();
        tog.onValueChanged.AddListener(action);
    }

    void RegisterSkill(Transform t, int i) {
        skillHolder[i] = new SkillHolder() {
            skillRadial = t.GetChild(1).GetComponent<Image>(),
            skillName = t.GetChild(2).GetChild(0).GetComponent<Text>()
        };
    }

    void RegisterText(Transform t, string key) {
        LocalizedItem item = t.gameObject.AddComponent<LocalizedItem>();
        item.Register(key.ToLower());
        item.Change();
        textRegistry.Add(key, item);
    }

    void UpdateText(string key, params object[] par) {
        textRegistry[unlocalizedExperience].Change();
        textRegistry[unlocalizedExperience].UpdateItem(par);
    }

    public void OnChangeLanguagePref() {
        UpdateText(unlocalizedExperience);
    }

    public Image GetImage(Transform obj) {
        return obj.GetComponent<Image>();
    }

    public Text GetText(Transform obj) {
        return obj.GetComponent<Text>();
    }
    #endregion

    #region Screens
    GameObject GetScreen(Screens screen) {
        switch (screen) {
            default: case Screens.Hud: return components.screenHUD;
            case Screens.Alpha: return components.screenAlpha;
            case Screens.Settings: return components.screenSettings;
        }
    }

    public void ChangeScreen(Screens screen) {
        StartCoroutine(IEChangeScreen(screen));
    }

    IEnumerator IEChangeScreen(Screens screen) {
        GetScreen(currentScreen).SetActive(false);
        currentScreen = screen;
        GetScreen(currentScreen).SetActive(true);
        yield return 0;
    }
    #endregion

    #region Hud

    public void UpdateHealthBar(float fill) {
        GetImage(components.imageHealthFill).fillAmount = fill;
    }

    public void UpdateFoodBar(float fill) {
        GetImage(components.imageFoodBarFill).fillAmount = fill;
    }

    public void UpdateExperienceBar(float fill) {
        GetImage(components.imageExperienceBarFill).fillAmount = fill;
    }

    public void UpdateLevelText(int i) {
        UpdateText(unlocalizedExperience, i);
    }
    #endregion

    #region Settings
    void LoadSettings() {
        //graphicalSettings = PlayerPrefs.GetInt(unlocalizedGraphics) == 1;
        //audioSettings = PlayerPrefs.GetInt(unlocalizedAudio) == 1;

        components.buttonGraphics.GetComponent<Toggle>().isOn = graphicalSettings;
        components.buttonAudio.GetComponent<Toggle>().isOn = audioSettings;

        settingsLoaded = true;
    }

    public void OnToggleAudio(bool b) {
        if (settingsLoaded) {
            audioSettings = !audioSettings;
        }
    }

    public void OnToggleGraphical(bool b) {
        if (settingsLoaded) {
            graphicalSettings = !graphicalSettings;
        }
    }
    #endregion

}


