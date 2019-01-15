using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Crate : Interactable
{
    public override void OnHit(Vector3 point)
    {
        GetComponent<Rigidbody>().AddExplosionForce(1000F, point, 4F);
        base.OnHit(point);
    }
}