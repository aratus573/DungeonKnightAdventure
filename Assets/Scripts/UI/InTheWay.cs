using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InTheWay : MonoBehaviour
{
    [SerializeField] private GameObject solidBody;
    [SerializeField] private GameObject transparentBody;

    void Awake()
    {
        ShowSolid();
    }

    public void ShowTransparent()
    {
        solidBody.SetActive(false);
        transparentBody.SetActive(true);
    }

    public void ShowSolid()
    {
        solidBody.SetActive(true);
        transparentBody.SetActive(false);
    }
}
