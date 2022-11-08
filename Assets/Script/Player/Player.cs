using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
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
    private float speed = 3;
    private float jumpHeight = 1;
    private bool jump;

    //Bars value
    public float AnxietyValue { get => anxietyValue; }
    private float anxietyValue = 0;
    public float BowelValue { get => bowelValue; }
    private float bowelValue = 0;
    private float f_anxietyMultiplier = 2;
    private float f_bowelMultiplier = 1;
    private float max = 100;
    private float slipAnxiety = 10;

    MySceneManager mySceneManager;

    public bool beingAsked = false;
    bool answered = false;
    int playerAns;
    GameObject quesDisHolder;
    TMP_Text quesDis;
    List<Button> Ans = new();

    public void Init( float anxiety, float bowel, Vector3 pos, int level )
    {
        anxietyValue = anxiety;
        bowelValue = bowel;
        transform.position = pos;
        f_anxietyMultiplier *= level;
        f_bowelMultiplier *= level;
    }
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
        inputManager.playerInput.Gameplay.AnswerQues0.performed += AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues1.performed += AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues2.performed += AnswerQuestion;
        inputManager.playerInput.Gameplay.Interact.performed += Interact_performed;

        bowelValue = 0;
        anxietyValue = 0;

        quesDisHolder = GameObject.FindGameObjectWithTag("MainCanvas").transform
            .Find("QuestionDisplayHolder").gameObject;
        quesDisHolder.SetActive(false);
        quesDis = quesDisHolder.transform.Find("Question").Find("Question").GetComponent<TMP_Text>();
        Ans.Add(quesDisHolder.transform.Find("Question").Find("Ans0").GetComponent<Button>());
        Ans.Add(quesDisHolder.transform.Find("Question").Find("Ans1").GetComponent<Button>());
        Ans.Add(quesDisHolder.transform.Find("Question").Find("Ans2").GetComponent<Button>());

        mySceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();
    }

    private void Interact_performed( InputAction.CallbackContext obj )
    {
        Door door = GameObject.Find("Door").GetComponent<Door>();
        if (Vector3.Distance(transform.position, door.transform.position) <= .7f) door.Exit();
    }

    public IEnumerator TeacherAsk( Question question )
    {
        beingAsked = true;
        float timer = 0;

        quesDisHolder.SetActive(true);
        quesDis.text = question.question;
        for (int i = 0; i < 3; i++)
        {
            Ans[i].transform.GetComponentInChildren<TMP_Text>().text = question.ans[i];
            var j = i;
            Ans[i].onClick.AddListener(delegate () { AnswerQuestion(j); });
        }

        while (!answered && timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (!answered)
        {
            AnswerCorrect(false);
        }
        if (playerAns == question.c_ans) AnswerCorrect(true);
        else AnswerCorrect(false);

        for (int i = 0; i < 3; i++)
        {
            Ans[i].onClick.RemoveAllListeners();
        }

        beingAsked = false;
        answered = false;
        quesDisHolder.SetActive(false);
    }
    public IEnumerator TeacherAsk( Question[] questions )
    {
        for (int i = 0; i < questions.Length; i++)
        {
            yield return StartCoroutine(TeacherAsk(questions[i]));
        }
    }
    void AnswerCorrect( bool isCorrect )
    {
        if (!isCorrect)
        {
            anxietyValue += max / 3 / max;
        }
        else
        {
            anxietyValue -= max / 8 / max;
        }
    }
    void AnswerQuestion( InputAction.CallbackContext context )
    {
        if (!beingAsked) return;
        switch (context.action.name)
        {
            case "AnswerQues0":
                AnswerQuestion(0);
                break;
            case "AnswerQues1":
                AnswerQuestion(1);
                break;
            case "AnswerQues2":
                AnswerQuestion(2);
                break;
        }
    }
    void AnswerQuestion( int answer )
    {
        answered = true;
        playerAns = answer;
    }
    private void OnDestroy()
    {
        inputManager.playerInput.Gameplay.Movement.performed -= Movement_performed;
        inputManager.playerInput.Gameplay.Movement.canceled -= Movement_performed;
        inputManager.playerInput.Gameplay.Jump.performed -= Jump_performed;
        inputManager.playerInput.Gameplay.Jump.canceled -= Jump_performed;
        inputManager.playerInput.Gameplay.AnswerQues0.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues1.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues2.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.Interact.performed -= Interact_performed;

    }
    private void Jump_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        jump = obj.ReadValueAsButton();
    }
    private void Movement_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
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
    void BarValueChange( ref float bar, float multiplier )
    {
        bar += Time.deltaTime * multiplier / max;
        if (bar >= 1) Die();
    }
    private void Die()
    {
        mySceneManager.GameEndBtn(false);
        Destroy(gameObject);
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

        if (!beingAsked) move.x += f_movementInput * Time.deltaTime * speed;

        rb.MovePosition(move);
    }
    public void Slip()
    {
        anxietyValue += slipAnxiety / max;
    }
}
