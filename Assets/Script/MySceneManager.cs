using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/************************************************
 * loading scene, saving and load previous game
 * This WILL NOT DESTROY ON LOAD only one instance
 * 
 * 
 ************************************************/
public class MySceneManager : MonoBehaviour
{
    static MySceneManager instance;
    InputManager inputManager;
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
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        if (SceneManager.GetActiveScene().buildIndex == 0)
            mainCanvas.transform.Find("QuitBtn").GetComponent<Button>()
            .onClick.AddListener(() => Application.Quit());
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
    public void LoadSceneBtn( int index, bool needInitAfterLoad = false )
    {
        StartCoroutine(LoadScene(index, needInitAfterLoad));
    }
    public IEnumerator LoadSceneBtn( int index, bool needInitAfterLoad, float bv, float avalue )
    {
        yield return StartCoroutine(LoadScene(index, needInitAfterLoad));
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().BarInit(bv, avalue);
    }
    public void NewGame()
    {
        DeleteSave();
        RemoveListeners();
        StartCoroutine(LoadLevel());
    }
    void RemoveListeners()
    {
        mainCanvas.transform.Find("NewGameBtn").GetComponent<Button>()
            .onClick.RemoveAllListeners();
        mainCanvas.transform.Find("LoadGameBtn").GetComponent<Button>()
            .onClick.RemoveAllListeners();
        mainCanvas.transform.Find("QuitBtn").GetComponent<Button>()
            .onClick.RemoveAllListeners();
    }
    public void LoadGame()
    {
        StartCoroutine(LoadLevel());
        RemoveListeners();
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

        if (index == SceneManager.sceneCountInBuildSettings - 1 || index == 0)
            inputManager.ChangeInputMap(InputManager.GameState.StartMenu);
        else inputManager.ChangeInputMap(InputManager.GameState.InGame);

        if (!needInitAfterLoad) yield return StartCoroutine(GetCanvas(0));
        else yield return StartCoroutine(GetCanvas(1));
        if (index == 0)
        {
            mainCanvas.transform.Find("NewGameBtn").GetComponent<Button>()
            .onClick.AddListener(delegate () { NewGame(); });
            mainCanvas.transform.Find("LoadGameBtn").GetComponent<Button>()
                .onClick.AddListener(delegate () { LoadGame(); });
        }
    }
    public IEnumerator LoadLevel()
    {
        string s_levelData = PlayerPrefs.GetString(s_savedGameKey);
        LevelData levelData;
        if (!string.IsNullOrEmpty(s_levelData))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(s_levelData);
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
        else
        {
            yield return StartCoroutine(LoadScene(1, false));
        }

    }
    IEnumerator LevelInit( LevelData levelData )
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.Init(levelData.anxiety, levelData.bowel,
            new Vector3(levelData.playerPos_x, levelData.playerPos_y, levelData.playerPos_z), levelData.level,
            levelData.beingAsked, levelData.timer
            );
        List<GameObject> teachers = GameObject.FindGameObjectsWithTag("Teacher").ToList();
        for (int i = 0; i < levelData.questions.Length; i++)
        {
            Teacher teacher = teachers[i].GetComponent<Teacher>();
            teacher.Init(levelData.questions[i], levelData.asked[i], levelData.asking[i]);
        }

        Toilet toilet = GameObject.FindGameObjectWithTag("Toilet").GetComponent<Toilet>();
        toilet.Init(levelData.toiletUsed, levelData.inToilet);

        yield return null;
    }
    public void SaveGameBtn()
    {
        LevelData dataToSave = GetLevelData();

        string s_dataToSave = JsonConvert.SerializeObject(dataToSave);
        PlayerPrefs.SetString(s_savedGameKey, s_dataToSave);
    }
    LevelData GetLevelData()// use to save
    {
        List<GameObject> teachers = GameObject.FindGameObjectsWithTag("Teacher").ToList();
        List<bool> asked = new();
        List<string> questions = new();
        List<bool> asking = new();
        foreach (GameObject teacher in teachers)
        {
            asked.Add(teacher.GetComponent<Teacher>().asking);
            questions.Add(teacher.GetComponent<Teacher>()._Question.name);
            asking.Add(teacher.GetComponent<Teacher>().asking);
        }
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        Vector3 playerPos = player.transform.position;
        Toilet toilet = GameObject.FindGameObjectWithTag("Toilet").GetComponent<Toilet>();
        LevelData levelData = new(SceneManager.GetActiveScene().buildIndex,
            asked.ToArray(), asking.ToArray(), questions.ToArray(), player.Timer, player.beingAsked,
            playerPos.x, playerPos.y, playerPos.z, player.AnxietyValue, player.BowelValue,
            toilet.used, toilet.inToilet
            );
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
        while (!Input.anyKeyDown) yield return null;
        yield return StartCoroutine(LoadScene(0, false));
        mainCanvas.transform.Find("NewGameBtn").GetComponent<Button>()
            .onClick.AddListener(delegate () { NewGame(); });
        mainCanvas.transform.Find("LoadGameBtn").GetComponent<Button>()
            .onClick.AddListener(delegate () { LoadGame(); });
        mainCanvas.transform.Find("QuitBtn").GetComponent<Button>()
            .onClick.AddListener(() => Application.Quit());
    }
    private void DeleteSave() //Delete Saved Game Data
    {
        PlayerPrefs.DeleteKey(s_savedGameKey);
    }
    public struct LevelData
    {
        //every data of the level that needed to be saved
        public int level;

        public bool[] asked;
        public string[] questions;
        public bool[] asking;

        public float timer;
        public float anxiety, bowel;
        public float playerPos_x;
        public float playerPos_y;
        public float playerPos_z;
        public bool beingAsked;
        public bool toiletUsed;
        public bool inToilet;
        public LevelData( int _level, bool[] _asked, bool[] _asking, string[] _questions, float _timer,
           bool _beingAsked, float _playerPos_x, float _playerPos_y, float _playerPos_z, float _anxiety, float _bowel,
           bool _toiletUsed, bool _inToilet
           )
        {
            level = _level;
            asked = _asked;
            asking = _asking;
            questions = _questions;
            timer = _timer;
            anxiety = _anxiety;
            bowel = _bowel;
            beingAsked = _beingAsked;
            playerPos_x = _playerPos_x;
            playerPos_y = _playerPos_y;
            playerPos_z = _playerPos_z;
            toiletUsed = _toiletUsed;
            inToilet = _inToilet;
        }
    }
}