using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Question")]
public class Question : ScriptableObject
{
    public string question;
    public string[] ans;
    public int c_ans;//correct ans index
}
