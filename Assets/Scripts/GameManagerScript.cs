using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    EnemyManager manager;
    public static GameManagerScript instance;
    
    public static float money = 0;
    public static PlayerStates playerState;


    public static Action<GameObject> moneyChanged = (obj) => { };

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = EnemyManager.instance;
        EnemyManager.onEnemyDied += EnemyDied;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void EnemyDied(GameObject obj)
    {
        //Enemy script = obj.GetComponent<Enemy>();
        money+=1;
        moneyChanged.Invoke(obj);
    }
}
