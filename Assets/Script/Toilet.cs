using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour
{
    [SerializeField] float relieveValue = 10;
    bool used = false;
    public void Relieve(ref float bowel)
    {
        bowel -= 10;
    }
}
