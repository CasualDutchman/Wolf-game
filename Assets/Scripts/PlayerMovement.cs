using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    Vector3 begin;
    Vector3 hold;
    Vector3 dir;

    public Image knob;

    public float speed;

    Vector3 velocity = Vector3.forward;

    float timer;

    public bool mouseControl = true;

	void Update () {
        if (!mouseControl) {
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
        } else {
            if (Input.GetMouseButtonDown(0)) {
                begin = Input.mousePosition;
                knob.enabled = true;
                knob.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0)) {
                hold = Input.mousePosition;
                dir = hold - begin;
                dir *= 0.01f;

                transform.Translate(dir * Time.deltaTime, Space.World);
            }

            if (Input.GetMouseButtonUp(0)) {
                knob.enabled = false;
            }
        }
    }
}
