using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSpell : MonoBehaviour
{
    public AudioControl Audio;
    PlayerLocomotion playerLocomotion;
    PlayerController playerController;
    private GameObject player;
    Vector3 toward;
    public float RangedSpellTime = 3f;

    private void Start()
    {
        //TODO : Spell1 Audio
        Audio = GameObject.FindWithTag("Audio").GetComponent<AudioControl>();
        Audio.Spell1();
        if (player != null)
        {
            toward = playerLocomotion.transform.forward;
        }
    }

    private void Awake()
    {
        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player").gameObject;
            playerLocomotion = player.GetComponent<PlayerLocomotion>();
            playerController = player.GetComponent<PlayerController>();
        }
    }
    private void Update()
    {
        RangedSpellTime -= Time.deltaTime;
        if (player != null) {
            //FoundEnemy();
/*            float distance = Vector3.Distance(transform.position, playerLocomotion.transform.position);
            if (distance > 14)*/
            if(RangedSpellTime <= 0)
                Destroy(gameObject);
            else
                transform.Translate(toward * 3f * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Attackable"))
        {
            playerController.attackTarget = other.gameObject;
            playerController.Hit();
        }
    }

    /*
    void FoundEnemy()
    {
        // judge radius
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(0, 1, 1), new Vector3(1.5f, 2, 1.5f), transform.rotation);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy") || target.CompareTag("Attackable"))
            {
                playerController.attackTarget = target.gameObject;
                playerController.Hit();
                //stop hitting multiple target
                break;
            }
        }
    }

    */
}
