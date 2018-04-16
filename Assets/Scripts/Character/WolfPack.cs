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
    public int experience;
    public int maxExperience;

    //[Header("UI")]
    //public Image healthbar;
    //public Image foodbar;
    //public Text levelText;
    //public Image experienceBar;

    [Header("Misc")]
    public float foodConsumptionInterval;
    public float foodConsumption;

    float foodTimer;

    public bool atRestingPlace;

	void Start () {
        health = maxhealth;
        food = maxFood;

        uimanager = UIManager.instance;

        Updatebars();
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

    public void Damage(float amount) {
        health -= amount;

        Updatebars();
    }

    public void ChangeFood(float amount) {
        food = Mathf.Clamp(food + amount, 0, maxFood);

        Updatebars();
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
