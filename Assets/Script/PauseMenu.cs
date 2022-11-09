using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    InputManager inputManager;
    MySceneManager mySceneManager;
    Player player;
    bool paused = false;
    [SerializeField] GameObject pausemenu;
    private void Start()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.playerInput.Gameplay.Pause.performed += Pause_performed;

        mySceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();
        pausemenu.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void OnDestroy()
    {
        inputManager.playerInput.Gameplay.Pause.performed -= Pause_performed;
    }
    public void GiveUp()
    {
        Time.timeScale = 1;
        mySceneManager.GameEndBtn(false);
    }
    public void SaveGameBtn()
    {
        Time.timeScale = 1;
        mySceneManager.SaveGameBtn();

        mySceneManager.LoadSceneBtn(0);
    }
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            pausemenu.SetActive(true);
            if (player.beingAsked) player.QuesDisHolder.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            pausemenu.SetActive(false);
            if (player.beingAsked) player.QuesDisHolder.SetActive(true);
        }
    }
}
