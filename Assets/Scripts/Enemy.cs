using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action<GameObject> onDeath = Self => { Debug.Log("dead"); };
    public EnemyState CurrentState = 0;
    private float timeAlive = 0.0f;
    public float timeHealthStart = 10.0f;
    public float spawnDelay;
    public float speed;
    private Rigidbody rb;
    private Material mat;
    private float traceInterval = 1f;
    private bool lastHit = false;
    // Start is called before the first frame update
    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        EnemyManager.onPlayerNoLongerExists += onPlayerNoLongerExists;
    }

    
    void Update()
    {
        
        
        timeAlive += Time.deltaTime;
        mat.SetFloat("_HealthPercent",(timeHealthStart-timeAlive)/timeHealthStart);
        if(timeHealthStart - timeAlive < 0)
        {
            onDeath.Invoke(gameObject);
            Destroy(gameObject);
        }
        switch (CurrentState) {
            //Literally do nothging
            case EnemyState.PlayerNotFound:
                PlayerNotFound_Condition();
                break;
            //Wait to hit the ground
            case EnemyState.Spawning:
                Spawning_Condition();
                break;
            //Just idle
            case EnemyState.Idle:
                //We know player exists
                //Idle();
                Idle_Condition();
                break;
            //Move between points
            case EnemyState.Patrol:
                //Patrol();
                Patrol_Condition();
                break;
            //Chase player
            case EnemyState.Chasing:
                //Chasing();
                Chasing_Condition();
                break;
            //Attack player
            case EnemyState.Attacking:
                //Attacking();
                Attacking_Condition();
                break;

        }
    }
    private void FixedUpdate()
    {
        switch (CurrentState) {
            case EnemyState.Idle:
                //We know player exists
                Idle();
                //Idle_Condition();
                break;
            //Move between points
            case EnemyState.Patrol:
                Patrol();
                //Patrol_Condition();
                break;
            //Chase player
            case EnemyState.Chasing:
                Chasing();
                //Chasing_Condition();
                break;
            //Attack player
            case EnemyState.Attacking:
                Attacking();
                //Attacking_Condition();
                break;

        }
    }
    void PlayerNotFound()
    {

    }
    void PlayerNotFound_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        if (EnemyManager.Player == null) return;
        //Player just found
        if (timeAlive > spawnDelay)
            CurrentState = EnemyState.Idle;
        else
            CurrentState = EnemyState.Spawning;
    }
    void Spawning()
    {

    }
    void Spawning_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        if (timeAlive > spawnDelay)
            CurrentState = EnemyState.Idle;
    }
    void Idle()
    {
        if (Player.GameObject == null) return;
        rb.AddForce(rb.velocity * -1 * 0.2f);
    }
    void Idle_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        
        if (Vector3.Distance(Player.GameObject.transform.position, transform.position) < 10.0f)
        {
            Debug.Log("I see you!!");
            CurrentState = EnemyState.Chasing;
        }
    }
    void Patrol()
    {

    }
    void Patrol_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
    }
    void Chasing()
    {
        Vector2 PlayerPos = new Vector2(Player.GameObject.transform.position.x, Player.GameObject.transform.position.z);
        Vector2 MyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 Dir = PlayerPos-MyPos; 
        Vector2 DirNorm = Dir.normalized;
        rb.AddForce(new Vector3(DirNorm.x,0, DirNorm.y)*speed);
    }
    void Chasing_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        float dist = Vector3.Distance(Player.GameObject.transform.position, transform.position);
        traceInterval -= Time.deltaTime;
        if (traceInterval <= 0)
        {
            RaycastHit hit;
            bool res = Physics.Raycast(transform.position, Player.GameObject.transform.position - transform.position, out hit);
            if (res && hit.collider.gameObject.CompareTag("Obstacle")) { 
                CurrentState = EnemyState.Idle;
                lastHit = true;
                return;
            }
            else
            {
                lastHit = false;
            }
                traceInterval = 1;
        }
        if (lastHit) return;

        if (dist > 15.0f)
        {
            Debug.Log("I lost you :(");
            CurrentState = EnemyState.Idle;
        }
        else if (dist < 0.5f) 
        {
            CurrentState = EnemyState.Attacking;
        }
    }
    void Attacking()
    {

    }
    void Attacking_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        float dist = Vector3.Distance(Player.GameObject.transform.position, transform.position);
        if (dist > 1.0f)
        {
            Debug.Log("I cant attack you!");
            CurrentState = EnemyState.Chasing;
        }
    }

    void onPlayerNoLongerExists()
    {
        CurrentState = EnemyState.PlayerNotFound;
    }

    private void OnDrawGizmos()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 10f);
                break;
            case EnemyState.Chasing:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, 15f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
                break;
        }
    }
}
