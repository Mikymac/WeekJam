using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnyBoi : MonoBehaviour {

    public Vector3 Spinny;

    public int maxAng, minAng;
    bool dir;
    Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(Spinny);
        if (dir)
        {
            cam.fieldOfView += Time.fixedDeltaTime * 2;
            if (cam.fieldOfView >= maxAng)
            {
                dir = !dir;
            }
        } else
        {
            cam.fieldOfView -= Time.fixedDeltaTime * 2;
            if (cam.fieldOfView <= minAng)
            {
                dir = !dir;
            }
        }
    }
}
