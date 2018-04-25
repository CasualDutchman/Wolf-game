using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponents : MonoBehaviour {

    [Header("Screens")]
    public GameObject screenHUD;
    public GameObject screenAlpha, screenSettings;

    [Header("HUD")]
    public Transform imageHealthFill;
    public Transform imageFoodBarFill;
    public Transform imageExperienceBarFill;
    public Transform textExperience;
    public Transform buttonStore;
    public Transform buttonSettings;
    public Transform buttonSide;
    public Transform buttonAlpha;

    //[Header("Wolf Pack Screen")]

    [Header("Alpha Screen")]
    public Transform textAlphaTitle;
    public Transform skillA1;
    public Transform skillA2;
    public Transform skillB1;
    public Transform skillB2;
    public Transform skillB3;
    public Transform buttonBackAlpha;

    [Header("Settings Screen")]
    public Transform textSettingsTitle;
    public Transform textGraphical;
    public Transform textGraphicalOnOff;
    public Transform textAudio;
    public Transform textAudioOnOff;
    public Transform buttonGraphics;
    public Transform buttonAudio;
    public Transform buttonBackSettings;
    public Transform buttonEnglish;
    public Transform buttonDutch;
}
