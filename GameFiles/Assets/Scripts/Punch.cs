using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i == null)
            return;
        Debug.Log("isWorking");
        i.OnHit(transform.position);
    }
}
