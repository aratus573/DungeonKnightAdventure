using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 1f;
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        transform.localPosition = new Vector3(Random.Range(-1,1), 2, 0);
    }

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
    public void ChangeText(string text, Color color)
    {
        TMP_Text tmp = this.gameObject.GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.color = color;
    }

}
