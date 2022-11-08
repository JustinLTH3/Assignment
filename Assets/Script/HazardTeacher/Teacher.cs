using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Teacher : MonoBehaviour
{
    public bool asked = false;
    Question _question;
    public Question _Question { get => _question; }
    Player player;
    float askDis = 2;
    public bool inited = false;
    public void Init( string question, bool _asked )
    {
        _question = Resources.Load<Question>("Question/" + question);
        asked = _asked;
        inited = true;
    }
    private void Awake()
    {
        if (!inited)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            Question[] questions = Resources.LoadAll<Question>("Question/");
            _question = questions[Random.Range(0, questions.Length)];
            asked = false;
            inited = true;
        }
    }
    private void Update()
    {
        if (!asked)
            if (Vector3.Distance(transform.position, player.transform.position) <= askDis)
                AskQues();
    }
    void AskQues()
    {
        if (player.beingAsked) return;
        asked = true;
        player.StartCoroutine(player.TeacherAsk(_question));
    }

}
