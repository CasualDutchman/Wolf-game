using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WolfMovement : MonoBehaviour {

    public GroupMovement groupMovement;

    public int id;
    public Vector3 velocity;

    public Transform canvas;
    Image healthbar;

    public float health;
    public float maxHealth;
    public float maxSpeed;
    public float attackSpeed;
    public float attackDamage;

    Animator anim;

    public Material mat1, mat2;

    List<Enemy> enemiesToAttack = new List<Enemy>();
    float attackTimer;

    bool atResting = false;
    float restingTimer;

    void Start () {
        anim = GetComponent<Animator>();
        healthbar = canvas.GetChild(1).GetComponent<Image>();
    }
	
	void Update () {
        Vector3 target = GroupMovement.instance.RelativePositionToWorld(id);
        //velocity = (target - transform.position).normalized;
        transform.position = target;

        /*
        velocity = (target - transform.position).normalized;
        anim.SetFloat("Blend", 0.5f * Mathf.FloorToInt(velocity.magnitude * maxSpeed));


        //velocity = Vector3.ClampMagnitude(velocity, 1);
        //anim.SetFloat("Blend", 0.5f * Mathf.FloorToInt(velocity.magnitude * maxSpeed));

        velocity = Vector3.ClampMagnitude(velocity * maxSpeed, maxSpeed);
        transform.position += velocity * Time.deltaTime;
        */
        anim.SetFloat("Blend", velocity.magnitude / maxSpeed);

        Vector3 look = velocity;
        if (enemiesToAttack.Count > 0 && enemiesToAttack[0] != null && !Input.GetMouseButton(0)) {
            look = enemiesToAttack[0].transform.position - transform.position;
        }
        look.y = 0;
        if (look !=  Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(look);
        }

        Ray ray = new Ray(transform.position + (Vector3.up * 10), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20, LayerMask.GetMask("Water"))) {
            Vector3 v3 = transform.position;
            v3.y = hit.point.y;
            transform.position = v3;
        }

        canvas.localEulerAngles = new Vector3(0, -transform.eulerAngles.y, 0);

        groupMovement.isAttacking = enemiesToAttack.Count > 0;

        RemoveEnemy();

        if (atResting) {
            restingTimer += Time.deltaTime;
            if (restingTimer >= 3) {
                if (health < maxHealth) {
                    health = Mathf.Clamp(health + 10, 0, maxHealth);
                    healthbar.fillAmount = health / maxHealth;

                    groupMovement.UpdateHealthBar();
                    groupMovement.AddFood(-5);

                    if (health >= maxHealth) {
                        canvas.gameObject.SetActive(false);
                    }
                } 
                restingTimer = 0;
            }
        }
    }

    void RemoveEnemy() {
        if (enemiesToAttack.Count > 0) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed) {
                attackTimer = 0;
                if (enemiesToAttack[0] != null && enemiesToAttack[0].Damage(attackDamage)) {
                    groupMovement.KillEnemy();
                    SkillManager.instance.KillAnimal(enemiesToAttack[0].animalType);
                    enemiesToAttack.RemoveAt(0);
                    groupMovement.isAttacking = false;
                    return;
                }
            }

            if (enemiesToAttack[0] != null) {
                float dis = Vector3.Distance(enemiesToAttack[0].transform.position, transform.position);
                if (dis > 8) {
                    enemiesToAttack.RemoveAt(0);
                    groupMovement.isAttacking = false;
                    return;
                }
            } else {
                enemiesToAttack.RemoveAt(0);
            }
        }
    }

    public bool Damage(float amount) {
        health -= amount;
        if (health < maxHealth) {
            canvas.gameObject.SetActive(true);
            healthbar.fillAmount = health / maxHealth;
        }

        groupMovement.UpdateHealthBar();

        if (health < 0) {
            print("killed wolf");
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Not Walkable"))) {
            other.GetComponent<Renderer>().material = mat2;
        } 
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
            enemiesToAttack.Add(other.GetComponent<Enemy>());
        } 
        else if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Resting"))) {
            atResting = true;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Not Walkable"))) {
            if(!other.GetComponent<Renderer>().material.Equals(mat2))
                other.GetComponent<Renderer>().material = mat2;
        } 
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
            if (!enemiesToAttack.Contains(other.GetComponent<Enemy>())) {
                enemiesToAttack.Add(other.GetComponent<Enemy>());
            }
        } 
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Resting"))) {
            atResting = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Not Walkable"))) {
            other.GetComponent<Renderer>().material = mat1;
        } 
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
            enemiesToAttack.Remove(other.GetComponent<Enemy>());
        } 
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Resting"))) {
            atResting = false;
        }
    }
}
