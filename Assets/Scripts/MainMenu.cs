using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public InputField playerName;
    public MainManager mainManager;
    public string nameOfPlayer;
    public string bestName;
    public int score;

    public TextMeshProUGUI nameAndScore;

    // Start is called before the first frame update
    void Start()
    {
        if (mainManager != null)
        {
            mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject); // Data Persistence between sence

        LoadPlayerData();  // Load Data from Previous game

        UploadSocreLeaderboard();

    }

    [System.Serializable]
    class SaveData
    {
        public string inputNameOfPlayer; // InputField PlayerName to store - DDSc
        public string bestNameOfPlayer;  // Best Player name from previous Game to store
        public int score; // Best score from previous game to store
    }

    public void SavePlayerData()
    {
        SaveData data = new SaveData();

        data.inputNameOfPlayer = nameOfPlayer;
        data.bestNameOfPlayer = bestName;
        data.score = score;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            nameOfPlayer = data.inputNameOfPlayer;
            bestName = data.bestNameOfPlayer;
            score = data.score;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);

        nameOfPlayer = playerName.text;

        SavePlayerData(); // When Start button is clicked , All data from menu scene is saved from here
    }

    public void UploadSocreLeaderboard()  // Update the loaded data from main sence to LeaderBoard
    {
        nameAndScore.text = $"Name :{bestName} --- Score :{score}";
    }
    public void Exit() // When Exit button is clicked , the game exits (via UnityEditor or application)
    {
        MainMenu.Instance.SavePlayerData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
}
