using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;

    UIManager uiManager;

    Dictionary<string, string> dictionary = new Dictionary<string, string>();//1. key, code to actual language // 2. value, display language

    public string loadedlanguage = "EN_us";
    public TextAsset att;

    public bool debugAll = false;

    void Awake() {
        instance = this;
        uiManager = GetComponent<UIManager>();

        if (PlayerPrefs.HasKey("Language")) {
            loadedlanguage = PlayerPrefs.GetString("Language");
        }
        StartCoroutine(LoadDictionary(loadedlanguage));
    }

    public void OnChangeLanguagePref(string lang) {
        PlayerPrefs.SetString("Language", lang);
        PlayerPrefs.Save();
        StartCoroutine(LoadDictionary(lang));

        uiManager.OnChangeLanguagePref();
    }

    public string GetLocalizedValue(string key) {
        string result = "Missing Localized Info. key = " + key;

        if (dictionary.ContainsKey(key)) {
            result = dictionary[key];
        }

        return result;
    }

    IEnumerator LoadDictionary(string lang) {
        dictionary = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, lang + ".txt");

        if (File.Exists(filePath)) {
            string[] dataArray = File.ReadAllLines(filePath);
            foreach (string data in dataArray) {
                if (data.StartsWith("/") || string.IsNullOrEmpty(data))
                    continue;

                string[] keyValue = data.Split('=');
                dictionary.Add(keyValue[0], keyValue[1]);
                if (debugAll) {
                    #if (UNITY_EDITOR)
                    Debug.Log(string.Format("{0} || {1} , {2}", data, keyValue[0], keyValue[1]));
                    #endif
                }
            }

            #if (UNITY_EDITOR)
            Debug.Log("Localized content loaded. Dictionary contains " + dictionary.Count + " entries");
            #endif
        } else {
            #if (UNITY_EDITOR)
            Debug.LogError(lang + ".txt does not excist in the StreamingAssets folder.");
            #endif
        }

        yield return 0; 
    }
}
