using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    /****************************************
     * The Camera will follow the player
     ****************************************/
    Transform player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void FixedUpdate()
    {
        if (player != null)
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}
