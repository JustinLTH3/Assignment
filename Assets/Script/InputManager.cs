using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    static InputManager instance;
    public enum GameState
    {
        StartMenu,
        InGame,

    }
    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        playerInput = new();
        //playerInput.Gameplay.Enable();
    }
    public void ChangeInputMap(GameState gameState)
    {
        playerInput.Disable();
        switch (gameState)
        {
            case GameState.InGame:
                playerInput.Gameplay.Enable();
                break;
            case GameState.StartMenu:
                playerInput.Menu.Enable();
                break;
        }
    }

}
