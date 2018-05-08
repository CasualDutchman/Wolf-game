using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType { Raccoon, Fox, Coyote, Jackal, Dog, Cougar, Tiger, Bear, Grizzly, Reindeer, Moose, Bison, Muskox }

public class Enemy : MonoBehaviour {

    Animator anim;

    float timer;

    Vector3 target;
    Vector3 velocity;

    public AnimalType animalType = AnimalType.Fox;

    public float health;
    public float maxHealth;

    public float maxSpeed;

    public int level = 1;

    public Transform targetWolf;

    public float attackDamage;
    public float attackSpeed;

    void Start () {
        anim = GetComponent<Animator>();

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
                    targetWolf.GetComponent<GroupMovement>().Attack(attackDamage, level);
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

        Vector3 dir = target - transform.position;
        velocity += dir.normalized * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        anim.SetFloat("Blend", velocity.magnitude / maxSpeed);

        //velocity = Vector3.ClampMagnitude(velocity, 1);
        //velocity = Vector3.ClampMagnitude(velocity * Time.deltaTime * maxSpeed, maxSpeed);
        transform.position += velocity;

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
    }

    public bool Damage(float amount) {
        health -= amount;
        if (health < 0) {
            EnemyManager.instance.Remove(this);
            GroupMovement.instance.KillEnemy();
            SkillManager.instance.KillAnimal(animalType);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
