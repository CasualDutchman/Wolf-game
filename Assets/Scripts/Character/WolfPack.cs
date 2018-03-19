using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WolfPack : MonoBehaviour {

    public float health;
    public float maxhealth;
    public Image healthbar;

    public float food;
    public float maxFood;
    public Image foodbar;
    public float foodConsumptionInterval;
    public float foodConsumption;

    public int level;
    public Text levelText;

    public Image experienceBar;
    public int experience;
    public int maxExperience;

    float foodTimer;

    public bool atRestingPlace;

	void Start () {
        health = maxhealth;
        food = maxFood;

        UpdateTexts();
        UpdateExperienceTexts();
    }
	
	void Update () {
        foodTimer += Time.deltaTime;
        if (foodTimer >= foodConsumptionInterval) {
            foodTimer -= foodConsumptionInterval;
            food -= foodConsumption;

            if (health < maxhealth && atRestingPlace) {
                health += 5;
            }

            UpdateTexts();
        }

        if (health < maxhealth) {
            foodConsumption = atRestingPlace ? 4.5f : 0.3f;
            foodConsumptionInterval = atRestingPlace ? 1f : 1.7f;
        } else {
            foodConsumption = 0.1f;
            foodConsumptionInterval = atRestingPlace ? 5f : 3f;
        }
	}

    public void UpdateTexts() {
        healthbar.fillAmount = health / maxhealth;
        foodbar.fillAmount = food / maxFood;
        //healthText.text = health + "/" + maxhealth;
        //foodText.text = food.ToString("F1") + "/" + maxFood;
    }

    public void UpdateExperienceTexts() {
        levelText.text = "Level: " + level.ToString();
        experienceBar.fillAmount = (float)experience / (float)maxExperience;
        //xpText.text = experience.ToString();
    }

    public void Damage(float amount) {
        health -= amount;

        UpdateTexts();
    }

    public void ChangeFood(float amount) {
        food = Mathf.Clamp(food + amount, 0, maxFood);

        UpdateTexts();
    }

    public void AddXP(int amount) {
        experience += amount;

        if (experience >= maxExperience) {
            level++;
            experience -= maxExperience;
        }

        UpdateExperienceTexts();
    }
}
