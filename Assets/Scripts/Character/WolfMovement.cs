using System.Collections;
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

        transform.rotation = Quaternion.LookRotation(velocity);
    }
}
