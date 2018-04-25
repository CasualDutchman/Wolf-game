using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public UIComponents components;

    [Header("HUD")]
    public string levelPreset;

    void Awake() {
        instance = this;
    }

    void Start() {
        
    }

    public void OnChangeLanguagePref() {
        
    }

    public Image GetImage(Transform obj) {
        return obj.GetComponent<Image>();
    }

    public Text GetText(Transform obj) {
        return obj.GetComponent<Text>();
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
        GetText(components.textExperience).text = LocalizationManager.instance.GetLocalizedValue(levelPreset, i); //string.Format(LocalizationManager.instance.GetLocalizedValue(levelPreset), i);
    }
}
