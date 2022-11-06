using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    InputManager inputManager;
    MySceneManager mySceneManager;
    bool paused = false;
    [SerializeField] GameObject pausemenu;
    private void Start()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.playerInput.Gameplay.Pause.performed += Pause_performed;

        mySceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();
        pausemenu.SetActive(false);
    }
    public void SaveGame()
    {
        mySceneManager.SaveGame();
        mySceneManager.LoadSceneBtn(0);
        Time.timeScale = 1;
    }
    private void Pause_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            pausemenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausemenu.SetActive(false);
        }
    }
}
