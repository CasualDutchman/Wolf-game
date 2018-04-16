using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void UpdateHealthBar(float fill) {
        components.healthBarFill.fillAmount = fill;
    }

    public void UpdateFoodBar(float fill) {
        components.foodBarFill.fillAmount = fill;
    }

    public void UpdateExperienceBar(float fill) {
        components.experienceBarFill.fillAmount = fill;
    }

    public void UpdateLevelText(int i) {
        components.experienceText.text = LocalizationManager.instance.GetLocalizedValue(levelPreset, i); //string.Format(LocalizationManager.instance.GetLocalizedValue(levelPreset), i);
    }
}
