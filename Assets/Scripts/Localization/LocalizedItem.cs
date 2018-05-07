using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedItem : MonoBehaviour {

    Text _text;
    public string key;
    string value;

    object[] pars;

    public void Register(string k) {
        _text = GetComponent<Text>();
        key = k;
    }

    public void Change() {
        value = LocalizationManager.instance.GetLocalizedValue(key);
        if (pars != null && pars.Length > 0)
            value = string.Format(value, pars);
        _text.text = value;
    }

    public void UpdateItem(params object[] par) {  
        _text.text = string.Format(value, par);
        pars = par;
    }
}
