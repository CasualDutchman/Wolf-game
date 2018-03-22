﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovement : MonoBehaviour {

    public int id;
    public Vector3 velocity;

    public float maxSpeed;

	void Start () {
		
	}
	
	void Update () {
        Vector3 target = GroupMovement.instance.RelativePositionToWorld(id);

        velocity = target - transform.position;
        velocity = Vector3.ClampMagnitude(velocity, 1);
        velocity = Vector3.ClampMagnitude(velocity * Time.deltaTime * maxSpeed, maxSpeed);
        transform.position += velocity;

        Vector3 look = velocity;
        look.y = 0;
        if (look !=  Vector3.zero) {
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
}
