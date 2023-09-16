using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioClip swordMiss;
    public AudioClip swordHit;
    public AudioClip enemyHit;
    public AudioClip spell1;
    public AudioClip spell2;
    public AudioClip enemyRock;
    public AudioClip levelup;
    public AudioClip playerDie;
    public AudioClip bossDie;

    public AudioSource Audio;

    void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    public void SwordMiss()
    {
        Audio.PlayOneShot(swordMiss, GameManager.Volume);
    }
    public void SwordHit()
    {
        Audio.PlayOneShot(swordHit, GameManager.Volume * 0.2f);
    }
    public void EnemyHit()
    {
        Audio.PlayOneShot(enemyHit, GameManager.Volume * 0.3f);
    }
    public void Spell1()
    {
        Audio.PlayOneShot(spell1, GameManager.Volume);
    }
    public void Spell2()
    {
        Audio.PlayOneShot(spell2, GameManager.Volume);
    }
    public void EnemyRock()
    {
        Audio.PlayOneShot(enemyRock, GameManager.Volume);
    }
    public void Levelup()
    {
        Audio.PlayOneShot(levelup, GameManager.Volume);
    }
    public void PlayerDie()
    {
        Audio.PlayOneShot(playerDie, GameManager.Volume);
    }
    public void BossDie()
    {
        Audio.PlayOneShot(bossDie, GameManager.Volume);
    }


}
