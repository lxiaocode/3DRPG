using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    public Texture2D point, doorway, attack, target, arrow;
    
    private RaycastHit _raycastHit;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {

        SetCursorTexture();
        MouseControl();
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _raycastHit))
        {
            switch (_raycastHit.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && _raycastHit.collider != null)
        {
            if (_raycastHit.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(_raycastHit.point);
            }
            else if (_raycastHit.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(_raycastHit.collider.gameObject);
            }
        }
    }
}
