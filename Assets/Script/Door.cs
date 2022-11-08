using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : MonoBehaviour
{
    MySceneManager mySceneManager;
    private void Start()
    {
        mySceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();
    }
    public void Exit()
    {
        var index = SceneManager.GetActiveScene().buildIndex + 1;
        if (index == SceneManager.sceneCountInBuildSettings - 1)
        {
            mySceneManager.GameEndBtn(true);
        }
        else
        {
            mySceneManager.LoadSceneBtn(index);
        }

    }
}
