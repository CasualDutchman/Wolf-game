using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonChanging : MonoBehaviour {

    public Material material;
    public float changeSeasonTime = 5;

    float timer;

	void Start () {
		
	}
	
	void Update () {
        float one = 1 / changeSeasonTime;
        timer += Time.deltaTime * one;

        if (timer >= 4) {
            timer = 0;
        }

        material.SetFloat("_Blend", timer);
	}
}
