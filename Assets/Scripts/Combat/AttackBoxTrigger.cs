using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoxTrigger : MonoBehaviour
{

    [SerializeField]
    GameObject AttackTarget;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Attackable"))
        {
            AttackTarget = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == AttackTarget)
        {
            AttackTarget = null;
        }
    }

    public GameObject GetTarget()
    {
        return AttackTarget;
    }

    void OnEnable()
    {
        AttackTarget = null;
    }

}
