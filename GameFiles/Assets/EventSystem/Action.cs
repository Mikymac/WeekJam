using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public bool active;

    public virtual void Activate ()
    {
        return;
    }

    public virtual void Deactivate ()
    {
        return;
    }
}
