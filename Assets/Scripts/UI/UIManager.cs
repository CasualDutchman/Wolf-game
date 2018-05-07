using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public EventSystem eventSystem;
    public WolfPack wolfPack;

    Settingsmanager settings;
    SkillManager skillManager;

    Dictionary<string, LocalizedItem> textRegistry = new Dictionary<string, LocalizedItem>();

    SkillHolder[] skillHolder = new SkillHolder[5];

    [Header("Adio")]
    UnityEngine.Audio.AudioMixer mixer;

    [Header("HUD")]
    public string unlocalizedExperience;
    public string unlocalizedAlpha;
    public float sideAppearTime = 1;
    bool sideOpen = false;
    bool sideChanging = false;
    Vector3 localSizeOrigin;

    [Header("Alpha")]
    public string unlocalizedAlphaTitle;

    [Header("Settings")]
    public string unlocalizedSettingsTitle;
    public string unlocalizedGraphics;
    public string unlocalizedGraphicsOnOff;
    public string unlocalizedAudio;
    public string unlocalizedAudioOnOff;
    public string unlocalizedEnglish;
    public string unlocalizedDutch;

    void Awake() {
        instance = this;
    }

    void Start() {
        settings = Settingsmanager.instance;
        skillManager = GetComponent<SkillManager>();
        localSizeOrigin = components.buttonSide.localPosition;
        RegisterTexts();
        LoadSettings();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (currentScreen != Screens.Hud) {
                ChangeScreen(Screens.Hud);
            } else {
                skillManager.SaveSkills();
                wolfPack.Save();
                PlayerPrefs.Save();
                Application.Quit();
            }
        }
    }

    void OnApplicationPause(bool pause) {
        if (pause) {
            skillManager.SaveSkills();
            wolfPack.Save();
            PlayerPrefs.Save();
        }
    }

    void RegisterTexts() {
        //HUD
        RegisterText(components.textExperience, unlocalizedExperience);
        RegisterButton(components.buttonSettings, () => ChangeScreen(Screens.Settings));
        RegisterButton(components.buttonSide, () => OnSide());
        RegisterButton(components.buttonAlpha, () => ChangeScreen(Screens.Alpha));
        RegisterText(components.buttonAlpha.GetChild(0), unlocalizedAlpha);

        //Alpha
        RegisterSkill(components.skillA1, 0);
        RegisterSkill(components.skillA2, 1);
        RegisterSkill(components.skillB1, 2);
        RegisterSkill(components.skillB2, 3);
        RegisterSkill(components.skillB3, 4);
        RegisterButton(components.buttonBackAlpha, () => ChangeScreen(Screens.Hud));

        //Settings
        RegisterButton(components.buttonBackSettings, () => ChangeScreen(Screens.Hud));
        RegisterText(components.textSettingsTitle, unlocalizedSettingsTitle);
        RegisterText(components.textGraphical, unlocalizedGraphics);
        RegisterText(components.textGraphicalOnOff, unlocalizedGraphicsOnOff);
        RegisterText(components.textAudio, unlocalizedAudio);
        RegisterText(components.textAudioOnOff, unlocalizedAudioOnOff);
        RegisterToggle(components.buttonGraphics, (b) => { OnToggleGraphical(b);});
        RegisterToggle(components.buttonAudio, (b) => { OnToggleAudio(b); });
        RegisterButton(components.buttonEnglish, () => OnChangeLanguage("EN_us"));
        RegisterText(components.buttonEnglish.GetChild(0), unlocalizedEnglish);
        RegisterButton(components.buttonDutch, () => OnChangeLanguage("NL_nl"));
        RegisterText(components.buttonDutch.GetChild(0), unlocalizedDutch);
        RegisterButton(components.buttonQuitDelete, () => DeleteAndQuit());
    }

    public bool IsHittingUI() {
        return eventSystem.IsPointerOverGameObject();
    }

    public bool IsHUD() {
        return currentScreen == Screens.Hud;
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
        foreach (KeyValuePair<string, LocalizedItem> item in textRegistry) {
            item.Value.Change();
            //item.Value.UpdateItem("");
        }
        //UpdateText(unlocalizedExperience);
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
        if(screen == Screens.Hud) {
            components.buttonSide.localPosition = localSizeOrigin;
            sideOpen = false;
        }
        else if (screen == Screens.Alpha) {
            OnAlphaOpened();
        }
        StartCoroutine(IEChangeScreen(screen));
    }

    IEnumerator IEChangeScreen(Screens screen) {
        GetScreen(currentScreen).SetActive(false);
        currentScreen = screen;
        GetScreen(currentScreen).SetActive(true);
        yield return 0;
    }
    #endregion

    #region Alpha
    void OnAlphaOpened() {
        for (int i = 0; i < 5; i++) {
            skillHolder[i].skillName.text = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.unlocalizedName);
            if (skillManager.skills[i].skill.skillType == SkillType.A) {
                skillHolder[i].skillRadial.fillAmount = (float)skillManager.skills[i].animalCounter / (float)skillManager.skills[i].skill.animalCount;
            } else {
                skillHolder[i].skillRadial.fillAmount = 1;
            }
        }
    }
    #endregion

    #region Hud
    public void OnSide() {
        if (sideChanging)
            return;

        sideChanging = true;
        StartCoroutine(Side());
    }

    IEnumerator Side() {
        float timer = 0;
        bool run = true;
        while (run) {
            timer += Time.deltaTime * (1 / sideAppearTime);
            Vector3 newpos;
            if (!sideOpen) {
                newpos = Vector3.Lerp(localSizeOrigin, localSizeOrigin + new Vector3(220, 0, 0), timer);
            }else {
                newpos = Vector3.Lerp(localSizeOrigin + new Vector3(220, 0, 0), localSizeOrigin, timer);
            }
            components.buttonSide.localPosition = newpos;
            if (timer >= 1) {
                sideOpen = !sideOpen;
                run = false;
                sideChanging = false;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

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
        settings.graphicalSettings = PlayerPrefs.GetInt(unlocalizedGraphics) == 0 ? Graphical.High : Graphical.Low;
        settings.audioSettings = PlayerPrefs.GetInt(unlocalizedAudio) == 0 ? OnOff.On : OnOff.Off;

        settings.SetGraphical();
        settings.SetAudio();

        components.buttonGraphics.GetComponent<Toggle>().isOn = settings.graphicalSettings == Graphical.High;
        components.buttonAudio.GetComponent<Toggle>().isOn = settings.audioSettings == OnOff.On;

        settings.EnableSettings();
    }

    public void OnToggleAudio(bool b) {
        settings.OnToggleAudio(b, unlocalizedAudio);
    }

    public void OnToggleGraphical(bool b) {
        settings.OnToggleGraphical(b, unlocalizedGraphics);
    }

    public void OnChangeLanguage(string lang) {
        if (!LocalizationManager.instance.loadedlanguage.Equals(lang)) {
            LocalizationManager.instance.OnChangeLanguagePref(lang);
            OnChangeLanguagePref();
        }
    }

    public void DeleteAndQuit() {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
    #endregion

}


