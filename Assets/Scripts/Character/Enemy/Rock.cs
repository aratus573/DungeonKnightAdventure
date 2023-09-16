using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates{HitPlayer, HitEnemy, HitNothing};
    private Rigidbody rb;
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;
    public float back_force;
    public int damage;
    public GameObject target;
    public float destroyTime;
    public float remainDestroyTime;
    private Vector3 direction;
    public GameObject breakEffect;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        remainDestroyTime = destroyTime;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void Update()
    {
        remainDestroyTime -= Time.deltaTime;
        if (remainDestroyTime <= 0f)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        ////Debug.Log(rb.velocity.sqrMagnitude);/
        if(rb.velocity.sqrMagnitude < 1f && rockStates == RockStates.HitPlayer)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if(target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position + new Vector3(0, 0.5f, 0)).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<InputManager>().OnDisable();
                    other.gameObject.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
                    other.gameObject.GetComponent<InputManager>().OnEnable();
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
