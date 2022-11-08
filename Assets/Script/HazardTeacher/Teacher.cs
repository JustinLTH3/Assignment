using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Teacher : MonoBehaviour
{
    bool asked = false;
    Question _question;
    Player player;
    float askDis = 2;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Question[] questions = Resources.LoadAll<Question>("Question/");
        _question = questions[Random.Range(0, questions.Length)];
        asked = false;
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
