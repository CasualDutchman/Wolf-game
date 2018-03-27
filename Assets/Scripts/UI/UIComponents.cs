using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponents : MonoBehaviour {

    [Header("HUD")]
    public Transform screenHUD;
    public Image healthBarFill;
    public Image foodBarFill;
    public Image experienceBarFill;
    public Text experienceText;

    [Header("Wolf Pack Screen")]
    public Transform screenWolfPack;

    [Header("Alpha Screen")]
    public Transform screenAlpha;

    [Header("Settings Screen")]
    public Transform screenSettings;
}
