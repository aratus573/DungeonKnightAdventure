using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    [HideInInspector]
    RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    private GameObject player;
    private GameObject attackTarget;

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        SetCursorTexture();
        RotateToCursor();
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hitInfo))
        {
            // ¤Á´«¹«¼Ð¶K¹Ï
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Attackable":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if(Input.GetMouseButton(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
        }
    }

    void RotateToCursor()
    {

        if (Input.GetMouseButton(0) && hitInfo.collider.gameObject.CompareTag("Enemy"))
        {
            attackTarget = hitInfo.collider.gameObject;
            player.transform.forward = -attackTarget.transform.forward;
        }
    }
}
