using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public List<Actor> actors = new List<Actor>();

    public GameObject AIPrefab;

    public GameObject BallPrefab;
    GameObject ball;
    GameUIManager UI;
    Game game;

    float ResetTime = -50;

    float UtilityTimer = 0;

    Team Home;
    Team Away;

    enum GameState
    {
        NONE,
        PRESTART,
        GAME,
        PAUSED
    }

    GameState state;

    public static GameManager instance = null;

    void Start()
    {
        if (instance != null)
            DestroyImmediate(instance.gameObject);

        instance = this;
        Debug.Log("Setting Instance");

        state = GameState.NONE;
        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            PreStartGame();
        }
    }

    void PreStartGame()
    {
        UtilityTimer = Time.time + 0.1f;

        state = GameState.PRESTART;

        if (Home == null)
            CreateTeams();

        AddAI();
        //AddAI();
    }

    void AddAI()
    {
        GameObject ai = GameObject.Instantiate(AIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(ai);
    }

    void OnStartGame()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        UI = GameObject.Find("UICanvas").GetComponent<GameUIManager>();
        SetScore();
        
        SpawnBall();

        for (int i = 0; i < actors.Count; i++)
        {
            bool homeTeam = actors[i].Team == "Home";

            actors[i].OnGameReset();
            if(homeTeam)
                actors[i].SetTeamKit(Home);
            else
                actors[i].SetTeamKit(Away);

            if(actors[i].tag == "AI")
            {
                (actors[i] as AIPlayer).SetupAI(game);
            }
            else if(actors[i].tag == "Keeper")
            {
                (actors[i] as AIKeeper).SetupAI(game);
            }
        }

        state = GameState.GAME;
    }

    public void AddActor(Actor a)
    {
        if (a.tag == "Keeper")
        {
            a.Team = a.transform.position.x < 0 ? "Home" : "Away";
        } else if (a.tag == "Player")
        {
            //Shitty Team assignment, fix this
            bool homeTeam = a.GetComponent<PlayerSync>().LobbyTeam == "Home";

            if (Home == null)
                CreateTeams();

            if (homeTeam)
            {
                a.Team = "Home";
                Home.AddPlayer(a);
            } else
            {
                a.Team = "Away";
                Away.AddPlayer(a);
            }

            float x = homeTeam ? -20 : 20;
            a.transform.position = new Vector3(x, a.transform.position.y, Random.Range(-15, 15));            
        } 
        else if (a.tag == "AI")
        {
            bool homeTeam = true;

            a.Team = "Home";
            Home.AddPlayer(a);

            float x = homeTeam ? -20 : 20;
            a.transform.position = new Vector3(x, a.transform.position.y, Random.Range(-15, 15)); 
        }

        actors.Add(a);

    }
	
	// Update is called once per frame
	void Update () 
    {
        if (state == GameState.PRESTART)
        {
            if(Time.time > UtilityTimer)
                OnStartGame();
        }

        DebugKeys();

        if (ResetTime > 0 && Time.time > ResetTime)
        {
            Reset();
            ResetTime = -50;
        }
	}

    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBall();
        }
    }

    void SpawnBall()
    {
        ball = GameObject.Instantiate(BallPrefab, new Vector3(0, 10, 0), Quaternion.identity) as GameObject;
        ball.name = "Ball";
        NetworkServer.Spawn(ball);
    }

    public void Reset()
    {
        if (ball)
			Destroy(ball);

        SpawnBall();
        UI.RpcShowGoal(false);

        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].OnGameReset();
        }
    }

    public void OnGoal(string net)
    {
        if (ResetTime > 0)
            return;

        game.OnGoal(net);

        UI.RpcShowGoal(true);

        SetScore();

        ResetTime = Time.time + 3;        
    }

    void SetScore()
    {
        if(UI)
            UI.SetScore(game.ScoreHome, game.ScoreAway);
    }

    void CreateTeams()
    {
        Home = new Team();
        Home.SetKit("Home");
        
        Away = new Team();
        Away.SetKit("Away");
    }
}
