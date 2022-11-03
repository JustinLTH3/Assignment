using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Input buffer and InputManager
    float f_movementInput;
    InputManager inputManager;

    //Gravity and groundcheck 
    private float u_y;
    private float gravity = -10;
    bool isGrounded;
    Transform leg;
    [SerializeField] LayerMask ground;

    //movement
    Rigidbody2D rb;
    private float speed = 10;
    private float jumpHeight = 1;
    private bool jump;


    //Bars value
    public float AnxietyValue { get => anxietyValue; }
    private float anxietyValue;
    public float BowelValue { get => bowelValue; }
    private float bowelValue;
    private float f_anxietyMultiplier = 3;
    private float f_bowelMultiplier = 6;
    private float max = 100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        leg = transform.GetChild(0);

        inputManager.ChangeInputMap(InputManager.GameState.InGame);
        inputManager.playerInput.Gameplay.Movement.performed += Movement_performed;
        inputManager.playerInput.Gameplay.Movement.canceled += Movement_performed;
        inputManager.playerInput.Gameplay.Jump.performed += Jump_performed;
        inputManager.playerInput.Gameplay.Jump.canceled += Jump_performed;

        bowelValue = 0;
        anxietyValue = 0;

    }
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        jump = obj.ReadValueAsButton();
    }
    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        f_movementInput = obj.ReadValue<float>();
    }
    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(leg.position, .3f, ground);
    }
    private void Update()
    {
        BarValueChange(ref anxietyValue, f_anxietyMultiplier);
        BarValueChange(ref bowelValue, f_bowelMultiplier);
    }
    void BarValueChange(ref float bar, float multiplier)
    {
        bar += Time.deltaTime * multiplier / max;
        if (bar >= 1) Die();
    }

    private void Die()
    {
        Debug.Log("Die");
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
    }
    private void Move()         //All Movement
    {
        Vector2 move = new(); //position the player move to 

        move.x = transform.position.x;
        move.y = transform.position.y;

        if (!isGrounded) //gravity and jump
        {
            move.y += (u_y * Time.deltaTime + gravity * Time.deltaTime * Time.deltaTime / 2);
            u_y += gravity * Time.deltaTime;
        }
        else
        {
            if (jump)
            {
                u_y = 0;
                u_y = Mathf.Sqrt(-2 * gravity * jumpHeight);
                move.y += (Time.deltaTime * u_y * transform.up).y;
            }
        }

        move.x += f_movementInput * Time.deltaTime * speed;

        rb.MovePosition(move);
    }
}
