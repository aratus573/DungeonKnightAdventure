using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgm : MonoBehaviour
{
    GameObject[] bgms;
    bool initialized;
    float Volume = 0.5f;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        bgms = GameObject.FindGameObjectsWithTag("bgm");
        for(int i = 1; i < bgms.Length; ++i)
        {
            Destroy(bgms[i]);
        }

    }
    void Update()
    {
        if (GameManager.IsInitialized)
        {
            Volume = GameManager.Volume;
        }
        this.GetComponent<AudioSource>().volume = Volume;
    }
}
