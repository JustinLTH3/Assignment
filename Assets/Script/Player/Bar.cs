using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Bar : MonoBehaviour
{
    Image mask;
    Player player;

    void Start()
    {
        mask = GetComponent<Image>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameObject.name)
        {
            case "AnxietyBar":
                mask.fillAmount = player.AnxietyValue / Player.max;
                break;
            case "BowelBar":
                mask.fillAmount = player.BowelValue / Player.max;
                break;
        }

    }
}
