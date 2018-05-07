using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WolfPack : MonoBehaviour {

    UIManager uimanager;

    [Header("Wolf Attributes")]
    public float health;
    public float maxhealth;
    public float food;
    public float maxFood;
    public float attackDamage;
    public float attackSpeed;
    public int level;
    public float experience;
    public int maxExperience;

    [Header("Misc")]
    public float foodConsumptionInterval;
    public float foodConsumption;

    float foodTimer;

    public bool atRestingPlace;

    public string wolfBondCode, seasonedHunterCode, shareKnowledgeCode;

	void Start () {
        health = maxhealth;
        food = maxFood;

        uimanager = UIManager.instance;

        Load();

        Updatebars();
        UpdateExperienceTexts();
    }
	
    public void Save() {
        string str = "";
        str += health + "/";
        str += food + "/";
        str += level + "/";
        str += experience;

        PlayerPrefs.SetString("WolfPack", str);
    }

    public void Load() {
        if (PlayerPrefs.HasKey("WolfPack")) {
            string str = PlayerPrefs.GetString("WolfPack");
            string[] data = str.Split('/');
            health = float.Parse(data[0]);
            food = float.Parse(data[1]);
            level = int.Parse(data[2]);
            experience = float.Parse(data[3]);
        }
    }

	void Update () {
        foodTimer += Time.deltaTime;
        if (foodTimer >= foodConsumptionInterval) {
            foodTimer -= foodConsumptionInterval;
            food -= foodConsumption;

            if (health < maxhealth && atRestingPlace) {
                health += 5;
            }

            Updatebars();
        }

        if (health < maxhealth) {
            foodConsumption = atRestingPlace ? 4.5f : 0.3f;
            foodConsumptionInterval = atRestingPlace ? 1f : 1.7f;
        } else {
            foodConsumption = 0.1f;
            foodConsumptionInterval = atRestingPlace ? 5f : 3f;
        }
	}

    public void Updatebars() {
        uimanager.UpdateHealthBar(health / maxhealth);
        uimanager.UpdateFoodBar(food / maxFood);
    }

    public void UpdateExperienceTexts() {
        uimanager.UpdateExperienceBar((float)experience / (float)maxExperience);
        uimanager.UpdateLevelText(level);
    }

    public void Damage(float amount, int animalLevel) {
        if (SkillManager.instance.IsSkillActive(seasonedHunterCode))
            if (animalLevel < level)
                return;

        health -= amount - (SkillManager.instance.IsSkillActive(wolfBondCode) ? 5 : 0);

        Updatebars();
    }

    public void ChangeFood(float amount) {
        food = Mathf.Clamp(food + amount, 0, maxFood);

        Updatebars();
    }

    public void AddXP(float amount) {
        experience += amount + (amount * SkillManager.instance.GetSkillShareAmount(shareKnowledgeCode));

        if (experience >= maxExperience) {
            level++;
            experience -= maxExperience;
        }

        UpdateExperienceTexts();
    }
}
