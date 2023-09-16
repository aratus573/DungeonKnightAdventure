using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public AudioControl Audio;
    // private NavMeshAgent agent;
    CharacterStats characterStats;
    private Animator anim;
    public GameObject attackTarget;
    private AttackData_SO attackData;
    private Quaternion targetRotation;
    public AttackBoxTrigger AttackBox;
    //private float lastAttackTime;
    [HideInInspector]
    public bool isDead;
    //private float stopDistance;

    void Awake()
    {
        // agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        Audio = GameObject.FindWithTag("Audio").GetComponent<AudioControl>();
        //stopDistance = agent.stoppingDistance;
    }

    void Start()
    {
        //MouseManager.Instance.OnMouseClicked += MoveToTarget;
        //MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RigisterPlayer(characterStats);
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
        {
            //TODO : Player Die Audio
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();

        // lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        // anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead)
            return;

        //agent.stoppingDistance = stopDistance;
        // agent.isStopped = false;
        // agent.destination = target;
    }
    private void EventAttack(GameObject target)
    {
        if (isDead)
            return;

        if (target != null)
        {
            attackTarget = target;
            // StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        // agent.isStopped = false;
        // agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);

        //need to change to variable
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > 1)
        {
            // agent.destination = attackTarget.transform.position;
            yield return null;
        }

        // agent.isStopped = true;
        //Attack

        /* attack CD time define in InputManager & PlayerLocomotion
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = 0.5f;
        }
        */
    }


    void FoundEnemy()
    {
        /*
        // judge radius
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(0,1,1) , new Vector3(1.5f,2,1.5f), transform.rotation);
       
        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy") || target.CompareTag("Attackable"))
            {
                attackTarget = target.gameObject;
                Hit();
                //stop hitting multiple target
                break;
            }
        }
        */

        attackTarget = AttackBox.GetTarget();
        Hit();
        if(attackTarget == null)
            Audio.SwordMiss();
        else
            Audio.SwordHit();
    }

    public void Hit()
    {

        if (attackTarget != null) {
            
            if (attackTarget.CompareTag("Attackable"))
            {
                // video core 26 13:00 ~ 13:15
                if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
                {
                    attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                    //attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                    attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * attackTarget.GetComponent<Rock>().back_force, ForceMode.Impulse);
                }
            }
            else
            {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
            }
        }
    }
    public void Hit(int damage)
    {

        if (attackTarget != null)
        {

            if (attackTarget.CompareTag("Attackable"))
            {
                // video core 26 13:00 ~ 13:15
                if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
                {
                    attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                    //attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                    attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * attackTarget.GetComponent<Rock>().back_force, ForceMode.Impulse);
                }
            }
            else
            {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(damage, targetStats);
            }
        }
    }

}
