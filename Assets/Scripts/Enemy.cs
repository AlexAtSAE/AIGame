using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyState CurrentState = EnemyState.PlayerNotFound;
    private float timeAlive = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        switch (CurrentState) {
            //Literally do nothging
            case EnemyState.PlayerNotFound:
                if (EnemyManager.Player == null) break;
                //Spawning complete
                if (timeAlive > 1.0f)
                    CurrentState = EnemyState.Waiting;
                else
                    CurrentState = EnemyState.Spawning;
                break;
            //Wait to hit the ground
            case EnemyState.Spawning:
                if (timeAlive > 1.0f)
                    CurrentState = EnemyState.Waiting;
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
