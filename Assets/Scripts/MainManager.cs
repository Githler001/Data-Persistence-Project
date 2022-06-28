using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text bestNameScore;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private string bestName = "noNameIsUploaded";
    //BestName to display in BestNameScore text. In case for unknown error, that string will be shown

    private int bestScore = 0; // Best score to display in BestNameScoreText

    
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        LoadDataPlayer();
        // Data persistence between scenes = DDSc , Data persistence between sessions = DDSe
        // Data persistence between scenes(inputField playerName) and sessions(Best name and best score from previous game)

        bestName = MainMenu.Instance.bestName; //Load bestName to display in BNS text from bestname saved data

        bestNameScore.text = $"Best score of PlayerName \"{bestName}\" is {bestScore}";
        //Record of best name and score from previous game to display at the START of game - DDSe
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (MainMenu.Instance.score <= bestScore) //if current score is lower than best score stored in data , do this *** (Name and score mechanics)
            {
                bestName = MainMenu.Instance.bestName;
                // use the best name to display from the best name stored in bestname data from previous game - DPSe
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
           
            bestNameScore.text = $"Best score of PlayerName \"{bestName}\" is {bestScore}";
            // display the best name and score played in current game - DDSc & DDSe



            if (Input.GetKeyDown(KeyCode.Space))
            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";

        if(m_Points > bestScore) //if current score is higer than best score stored in data , do this *** (Name and score mechanics)
        {
            bestScore = m_Points;
            MainMenu.Instance.score = bestScore;
            //Upload current score to best score data for next game and leaderboard - DDSc

            MainMenu.Instance.bestName = bestName = MainMenu.Instance.nameOfPlayer;
            // BestName Data <- BestName to display <- name Data from menu input --- DDSe <- DDSc
            // Explaination - Load playerName saved data from menu scene inputfield to best game player data for displayment - DDSc
            // Name displayment here use only bestPlayerName data for not being interfered by playerName inputted from main menu - DDSe

            // I have to used two different name datas for previous best player name and current player name 
            // I did tried using only one name like score Data but name Input is always messing me up

            SaveDataPlayer(); // Upload to file , without this no data will be saved for next game


            // If editor want to restaet name and score, replace the codes below in respedted line
            // MainMenu.Instance.score = 0;
            // MainMenu.Instance.bestName = "TestName";
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
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

    public void SaveDataPlayer()
    {
        MainMenu.Instance.SavePlayerData(); // DDSc & DDSe
    }

    public void LoadDataPlayer()
    {
        MainMenu.Instance.LoadPlayerData(); // DDSc & DDSe
        bestName = MainMenu.Instance.nameOfPlayer; //From Data file to this scene from main menu - DDSc
        bestScore = MainMenu.Instance.score; //From Data file to this scene - DDSe
    }

    
}
