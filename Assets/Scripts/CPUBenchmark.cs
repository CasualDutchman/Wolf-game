using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUBenchmark : MonoBehaviour {

    public UnityEngine.UI.Slider slider;

	void Update () {
        for (int i = 0; i < (int)slider.value; i++) {
            for (int j = 0; j < (int)slider.value; j++) {
                for (int k = 0; k < (int)slider.value; k++) {
                    Vector3 g = Vector3.Lerp(Vector3.one * Mathf.FloorToInt(100 / 5.5f), Vector3.one * Mathf.FloorToInt(100 / 5.5f), 0.5f);
                    Vector3 g1 = Vector3.Lerp(Vector3.one * Mathf.FloorToInt(100 / 5.5f), Vector3.one * Mathf.FloorToInt(100 / 5.5f), 0.5f);
                    Vector3 g2 = Vector3.Lerp(Vector3.one * Mathf.FloorToInt(100 / 5.5f), Vector3.one * Mathf.FloorToInt(100 / 5.5f), 0.5f);
                    Vector3 g3 = Vector3.Lerp(Vector3.one * Mathf.FloorToInt(100 / 5.5f), Vector3.one * Mathf.FloorToInt(100 / 5.5f), 0.5f);

                    g = g.normalized;
                    g1 = g1.normalized;
                    g2 = g2.normalized;
                    g3 = g3.normalized;

                    Vector3 p = g + g1 + g2 + g3;
                    p *= 4;
                }
            }
        }
	}
}
