using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerControllerInput : MonoBehaviour
{
    public static GameObject Player;
    public Transform cursor;
    public RectTransform UICursor;
    [SerializeField] private CharacterController2D controller2D;
    [SerializeField] private PlayerInput input;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private MeleeWeapon meleeWeapon;
    
   
    
    private Vector3 cursorPosition;
    private Vector3 planePosition;
    Plane m_Plane;

    public bool buildMode = true;
    


    private void Awake()
    {
        Player = this.gameObject;
    }

    private void Start()
    {
        planePosition = new Vector3(transform.position.x, transform.position.y, 1f);

        m_Plane = new Plane(Vector3.forward, planePosition);
        Cursor.visible = false;

        input.actions["Attack"].performed += OnPerformed;
        input.actions["Cancel"].performed += OnCancelPerformed;
    }

    private void OnDisable()
    {
        input.actions["Attack"].performed -= OnPerformed;
        input.actions["Cancel"].performed -= OnCancelPerformed;
    }

    private void OnCancelPerformed(InputAction.CallbackContext obj)
    {
        if (BuildManager.Instance.buildingMode)
        {
            BuildManager.Instance.CancelBuild();
        }
    }

    private void OnPerformed(InputAction.CallbackContext context)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (BuildManager.Instance.buildingMode)
            {
                BuildManager.Instance.Build();
            }
            else
            {
                meleeWeapon.Attack();

            }
        }
    }
    
    private void Update()
    {
        Vector2 move = input.actions["Movement"].ReadValue<Vector2>();
        animator.SetBool("IsMoving", Mathf.Abs(move.magnitude) > 0);

        sprite.flipX = move.x < 0;

        controller2D.Move(move);

        var ray = Camera.main.ScreenPointToRay(input.actions["Mouse"].ReadValue<Vector2>());

        float enter = 0.0f;

        UICursor.position = input.actions["Mouse"].ReadValue<Vector2>();


        if (m_Plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            cursorPosition = hitPoint;

            Vector2 aimDirection = cursorPosition - transform.position;

            float angle = CalculateAngle(Vector3.up, aimDirection);

            meleeWeapon.rotation = angle;
            cursor.transform.position = Vector3.Lerp(cursor.transform.position, cursorPosition, Time.deltaTime * 30f);
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            cursor.gameObject.SetActive(false);
            UICursor.gameObject.SetActive(true);
        }
        else
        {
            cursor.gameObject.SetActive(true);
            UICursor.gameObject.SetActive(false);
        }
    }


    float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    private void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
        Handles.color = Color.black;
        Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, 1f), Vector3.forward, 1f);
        Handles.color = Color.white;
        Handles.DrawWireDisc(cursorPosition, Vector3.forward, 0.25f);
        #endif
    }
}