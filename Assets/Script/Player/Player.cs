using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    InputManager inputManager;

    //movement
    Rigidbody2D rb;
    private float speed = 3;
    private float jumpHeight = 1;
    private bool jump;
    float f_movementInput;//movement input buffer

    //Gravity and groundcheck 
    private float u_y;
    private float gravity = -10;
    bool isGrounded;
    Transform leg;
    [SerializeField] LayerMask ground;

    //Every things about anxiety and bowel bar
    public float AnxietyValue { get => anxietyValue; }
    private float anxietyValue = 0;
    public float BowelValue { get => bowelValue; }
    private float bowelValue = 0;
    private float f_anxietyMultiplier = 3;
    private float f_bowelMultiplier = 2;
    public const float max = 100;
    private float slipAnxiety = 10;

    MySceneManager mySceneManager;

    /*beingAsked is used to indicate the player is beingAsked by the teacher
    avoiding multiple teachers asking together*/
    public bool beingAsked = false;
    bool answered = false;
    int playerAns;

    //The timer for answering question
    public float Timer { get; private set; }

    //Store UI Object that display the questions, choices
    public GameObject QuesDisHolder { get; private set; }
    TMP_Text quesDis;
    List<Button> Ans = new();
    Image timerI;

    //Control Animation of the player
    [SerializeField] Animator animator;

    public void Init( float anxiety, float bowel, Vector3 pos, int level, bool _beingAsked, float timer )
    {
        anxietyValue = anxiety;
        bowelValue = bowel;
        transform.position = pos;
        f_anxietyMultiplier *= level;
        f_bowelMultiplier *= level;
        beingAsked = _beingAsked;
        Timer = timer;
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

        QuesDisHolder = GameObject.FindGameObjectWithTag("MainCanvas").transform
            .Find("QuestionDisplayHolder").gameObject;
        QuesDisHolder.SetActive(false);
        quesDis = QuesDisHolder.transform.Find("Question").Find("Question").GetComponent<TMP_Text>();
        Ans.Add(QuesDisHolder.transform.Find("Question").Find("Ans0").GetComponent<Button>());
        Ans.Add(QuesDisHolder.transform.Find("Question").Find("Ans1").GetComponent<Button>());
        Ans.Add(QuesDisHolder.transform.Find("Question").Find("Ans2").GetComponent<Button>());

        mySceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();
        timerI = QuesDisHolder.transform.Find("Question").Find("TimerFrame").Find("Timer").GetComponent<Image>();
    }
    private void Interact_performed( InputAction.CallbackContext obj )
    {
        Door door = GameObject.Find("Door").GetComponent<Door>();
        Toilet toilet;
        if (GameObject.Find("Toilet") != null)
        {
            toilet = GameObject.Find("Toilet").GetComponent<Toilet>();
            float d_dis = Vector3.Distance(transform.position, door.transform.position);
            float t_dis = Vector3.Distance(transform.position, toilet.transform.position);
            if (d_dis < t_dis && d_dis <= .6f)
            {
                door.Exit();
            }
            else if (t_dis < d_dis && t_dis <= .6f || toilet.inToilet)
            {
                toilet.Use();
            }
        }
        else
        {
            float d_dis = Vector3.Distance(transform.position, door.transform.position);
            if (d_dis <= .6f) door.Exit();
        }
    }
    public IEnumerator TeacherAsk( Question question, Teacher teacher )
    {
        if (!beingAsked) Timer = 0;
        beingAsked = true;

        QuesDisHolder.SetActive(true);
        quesDis.text = question.question;

        for (int i = 0; i < 3; i++)
        {
            Ans[i].transform.GetComponentInChildren<TMP_Text>().text = question.ans[i];
            var j = i;
            Ans[i].onClick.AddListener(delegate () { AnswerQuestion(j); });

        }
        while (!answered && Timer <= 10)
        {
            Timer += Time.deltaTime;
            timerI.fillAmount = 1 - Timer / 10;
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
        teacher.asked = true;
        QuesDisHolder.SetActive(false);
    }
    void AnswerCorrect( bool isCorrect )
    {
        if (!isCorrect)
        {
            anxietyValue += max / 3;
        }
        else
        {
            anxietyValue -= max / 8;
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
        //Unsubscribe input event
        inputManager.playerInput.Gameplay.Movement.performed -= Movement_performed;
        inputManager.playerInput.Gameplay.Movement.canceled -= Movement_performed;
        inputManager.playerInput.Gameplay.Jump.performed -= Jump_performed;
        inputManager.playerInput.Gameplay.Jump.canceled -= Jump_performed;
        inputManager.playerInput.Gameplay.AnswerQues0.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues1.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.AnswerQues2.performed -= AnswerQuestion;
        inputManager.playerInput.Gameplay.Interact.performed -= Interact_performed;

    }
    private void Jump_performed( InputAction.CallbackContext obj )
    {
        jump = obj.ReadValueAsButton();
    }
    private void Movement_performed( InputAction.CallbackContext obj )
    {
        f_movementInput = obj.ReadValue<float>();
    }
    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(leg.position, .35f, ground);
    }
    private void Update()
    {
        BarValueChange(ref anxietyValue, f_anxietyMultiplier);
        BarValueChange(ref bowelValue, f_bowelMultiplier * (1 + anxietyValue / max));
    }
    void BarValueChange( ref float bar, float multiplier )
    {
        bar += Time.deltaTime * multiplier;
        bar = Mathf.Clamp(bar, 0, max);
        if (bar >= max) Die();
    }
    public void BarInit( float b_v, float a_v )
    {
        bowelValue = b_v;
        anxietyValue = a_v;
    }
    public void Relieve( float _value )
    {
        bowelValue -= _value;
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
        if (move.x != transform.position.x) animator.SetBool("walking", true);
        if (move.x < transform.position.x) transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (move.x > transform.position.x) transform.rotation = Quaternion.identity;
        else animator.SetBool("walking", false);
        rb.MovePosition(move);
    }
    public void Slip()
    {
        anxietyValue += slipAnxiety * SceneManager.GetActiveScene().buildIndex;
        speed += 3;
    }
    public void SlipExit()
    {
        speed -= 3;
    }
}