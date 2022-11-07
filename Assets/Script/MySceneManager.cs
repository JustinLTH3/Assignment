using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

/************************************************
 * loading scene, saving and load previous game
 * This WILL NOT DESTROY ON LOAD only one instance
 * 
 * 
 ************************************************/
public class MySceneManager : MonoBehaviour
{
    static MySceneManager instance;
    string s_savedGameKey = "SavedGame";
    Canvas loadScreen, mainCanvas;

    private void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GetCanvas();
        loadScreen.enabled = false;
    }
    void GetCanvas()
    {
        loadScreen = GameObject.FindGameObjectWithTag("LoadScreen").GetComponent<Canvas>();
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
    }
    IEnumerator GetCanvas( int index )//0== main, 1 == load
    {
        loadScreen = GameObject.FindGameObjectWithTag("LoadScreen").GetComponent<Canvas>();
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        loadScreen.enabled = index == 1;
        mainCanvas.enabled = index == 0;
        yield return null;
    }
    public void LoadSceneBtn( int index )
    {
        StartCoroutine(LoadScene(index, false));
    }
    public void NewGame()
    {
        DeleteSave();
        StartCoroutine(LoadLevel());
    }
    private IEnumerator LoadScene( int index, bool needInitAfterLoad )
    {
        yield return StartCoroutine(GetCanvas(1));
        yield return new WaitForSecondsRealtime(.5f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        while (!operation.isDone)
        {
            //Showing load screen;
            yield return null;
        }

        if (!needInitAfterLoad) yield return StartCoroutine(GetCanvas(0));
        else yield return StartCoroutine(GetCanvas(1));
    }
    public IEnumerator LoadLevel()
    {
        string s_levelData = PlayerPrefs.GetString(s_savedGameKey);
        LevelData levelData;
        if (!string.IsNullOrEmpty(s_levelData))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(s_levelData);
        }
        else
        {
            levelData = LevelData.Begining;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelData.level);
        while (!operation.isDone)
        {
            yield return null;
        }
        yield return StartCoroutine(GetCanvas(1));
        yield return StartCoroutine(LevelInit(levelData));
        //Initialize the level with level data, e.g. teachers' position
        loadScreen.enabled = false;
        mainCanvas.enabled = true;
    }
    IEnumerator LevelInit( LevelData levelData )
    {
        yield return null;
    }
    public void SaveGameBtn()
    {
        LevelData dataToSave = GetLevelData();


        string s_dataToSave = JsonConvert.SerializeObject(dataToSave);
        PlayerPrefs.SetString(s_savedGameKey, s_dataToSave);
    }

    LevelData GetLevelData()
    {
        LevelData levelData = new(SceneManager.GetActiveScene().buildIndex);


        return levelData;
    }
    public void GameEndBtn( bool win )
    {
        StartCoroutine(GameEnd(win));
    }
    private IEnumerator GameEnd( bool win )// back to start menu or retry.
    {
        DeleteSave();
        //Load Game End Scene;
        yield return StartCoroutine(LoadScene(SceneManager.sceneCountInBuildSettings - 1, true));
        
        if (win)
        {
            mainCanvas.transform.Find("Title").GetComponent<TMP_Text>().text = "You Win";
        }
        else
        {
            mainCanvas.transform.Find("Title").GetComponent<TMP_Text>().text = "You Lose";
        }
        yield return StartCoroutine(GetCanvas(0));
    }
    private void DeleteSave() //Delete Saved Game Data
    {
        PlayerPrefs.DeleteKey(s_savedGameKey);
    }
    public struct LevelData
    {
        //every data of the level that needed to be saved
        public int level;

        public static LevelData Begining = new(1);
        public LevelData( int _level )
        {
            level = _level;
        }
    }
}
