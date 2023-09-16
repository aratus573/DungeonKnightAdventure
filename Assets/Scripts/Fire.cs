using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public AudioControl Audio;
    PlayerLocomotion playerLocomotion;
    PlayerController playerController;
    private GameObject player;

    float tempTime;

    private void Awake()
    {
        //TODO : Fire Spell Audio
        Audio = GameObject.FindWithTag("Audio").GetComponent<AudioControl>();
        Audio.Spell2();
        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player").gameObject;
            playerLocomotion = player.GetComponent<PlayerLocomotion>();
            playerController = player.GetComponent<PlayerController>();
        }
    }
    private void Update()
    {
        tempTime += Time.deltaTime;

        if (player != null)
        {
            transform.rotation = playerLocomotion.transform.rotation;
            
            playerLocomotion.FireBallTime -= Time.deltaTime;
            if(tempTime > 0.2f)
            {
                FoundEnemy();
                tempTime = 0;
            }
            if (playerLocomotion.FireBallTime <= 0)
            {
                Destroy(gameObject);
                playerLocomotion.FireBallTime = 3;
            }
            else
                transform.position = playerLocomotion.transform.position;
        }
    }

    void FoundEnemy()
    {
        // judge radius
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(0, 1, 1), new Vector3(1.5f, 2, 1.5f), transform.rotation);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy") || target.CompareTag("Attackable"))
            {
                playerController.attackTarget = target.gameObject;
                playerController.Hit(GameManager.Instance.playerStats.STR + 5);
                //stop hitting multiple target
                break;
            }
        }
    }
}
