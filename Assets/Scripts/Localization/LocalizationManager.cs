using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;

    Dictionary<string, string> dictionary = new Dictionary<string, string>();//1. key, code to actual language // 2. value, display language

    string loadedlanguage = "EN_us";

    string missingKeyString = "Missing Localized Info";

    public bool debugAll = false;

    void Awake() {
        instance = this;

        if (PlayerPrefs.HasKey("Language")) {
            loadedlanguage = PlayerPrefs.GetString("Language");
        }
        StartCoroutine(LoadDictionary(loadedlanguage));
    }

    public void OnChangeLanguagePref(string lang) {
        PlayerPrefs.SetString("Language", lang);
        PlayerPrefs.Save();
        StartCoroutine(LoadDictionary(lang));
    }

    public string GetLocalizedValue(string key, params object[] arr) {
        string result = missingKeyString + ". key = " + key;

        if (dictionary.ContainsKey(key)) {
            result = dictionary[key];
        }

        if (arr.Length > 0)
            return string.Format(result, arr);
        else
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
                    Debug.Log(string.Format("{0} || {1} , {2}", data, keyValue[0], keyValue[1]));
                }
            }

            Debug.Log("Localized content loaded. Dictionary contains " + dictionary.Count + " entries");
        } else {
            Debug.LogError(lang + ".txt does not excist in the StreamingAssets folder.");
        }

        yield return 0; 
    }
}
