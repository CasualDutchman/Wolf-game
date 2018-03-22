﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupMovement : MonoBehaviour {

    public static GroupMovement instance;

    WolfPack wolfPack;

    public GameObject wolfPrefab;

    public Transform cameraRig;

    [HideInInspector]
    public Vector3[] relativePositions;
    [HideInInspector]
    public Vector3[] newRelativePositions;
    [HideInInspector]
    public Vector3[] oldRelativePositions;
    [HideInInspector]
    public Transform[] wolves;

    Enemy attackingEnemy;

    public int amountOfWolves;

    float timer;
    float attackTimer;
    float repositionTimer;
    bool reposition;

    public float maxSpeed = 3;

    [HideInInspector]
    public Vector3 centerOfWolves;

    Vector3 velocity;

    Vector3 begin;
    Vector3 hold;
    Vector3 dir;

    public Image knob;

    public bool mouseControl = true;

    void Awake() {
        instance = this;

    }

    void Start () {
        wolfPack = GetComponent<WolfPack>();

        relativePositions = new Vector3[amountOfWolves];
        newRelativePositions = new Vector3[amountOfWolves];
        oldRelativePositions = new Vector3[amountOfWolves];
        wolves = new Transform[amountOfWolves];
        for (int i = 0; i < amountOfWolves; i++) {
            Vector2 v2 = Random.insideUnitCircle * maxSpeed;
            newRelativePositions[i] = new Vector3(v2.x, 0, v2.y);
            //newRelativePositions[i] = oldRelativePositions[i];

            GameObject go = Instantiate(wolfPrefab, RelativePositionToWorld(i), Quaternion.identity);
            go.GetComponent<WolfMovement>().id = i;
            go.GetComponent<WolfMovement>().maxSpeed = Random.Range(maxSpeed - 0.3f, maxSpeed + 0.3f);

            wolves[i] = go.transform;

            reposition = true;
        }
	}
	
	void Update () {
        if (mouseControl) {
            if (Input.GetMouseButtonDown(0)) {
                begin = Input.mousePosition;
                knob.enabled = true;
                knob.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0)) {
                hold = Input.mousePosition;
                dir = hold - begin;
                dir = Vector3.ClampMagnitude(dir * 0.1f, 5);

                transform.Translate(new Vector3(dir.x, 0, dir.y) * Time.deltaTime, Space.World);
            }

            if (Input.GetMouseButtonUp(0)) {
                knob.enabled = false;
                transform.position = centerOfWolves;
            }
        } 
        else {
            velocity = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * maxSpeed, 0, Input.GetAxis("Vertical") * Time.deltaTime * maxSpeed);


            if (velocity.normalized.magnitude < 0.3f && Vector3.Distance(transform.position, centerOfWolves) > 4f) {
                transform.position = centerOfWolves;
            } else {
                transform.position += velocity;
            }
        }

        centerOfWolves = new Vector3();
        foreach (var item in wolves) {
            centerOfWolves += item.position;
        }
        centerOfWolves /= wolves.Length;
        cameraRig.position = centerOfWolves;

        timer += Time.deltaTime;
        if (timer > 10) {
            timer -= 10;
            for (int i = 0; i < amountOfWolves; i++) {
                Vector2 v2 = Random.insideUnitCircle * 3;
                newRelativePositions[i] = new Vector3(v2.x, 0, v2.y);
                reposition = true;
            }
        }

        if (reposition) {
            repositionTimer += Time.deltaTime * 0.15f;

            for (int i = 0; i < amountOfWolves; i++) {
                Vector3 v3 = Vector3.Lerp(oldRelativePositions[i], newRelativePositions[i], repositionTimer);
                relativePositions[i] = v3;
            }

            if (repositionTimer >= 1) {
                repositionTimer = 0;
                oldRelativePositions = newRelativePositions;
                newRelativePositions = new Vector3[amountOfWolves];
                reposition = false;
            }
        }

        if (EnemyManager.instance != null) {
            Enemy enemy = EnemyManager.instance.FromPosition(centerOfWolves, 5);

            if (enemy != null && enemy.targetWolf == null) {
                enemy.targetWolf = transform;
                attackingEnemy = enemy;
            }
        }

        if (attackingEnemy != null) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= wolfPack.attackSpeed) {
                attackTimer = 0;
                if (attackingEnemy.Damage(wolfPack.attackDamage)) {
                    attackingEnemy = null;
                }
            }
        }

        wolfPack.atRestingPlace = RestingManager.instance.RestingAreaAtPosition(centerOfWolves, 3) != null;
	}

    public void Attack(float damage) {
        wolfPack.Damage(damage);
    }

    public void AddFood(float amount) {
        wolfPack.ChangeFood(amount);
    }

    public void KillEnemy() {
        wolfPack.ChangeFood(15);
        wolfPack.AddXP(25);
    }

    public Vector3 RelativePositionToWorld(int id) {
        return transform.position + relativePositions[id];
    }
}
