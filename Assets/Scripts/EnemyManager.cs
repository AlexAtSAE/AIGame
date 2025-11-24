using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public GameObject _Player; //internal player variable
    public static GameObject Player; private bool waitingForPlayer;

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
    }
    // Update is called once per frame
    void Update()
    {
        //Set static player variable once internal player variable is found
        if(Player == null && _Player != null) Player = _Player;
    }

    private bool Internal_SpawnEnemy()
    {
        return false;
    }
    public static bool SpawnEnemy()
    {
        bool result = instance.Internal_SpawnEnemy();
        return result;
    }
}
