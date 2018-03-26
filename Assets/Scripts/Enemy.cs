using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    float timer;

    Vector3 target;
    Vector3 velocity;

    public float health;
    public float maxHealth;

    public float maxSpeed;

    public Transform targetWolf;

    public float attackDamage;
    public float attackSpeed;

    void Start () {
        target = transform.position;
        health = maxHealth;
    }

	void Update () {
        if (targetWolf != null) {
            target = targetWolf.GetComponent<GroupMovement>().centerOfWolves;
            if (Vector3.Distance(transform.position, target) < 2) {
                timer += Time.deltaTime;
                if (timer >= attackSpeed) {
                    timer -= attackSpeed;
                    targetWolf.GetComponent<GroupMovement>().Attack(attackDamage);
                }
            }
        } 
        else {
            timer += Time.deltaTime;
            if (timer >= 4) {
                timer -= 4;

                Vector2 v2 = Random.insideUnitCircle * maxSpeed;
                Vector3 v3 = new Vector3(v2.x, 0, v2.y);

                target = transform.position + v3;
            }
        }

        velocity = target - transform.position;
        velocity = Vector3.ClampMagnitude(velocity, 1);
        velocity = Vector3.ClampMagnitude(velocity * Time.deltaTime * maxSpeed, maxSpeed);
        transform.position += velocity;

        Vector3 look = velocity;
        look.y = 0;
        if (look != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(look);
        }

        Ray ray = new Ray(transform.position + (Vector3.up * 3), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10, LayerMask.GetMask("Water"))) {
            Vector3 v3 = transform.position;
            v3.y = hit.point.y;
            transform.position = v3;
        }
    }

    public bool Damage(float amount) {
        health -= amount;
        if (health < 0) {
            EnemyManager.instance.Remove(this);
            GroupMovement.instance.KillEnemy();
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
