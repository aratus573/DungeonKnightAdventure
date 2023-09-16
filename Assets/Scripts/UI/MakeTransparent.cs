using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    [SerializeField] private List<InTheWay> currentlyInTheWay;
    [SerializeField] private List<InTheWay> alreadyTransparent;
    [SerializeField] private Transform player;
    public Transform Camera;
    [SerializeField] float radius;
    [SerializeField] float cameraPlayerDistance;
    void Awake()
    {
        currentlyInTheWay = new List<InTheWay>();
        alreadyTransparent = new List<InTheWay>();
    }

    void Update()
    {
        cameraPlayerDistance = Vector3.Magnitude(Camera.position - (player.position + Vector3.up));
        radius = 0.8f;
        /*
        if (cameraPlayerDistance < 2)
        {
            radius = 1;
        }
        else if (cameraPlayerDistance < 3)
        {
            radius = 2;
        }
        else
        {
            radius = 3;
        }
        */
        GetAllObjectsInTheWay();
        MakeObjectsSolid();
        MakeObjectsTransparent();
        Debug.DrawRay(Camera.position, (player.position + Vector3.up) - Camera.position, Color.green);
    }

    void GetAllObjectsInTheWay()
    {
        currentlyInTheWay.Clear();

        Vector3 p1 = Camera.position;
        Vector3 p2 = (player.position + Vector3.up);
        var hits1_Forward = Physics.CapsuleCastAll(p1, p2, radius, p1 - p2, cameraPlayerDistance-0.5f);

        foreach (var hit in hits1_Forward)
        {
            if(hit.collider.gameObject.TryGetComponent(out InTheWay inTheWay))
            {
                if (!currentlyInTheWay.Contains(inTheWay))
                {
                    currentlyInTheWay.Add(inTheWay);
                }
            }
        }
        /*
        foreach (var hit in hits1_Backward)
        {
            if (hit.collider.gameObject.TryGetComponent(out InTheWay inTheWay))
            {
                if (!currentlyInTheWay.Contains(inTheWay))
                {
                    currentlyInTheWay.Add(inTheWay);
                }
            }
        }
        */
    }

    void MakeObjectsTransparent()
    {
        for(int i = 0; i < currentlyInTheWay.Count; ++i)
        {
            InTheWay inTheWay = currentlyInTheWay[i];

            if (!alreadyTransparent.Contains(inTheWay))
            {
                inTheWay.ShowTransparent();
                alreadyTransparent.Add(inTheWay);
            }
        }
    }


    void MakeObjectsSolid()
    {
        for(int i = alreadyTransparent.Count - 1; i >= 0; --i)
        {
            InTheWay wasInTheWay = alreadyTransparent[i];

            if (!currentlyInTheWay.Contains(wasInTheWay))
            {
                wasInTheWay.ShowSolid();
                alreadyTransparent.Remove(wasInTheWay);
            }
        }
    }
}
