using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 7;

    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            Vector3 direction = (attackTarget.transform.position - transform.position);

            direction = direction.normalized;

            attackTarget.GetComponent<InputManager>().OnDisable();
            attackTarget.GetComponent<Rigidbody>().AddForce(direction * kickForce, ForceMode.Impulse);
            attackTarget.GetComponent<InputManager>().OnEnable();
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
