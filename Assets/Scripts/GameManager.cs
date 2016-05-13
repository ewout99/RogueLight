using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //Singleton 
    public static GameManager instance = null;

    // References
    public BoardManager boardScript;

    // Game Varibales
    public int level = 0;
    public float turnDelay = 0.1f;
    public float levelStartDelay = 2f;
    

    // Player Varibles
    public int playerFoodPoints = 100;

    //Enemy Varibales
    public List<Enemy> enemiesList = new List<Enemy>();
    

    //UI refrences
    private Text levelText;
    public GameObject levelImage;


    // [HideInInspector]
    public bool playersTurn = true;
    public bool enemyMoving;
    public bool doingSetup;

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            Debug.Log("Singleton is made");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Singleton already exists");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    private void OnLevelWasLoaded (int index)
    {
        level++;
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("Level Image");
        levelText = GameObject.Find("Level Text").GetComponent<Text>();
        levelText.text = "Day: " + level;
        levelImage.SetActive(true);

        if (levelImage == null)
        {
            Debug.LogWarning("Faulty Refrence");
        }

        Invoke("HideLevelImage", levelStartDelay);
        enemiesList.Clear();
        boardScript.SetupScene(level);

    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if (playersTurn || enemyMoving || doingSetup)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
	}

    public void GameOver()
    {
        levelText.text = "After " + level + " horrible days you were eaten by ghouls.";
        levelImage.SetActive(true);
        enabled = false;
    }

    public void AddEnemyList(Enemy script)
    {
        enemiesList.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemiesList.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        else
        {
            for (int i = 0; i < enemiesList.Count; i++)
            {
                enemiesList[i].MoveEnemy();
                yield return new WaitForSeconds(enemiesList[i].moveTime);
            }
        }
        playersTurn = true;
        enemyMoving = false;
    }
}
