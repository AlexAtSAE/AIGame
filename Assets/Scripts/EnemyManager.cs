using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Net.Http.Headers;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public GameObject _Player; //internal player variable
    public static GameObject Player;

    public delegate void PlayerNoLongerExistsCallback();
    public static PlayerNoLongerExistsCallback onPlayerNoLongerExists = ()=>{};


    public GameObject EnemyPrefab;


    private static float timeElapsed = 0;

    private static Queue<EnemySpawnRequest> SpawnEnemyQueue = new Queue<EnemySpawnRequest>();
    private static EnemySpawnRequest? CurrentRequest = null;

    private float NextSpawnTime = 0;
    private int requestCountRemaining = 0;
    
    private void Awake()
    {
        
        if (instance == null) instance = this; 
        else
            {
                Debug.LogError("Enemy manager instance already exists!!");
                return;
            }

        //Try set static player variable to internal player variable
        if (Player == null && _Player != null)
            Player = _Player;
    }
    private void Start()
    {
        if (Player == null)
        {
            Debug.Log("Player not exist!!");
        }

        //AddEnemyRequest(new EnemySpawnRequest(5, 0.5f, 1f));
        //AddEnemyRequest(new EnemySpawnRequest(3, 1f, 1f));
    }





    void Update()
    {
        timeElapsed += Time.deltaTime;
        //Set static player variable once internal player variable is found
        if(Player == null && _Player != null) Player = _Player;
        
        //Spawn loop
        if (timeElapsed > NextSpawnTime && CurrentRequest.HasValue && requestCountRemaining > 0)
        {
            SpawnEnemy(UnityEngine.Random.value, UnityEngine.Random.value);
            NextSpawnTime += CurrentRequest.Value.timeSeperation;
            requestCountRemaining--;
        }

        //Check if there are any requests
        if (requestCountRemaining > 0) return;
        CurrentRequest = null;
        //Try get next request
        if (SpawnEnemyQueue.Count == 0) return;

        //Get and initialise next request        
        EnemySpawnRequest next = SpawnEnemyQueue.Dequeue();

        NextSpawnTime = timeElapsed + next.initDelay;
        requestCountRemaining = next.count;
        CurrentRequest = next;
        
        
    }
    public static bool AddEnemyRequest(EnemySpawnRequest request)
    {
        SpawnEnemyQueue.Enqueue(request);
        return true;
    }

    private bool Internal_SpawnEnemy(Vector2 position)
    {
        Instantiate(EnemyPrefab, new Vector3(position.x, 0.5f, position.y), Quaternion.identity);
        return false;
    }
    public static bool SpawnEnemy(Vector2 position)
    {
        bool result = instance.Internal_SpawnEnemy(position);
        return result;
    }
    public static bool SpawnEnemy(float x, float y)
    {
        bool result = instance.Internal_SpawnEnemy(new Vector2(x,y));
        return result;
    }

    public static void KillPlayer()
    {
        instance._Player = null;
        Player = null;
        onPlayerNoLongerExists.Invoke();
    }

}


[CustomEditor(typeof(EnemyManager))]
public class EnemyMangerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EnemyManager mainScript = (EnemyManager)target;
        if (GUILayout.Button("Kill player", GUILayout.Width(100), GUILayout.Height(50)))
        {
            EnemyManager.KillPlayer();
        }
        if (GUILayout.Button("Spawn enemies", GUILayout.Width(100), GUILayout.Height(50)))
        {
            EnemyManager.AddEnemyRequest(new EnemySpawnRequest(10, 0.1f, 0));
        }
    }
}



public struct EnemySpawnRequest
{
    public int count;
    public float timeSeperation;
    public float initDelay;
    public EnemySpawnRequest(int count = 1, float timeSeperation = 0.0f, float initDelay = 0.0f)
    {
        this.count = count;
        this.timeSeperation = timeSeperation;
        this.initDelay = initDelay;
    }
}