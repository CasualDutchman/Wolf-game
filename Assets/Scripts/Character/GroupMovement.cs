using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupMovement : MonoBehaviour {

    public static GroupMovement instance;

    WolfPack wolfPack;
    public UIManager uiManager;
    public GameObject wolfPrefab;

    public Transform cameraRig;

    public Vector3[] relativePositions;
    public Vector3[] newRelativePositions;
    public Vector3[] oldRelativePositions;
    [HideInInspector]
    public List<Transform> wolves;

    public int amountOfWolves;

    public bool isAttacking = false;

    float timer;
    //float attackTimer;
    float repositionTimer;
    bool reposition;

    public AnimationCurve repositionCurve;
    public AnimationCurve lerpCurve;

    public float maxSpeed = 3;
    float stopTime = 0;
    Vector3 tempDir;

    public Vector3 centerOfWolves;

    Vector3 velocity;

    Vector3 begin;
    Vector3 hold;
    Vector3 dir;

    public Image knob;

    bool uiHit = false;

    float treeTimer;

    void Awake() {
        instance = this;
    }

    void Start () {
        wolfPack = GetComponent<WolfPack>();

        relativePositions = new Vector3[amountOfWolves];
        newRelativePositions = new Vector3[amountOfWolves];
        oldRelativePositions = new Vector3[amountOfWolves];
        for (int i = 0; i < amountOfWolves; i++) {
            Vector2 v2 = Random.insideUnitCircle * maxSpeed;
            newRelativePositions[i] = new Vector3(v2.x, 0, v2.y);
            //newRelativePositions[i] = oldRelativePositions[i];

            GameObject go = Instantiate(wolfPrefab, RelativePositionToWorld(i), Quaternion.identity);
            WolfMovement wm = go.GetComponent<WolfMovement>();
            wm.id = i;
            wm.maxSpeed = maxSpeed;
            wm.health = wolfPack.maxhealth;
            wm.maxHealth = wolfPack.maxhealth;
            wm.groupMovement = this;
            wm.attackDamage = wolfPack.attackDamage;
            wm.attackSpeed = wolfPack.attackSpeed;

            wolves.Add(go.transform);

            reposition = true;
        }

	}
	
	void Update () {
        if (uiManager.IsHUD()) {
            if (Input.GetMouseButtonDown(0)) {
                uiHit = uiManager.IsHittingUI();

                if (!uiHit) {
                    begin = Input.mousePosition;
                    knob.enabled = true;
                    knob.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                }
            }

            if (Input.GetMouseButton(0) && !uiHit) {
                hold = Input.mousePosition;
                dir = hold - begin;
                Vector2 part = new Vector2(dir.x / (float)Display.main.systemWidth, dir.y / (float)Display.main.systemHeight);
                dir = Vector3.ClampMagnitude(dir * (part.magnitude * .1f), maxSpeed);

                transform.Translate(new Vector3(dir.x, 0, dir.y) * Time.deltaTime, Space.World);
                for (int i = 0; i < wolves.Count; i++) {
                    wolves[i].GetComponent<WolfMovement>().velocity = new Vector3(dir.x, 0, dir.y);
                }

                knob.transform.localEulerAngles = new Vector3(0, 0, -Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y)).eulerAngles.y);
            }

            if (Input.GetMouseButtonUp(0) && !uiHit) {
                hold = Input.mousePosition;
                dir = hold - begin;
                Vector2 part = new Vector2(dir.x / (float)Display.main.systemWidth, dir.y / (float)Display.main.systemHeight);
                dir = Vector3.ClampMagnitude(dir * (part.magnitude * .1f), maxSpeed);

                knob.enabled = false;
                transform.position = centerOfWolves;

                for (int i = 0; i < wolves.Count; i++) {
                    relativePositions[i] = wolves[i].position - centerOfWolves;
                    oldRelativePositions = relativePositions;
                    reposition = false;
                    timer = 0;
                }
                stopTime = 1;
                tempDir = dir;
            }
        }

        if (stopTime > 0 && !Input.GetMouseButton(0)) {
            stopTime -= Time.deltaTime;
            if (stopTime > 0) {
                dir = tempDir * stopTime;
                transform.Translate(new Vector3(dir.x, 0, dir.y) * Time.deltaTime, Space.World);
                for (int i = 0; i < wolves.Count; i++) {
                    relativePositions[i] = wolves[i].position - centerOfWolves;
                    oldRelativePositions = relativePositions;
                    wolves[i].GetComponent<WolfMovement>().velocity = new Vector3(dir.x, 0, dir.y);
                }
            }
        }

        timer += Time.deltaTime;
        if (timer > 10) {
            timer -= 10;
            for (int i = 0; i < amountOfWolves; i++) {
                Vector2 v2 = Random.insideUnitCircle * 3f;
                newRelativePositions[i] = new Vector3(v2.x, 0, v2.y);
                if (isAttacking) {
                    newRelativePositions[i] = newRelativePositions[i].normalized * 3f;
                }
                reposition = true;
            }
        }

        if (reposition) {
            repositionTimer += Time.deltaTime * 0.15f;

            for (int i = 0; i < amountOfWolves; i++) {
                Vector3 v3 = Vector3.Lerp(oldRelativePositions[i], newRelativePositions[i], lerpCurve.Evaluate(repositionTimer));
                relativePositions[i] = v3;

                if (!Input.GetMouseButton(0)) {
                    Vector3 v = newRelativePositions[i] - oldRelativePositions[i];
                    v = Vector3.ClampMagnitude(v, repositionCurve.Evaluate(repositionTimer) * 2);
                    wolves[i].GetComponent<WolfMovement>().velocity = v;
                }
            }

            if (repositionTimer >= 1) {
                repositionTimer = 0;
                oldRelativePositions = newRelativePositions;
                newRelativePositions = new Vector3[amountOfWolves];
                reposition = false;
            }
        }

        /*
        if (EnemyManager.instance != null) {
            Enemy enemy = (Enemy)EnemyManager.instance.FromPosition(centerOfWolves, 5);

            if (enemy != null && enemy.targetWolf == null) {
                enemy.targetWolf = transform;
                attackingEnemy = enemy;
            }
        }*

        if (attackingEnemy != null) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= wolfPack.attackSpeed) {
                attackTimer = 0;
                if (attackingEnemy.Damage(wolfPack.attackDamage)) {
                    attackingEnemy = null;
                }
            }
        }*/

        centerOfWolves = new Vector3();
        foreach (var item in wolves) {
            centerOfWolves += item.position;
        }
        centerOfWolves /= wolves.Count;
        cameraRig.position = Vector3.Lerp(cameraRig.position, centerOfWolves, 0.7f);

        //wolfPack.atRestingPlace = ((Transform[])RestingManager.instance.FromPosition(centerOfWolves, 3)).Length > 0;
    }

    public void Attack(float damage, int level) {
        wolfPack.Damage(damage, level);
    }

    public void UpdateHealthBar() {
        float max = 0;
        float current = 0;
        foreach (Transform item in wolves) {
            WolfMovement wolf = item.GetComponent<WolfMovement>();
            max += wolf.maxHealth;
            current += wolf.health;
        }
        max /= (float)wolves.Count;
        current /= (float)wolves.Count;

        uiManager.UpdateHealthBar(current / max);
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
