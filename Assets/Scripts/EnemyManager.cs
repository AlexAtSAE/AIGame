using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public GameObject _Player; //internal player variable
    public static GameObject Player; private bool waitingForPlayer;

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

        AddEnemyRequest(new EnemySpawnRequest(10, 0.1f, 3f));
        AddEnemyRequest(new EnemySpawnRequest(50, 0.01f, 1f));
    }
    




    void Update()
    {
        timeElapsed += Time.deltaTime;
        //Set static player variable once internal player variable is found
        if(Player == null && _Player != null) Player = _Player;
        
        //Spawn loop
        if (timeElapsed > NextSpawnTime && CurrentRequest.HasValue && requestCountRemaining > 0)
        {
            SpawnEnemy(Random.value, Random.value);
            NextSpawnTime += CurrentRequest.Value.timeSeperation;
            requestCountRemaining--;
        }

        //Check if there are any requests
        if (requestCountRemaining > 0) return;
        //Try get next request
        if (SpawnEnemyQueue.Count == 0) return;

        //Get and initialise next request        
        EnemySpawnRequest next = SpawnEnemyQueue.Dequeue();

        NextSpawnTime += next.initDelay;
        requestCountRemaining = next.count;
        CurrentRequest = next;
        
        
    }

    private bool Internal_SpawnEnemy(Vector2 position)
    {
        Instantiate(EnemyPrefab, new Vector3(position.x, 10.0f, position.y), Quaternion.identity);
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
    public static bool AddEnemyRequest(EnemySpawnRequest request)
    {
        SpawnEnemyQueue.Enqueue(request);
        return true;
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