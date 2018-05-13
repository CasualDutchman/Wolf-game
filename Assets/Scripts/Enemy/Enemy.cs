using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AnimalType { Raccoon, Fox, Coyote, Jackal, Dog, Cougar, Tiger, Bear, Grizzly, Reindeer, Moose, Bison, Muskox }

public enum Behavior { Attack, Flee }

public class Enemy : MonoBehaviour {

    Animator anim;

    public Transform cameraRig;

    public Transform canvas;
    Image healthbar;

    float timer;

    Vector3 target;
    Vector3 velocity;

    public AnimalType animalType = AnimalType.Fox;
    public Behavior behavior = Behavior.Attack;

    public float health;
    public float maxHealth;

    public float maxSpeed;

    public int level = 1;

    public WolfMovement targetWolf;
    bool chase = false;
    bool seeWolf = false;
    Vector3 lastKnowLocation;
    float lostTimer = 0;

    float attackTimer;
    public float attackDamage;
    public float attackSpeed;

    bool canDie = false;

    void Start () {
        anim = GetComponent<Animator>();

        healthbar = canvas.GetChild(1).GetComponent<Image>();

        target = transform.position;
        health = maxHealth;
    }

	void Update () {
        if(seeWolf == true && chase == true) {
            if (targetWolf != null) {
                if (behavior == Behavior.Attack) {
                    target = targetWolf.groupMovement.centerOfWolves;
                    if (Vector3.Distance(transform.position, target) < 2) {
                        timer += Time.deltaTime;
                        if (timer >= attackSpeed) {
                            timer -= attackSpeed;
                            targetWolf.groupMovement.Attack(attackDamage, level);

                        }
                    }
                } else {
                    target = transform.position - targetWolf.transform.position;
                    target = target.normalized * 20;
                }
            }
        }
        else if (seeWolf == false && chase == true) {
            lostTimer += Time.deltaTime;
            if (lostTimer >= 3) {
                chase = false;
                lostTimer = 0;
            }

            if (behavior == Behavior.Attack) {
                target = lastKnowLocation;
            } else {
                target = transform.position - lastKnowLocation;
                target = target.normalized * 20;
            }
        } 
        else if(seeWolf == false && chase == false) {
            timer += Time.deltaTime;
            if (timer >= 4) {
                timer -= 4;

                Vector2 v2 = Random.insideUnitCircle * maxSpeed;
                Vector3 v3 = new Vector3(v2.x, 0, v2.y);

                target = transform.position + v3;
            }
        }

        Vector3 dir = target - transform.position;
        velocity += dir.normalized * Time.deltaTime;

        float dis = Vector3.Distance(target, transform.position);
        float multiplier = behavior == Behavior.Attack ? (dis > maxSpeed ? 1 : dis / maxSpeed) : 1.3f;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed * multiplier);

        anim.SetFloat("Blend", velocity.magnitude / (maxSpeed - 1));

        transform.position += velocity * Time.deltaTime;

        Vector3 look = velocity;
        look.y = 0;
        if (look != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(look);
        }

        Ray ray = new Ray(transform.position + (Vector3.up * 7), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10, LayerMask.GetMask("Water"))) {
            Vector3 v3 = transform.position;
            v3.y = hit.point.y;
            transform.position = v3;
        }

        canvas.localEulerAngles = new Vector3(0, -transform.eulerAngles.y, 0);

        if (targetWolf != null && behavior == Behavior.Attack) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed) {
                attackTimer = 0;
                if (targetWolf.Damage(attackDamage)) {
                    targetWolf = null;
                }
            }
        }

        float diss = Vector3.Distance(transform.position, cameraRig.position);

        if (canDie || diss > 50) {
            EnemyManager.instance.Remove(this);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(target, 1f);
    }

    public bool Damage(float amount) {
        health -= amount;
        if (health < maxHealth && canvas != null) {
            canvas.gameObject.SetActive(true);
            healthbar.fillAmount = health / maxHealth;
        }

        if (health < 0) {
            canDie = true;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Wolf"))) {
            targetWolf = other.GetComponent<WolfMovement>();
            chase = true;
            seeWolf = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Wolf"))) {
            lastKnowLocation = other.transform.position;
            targetWolf = null;
            seeWolf = false;
        }
    }
}
