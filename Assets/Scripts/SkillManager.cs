using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    void Start () {
		
	}

    void Update () {
		
	}
}

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Skill", order = 1)]
public class Skill : ScriptableObject {
    public string unlocalizedName;
    public SkillType skillType;
}

public enum SkillType { A, B}
