using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

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
    private void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);


    }
    public void LoadSceneBtn( int index )
    {
        StartCoroutine(LoadScene(index));
    }
    private IEnumerator LoadScene( int index )
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        while (!operation.isDone)
        {
            //Showing load screen;
            yield return null;
        }
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
        
        yield return StartCoroutine(LevelInit(levelData));
        //Initialize the level with level data, e.g. teachers' position
    }
    IEnumerator LevelInit(LevelData levelData)
    {
        yield return null;
    }
    public void SaveGame()
    {
        LevelData dataToSave = new();
        string s_dataToSave = JsonConvert.SerializeObject(dataToSave);
        PlayerPrefs.SetString(s_savedGameKey, s_dataToSave);
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
