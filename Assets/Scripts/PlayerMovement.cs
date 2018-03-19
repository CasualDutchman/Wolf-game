using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;

    Vector3 velocity = Vector3.forward;

    float timer;

	void Update () {
        transform.position += velocity * Time.deltaTime * speed;// new Vector3(Time.deltaTime * speed, 0, Time.deltaTime * speed);

        timer += Time.deltaTime;
        if (timer >= 10) {
            velocity = new Vector3(-1 + (Random.value * 2), 0, -1 + (Random.value * 2));
            velocity.Normalize();
            timer = 0;
        }

        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10)) {
            Vector3 v3 = transform.position;
            v3.y = hit.point.y;
            transform.position = v3;
        }
	}
}
