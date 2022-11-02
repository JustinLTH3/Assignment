using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    static InputManager instance;
    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);

        playerInput = new();
        //playerInput.Gameplay.Enable();

    }


}
