using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settingsmanager : MonoBehaviour {

    public static Settingsmanager instance;

    public Graphical graphicalSettings;
    public OnOff audioSettings;

    bool settingsLoaded = false;

    //graphical
    public Camera treeCamera;
    public Material treeMaterial, treeMaterialAlpha;
    RenderTexture RT;
    int rtSize = 1024;

    Vector2Int nativeResolution;

    //audio
    public AudioMixer mixer;
    public GameObject audioPrefab;

    void Awake() {
        nativeResolution = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);

        instance = this;

        treeCamera.enabled = false;
        ChangeTreeCamera(1);
    }

    public void EnableSettings() {
        settingsLoaded = true;
    }

    public void DisableSettings() {
        settingsLoaded = false;
    }

    public void OnToggleAudio(bool b, string str) {
        if (settingsLoaded) {
            audioSettings = b ? OnOff.On : OnOff.Off;
            SetAudio();
            PlayerPrefs.SetInt(str, b ? 0 : 1);
            PlayerPrefs.Save();
        }
    }

    public void SetAudio() {

    }

    public void OnToggleGraphical(bool b, string str) {
        if (settingsLoaded) {
            graphicalSettings = b ? Graphical.High : Graphical.Low;
            SetGraphical();
            PlayerPrefs.SetInt(str, b ? 0 : 1);
            PlayerPrefs.Save();
        }
    }

    public void SetGraphical() {
        QualitySettings.SetQualityLevel(graphicalSettings == Graphical.High ? 3 : 1);
        ChangeTreeCamera(graphicalSettings == Graphical.High ? 1 : 0.75f);

        float scale = graphicalSettings == Graphical.High ? 1 : 0.85f;
        Screen.SetResolution((int)(nativeResolution.x * scale), (int)(nativeResolution.y * scale), true);
    }

    void ChangeTreeCamera(float scale) {
        RT = new RenderTexture((int)(rtSize * scale), (int)(rtSize * scale), 16);
        RT.Create();
        treeCamera.targetTexture = RT;
        UpdateTreeCamera();
        treeMaterial.mainTexture = RT;
        treeMaterialAlpha.mainTexture = RT;
    }

    public void UpdateTreeCamera() {
        treeCamera.Render();
    }

    public void PlaySound(AudioClip clip) {
        if (audioSettings == OnOff.On) {
            GameObject go = Instantiate(audioPrefab);
            go.GetComponent<AudioSource>().clip = clip;
            go.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.outputAudioMixerGroup;
        }
    }
}

public enum Graphical { High, Low }

public enum OnOff { On, Off }