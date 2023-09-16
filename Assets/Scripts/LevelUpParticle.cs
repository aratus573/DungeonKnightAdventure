using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpParticle : MonoBehaviour
{
    PlayerLocomotion playerLocomotion;
    private GameObject player;
    public float LevelUpParticleTime = 4f;

    private void Awake()
    {
        //TODO : Fire Spell Audio

        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player").gameObject;
            playerLocomotion = player.GetComponent<PlayerLocomotion>();
        }
    }
    private void Update()
    {

        if (player != null)
        {
            transform.rotation = playerLocomotion.transform.rotation;
            LevelUpParticleTime -= Time.deltaTime;
            if (LevelUpParticleTime <= 0)
                Destroy(gameObject);
            else
                transform.position = playerLocomotion.transform.position;
        }
    }
}
