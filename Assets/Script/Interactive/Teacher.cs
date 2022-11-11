using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Teacher : MonoBehaviour
{
    public bool asking = false;
    public bool asked = false;
    Question _question;
    public Question _Question { get => _question; }
    Player player;
    float askDis = 2;
    public bool inited = false;
    public void Init( string question, bool _asked, bool _asking )
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _question = Resources.Load<Question>("Question/" + question);
        asking = _asking;
        asked = _asked;
        inited = true;
        if (asking && player.beingAsked) AskQues();
    }
    private void Awake()
    {
        if (!inited)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            Question[] questions = Resources.LoadAll<Question>("Question/");
            _question = questions[Random.Range(0, questions.Length)];


            asking = false;
            asked = false;
            inited = true;
        }
    }
    private void Update()
    {
        if (player == null) return;
        if (!asked)
            if (Vector3.Distance(transform.position, player.transform.position) <= askDis)
            {
                if (!player.beingAsked) AskQues();
            }
    }
    void AskQues()
    {
        asking = true;
        for (int i = 2; i > -1; i--)
        {
            int j = Random.Range(0, i + 1);
            string x = _question.ans[j];
            _question.ans[j] = _question.ans[i];
            _question.ans[i] = x;
            if (j == _question.c_ans)
            {
                _question.c_ans = i;
            }
            else if (i == _question.c_ans)
            {
                _question.c_ans = j;
            }
        }
        player.StartCoroutine(player.TeacherAsk(_question, this));
    }
}