using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour
{
    [SerializeField] float relieveValue = 40;
    [SerializeField] Transform room;
    public bool used { get; private set; }
    public bool inToilet { get; private set; }
    public bool playerin { get; private set; }
    bool inited;
    [SerializeField] Transform door, relieveBtn;
    Player player;
    private void Start()
    {
        if (!inited)
            used = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void Init( bool _used, bool _inToilet )
    {
        used = _used;
        inToilet = _inToilet;
        inited = true;
    }
    public void InsideUse()
    {
        if (inToilet)
        {
            float d_dis = Vector3.Distance(player.transform.position, door.position);
            float r_dis = Vector3.Distance(player.transform.position, relieveBtn.position);
            if (d_dis <= .6f && d_dis < r_dis)
            {
                GoOutSide();
            }
            else if (r_dis <= .6f)
            {
                Relieve();
            }
        }
    }
    public void Use()
    {
        if (!inToilet)
        {
            inToilet = true;
            GoInside();
        }
        else
        {
            InsideUse();
        }
    }
    private void GoInside()
    {
        inToilet = true;
        player.transform.position = room.position;
    }
    private void GoOutSide()
    {
        inToilet = false;
        player.transform.position = transform.localPosition;
    }
    void Relieve()
    {
        if (used) return;
        player.Relieve(relieveValue);
        used = true;
    }
}
