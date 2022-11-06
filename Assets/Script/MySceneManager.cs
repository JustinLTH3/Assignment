using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;

/************************************************
 * loading scene, saving and load previous game
 * This WILL NOT DESTROY ON LOAD only one instance
 * 
 * 
 ************************************************/
public class MySceneManager : MonoBehaviour
{
    MySceneManager instance;
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
        loadScreen = GameObject.Find("LoadScreen").GetComponent<Canvas>();
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
    }
    void GetCanvas( int index )//0== main, 1 == load
    {
        loadScreen = GameObject.Find("LoadScreen").GetComponent<Canvas>();
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        loadScreen.enabled = index == 1;
        mainCanvas.enabled = index == 0;
    }
    public void LoadSceneBtn( int index )
    {
        StartCoroutine(LoadScene(index));
    }
    public void NewGame()
    {
        DeleteSave();
        StartCoroutine(LoadLevel());
    }
    private IEnumerator LoadScene( int index )
    {
        mainCanvas.enabled = false;
        loadScreen.enabled = true;
        yield return new WaitForSecondsRealtime(.5f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            //Showing load screen;
            yield return null;
        }

        GetCanvas();
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
        GetCanvas(1);
        yield return StartCoroutine(LevelInit(levelData));
        //Initialize the level with level data, e.g. teachers' position
        loadScreen.enabled = false;
        mainCanvas.enabled = true;
    }
    IEnumerator LevelInit( LevelData levelData )
    {
        yield return null;
    }
    public void SaveGame()
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
    public void GameEnd()// back to start menu or retry.
    {
        DeleteSave();

        //Load Game End Scene;
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
