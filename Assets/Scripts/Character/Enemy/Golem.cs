using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 9;
    public GameObject rockPrefab;
    public Transform handPos;

    //Animation Event
    public void KickOFF()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            Vector3 direction = (attackTarget.transform.position - transform.position);

            direction = direction.normalized;

            attackTarget.GetComponent<Rigidbody>().AddForce(direction * kickForce, ForceMode.Impulse);
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    //Animation Event

    public void ThrowRock()
    {
        //TODO : trebuchet audio
        Audio.EnemyRock();
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
