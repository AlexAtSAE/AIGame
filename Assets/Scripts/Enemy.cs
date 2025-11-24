using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyState CurrentState = EnemyState.PlayerNotFound;
    private EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = EnemyManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState) {
            //Literally do nothging
            case EnemyState.PlayerNotFound:
                break;
            //Wait to hit the ground
            case EnemyState.Spawning:
                break;
            //Just idle
            case EnemyState.Waiting:
                break;
            //Move between points
            case EnemyState.Patrol:
                break;
            //Chase player
            case EnemyState.Chasing:
                break;
            //Attack player
            case EnemyState.Attacking:
                break;

        }

    }
}
