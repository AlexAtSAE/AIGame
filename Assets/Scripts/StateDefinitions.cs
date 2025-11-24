using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    PlayerNotFound,
    Spawning,
    Waiting,
    Patrol,
    Chasing,
    Attacking
}