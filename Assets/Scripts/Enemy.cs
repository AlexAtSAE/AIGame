using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Action<GameObject> onDeath = Self => { Debug.Log("dead"); };
    public EnemyState CurrentState = 0;
    public LayerMask obstacleLayer;
    private float timeAlive = 0.0f;
    public float timeHealthStart = 10.0f;
    public float spawnDelay;
    public float speed;
    public float damage;
    private Rigidbody rb;
    private Material mat;
    private float traceInterval = 1f;
    private bool lastHit = false;
    [SerializeField] private float defaultAttackCooldown = 1f;
    private float attackCooldown = 1f;
    private float timeIdle;
    private NavMeshAgent agent;
    private List<Vector3> navigationPositions;
    private int navigationPosIndex;
    

    // Start is called before the first frame update
    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
        navigationPositions = new List<Vector3>();
        navigationPositions.Add(new Vector3(UnityEngine.Random.Range(-20, 20),0.5f, UnityEngine.Random.Range(-20, 20)));
        navigationPositions.Add(new Vector3(UnityEngine.Random.Range(-20, 20),0.5f, UnityEngine.Random.Range(-20, 20)));
        navigationPositions.Add(new Vector3(UnityEngine.Random.Range(-20, 20),0.5f, UnityEngine.Random.Range(-20, 20)));
        

    }
    void Start()
    {
        EnemyManager.onPlayerNoLongerExists += onPlayerNoLongerExists;
        rb.maxLinearVelocity = 10.0f;
    }

    
    void Update()
    {
        //timers
        timeAlive += Time.deltaTime;
        attackCooldown -= Time.deltaTime;

        //material
        mat.SetFloat("_HealthPercent",(timeHealthStart-timeAlive)/timeHealthStart);

        //raycast
        PeriodicPlayerRaycast();

        //life
        if(timeHealthStart - timeAlive < 0)
        {
            onDeath.Invoke(gameObject);
            Destroy(gameObject);
        }



        //State conditions
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
                Idle_Condition();
                break;
            //Move between points
            case EnemyState.Patrol:
                Patrol_Condition();
                break;
            //Chase player
            case EnemyState.Chasing:
                Chasing_Condition();
                break;
            //Attack player
            case EnemyState.Attacking:
                Attacking_Condition();
                break;

        }
    }
    private void FixedUpdate()
    {
        //Physics
        switch (CurrentState) {
            case EnemyState.Idle:
            //We know player exists
                Idle();
                break;
            //Move between points
            case EnemyState.Patrol:
                Patrol();
                break;
            //Chase player
            case EnemyState.Chasing:
                Chasing();
                break;
            //Attack player
            case EnemyState.Attacking:
                Attacking();
                break;
        }
    }
    void PlayerNotFound()
    {

    }
    void PlayerNotFound_Condition()
    {
        if (Player.GameObject == null) { ChangeState(EnemyState.PlayerNotFound); return; }
        if (EnemyManager.Player == null) return;
        //Player just found
        if (timeAlive > spawnDelay)
            ChangeState(EnemyState.Idle);
        else
            ChangeState(EnemyState.Spawning);
    }
    void Spawning()
    {

    }
    void Spawning_Condition()
    {
        if (Player.GameObject == null) { ChangeState(EnemyState.PlayerNotFound); return; }
        if (timeAlive > spawnDelay)
            ChangeState(EnemyState.Idle);
    }
    void Idle()
    {
        timeIdle += Time.deltaTime;
        if (Player.GameObject == null) return;
        rb.AddForce(rb.velocity * -1 * 0.2f);
    }
    void Idle_Condition()
    {
        if (Player.GameObject == null) { ChangeState(EnemyState.PlayerNotFound); return; }

        

        if (Vector3.Distance(Player.position, transform.position) < 10.0f && !lastHit)
        {
            ChangeState(EnemyState.Chasing);
            return;
            
        }

        if (timeIdle > 5)
        {
            ChangeState(EnemyState.Patrol);
        }

    }
    void Patrol()
    {
        //pick random points and walk to them
        Vector3 currentPos = navigationPositions[navigationPosIndex % navigationPositions.Count];
        if (Vector3.Distance(currentPos,transform.position) < 0.5f)
        {
            navigationPosIndex = navigationPosIndex + 1;
        }
        agent.SetDestination(currentPos);
    }
    void Patrol_Condition()
    {
        if (Player.GameObject == null) { ChangeState(EnemyState.PlayerNotFound); return; }

        if (Vector3.Distance(Player.position, transform.position) < 10.0f && !lastHit)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }

    }
    void Chasing()
    {
        /*Vector2 PlayerPos = new Vector2(Player.position.x, Player.position.z);
        Vector2 MyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 Dir = PlayerPos-MyPos; 
        Vector2 DirNorm = Dir.normalized;
        rb.AddForce(new Vector3(DirNorm.x,0, DirNorm.y)*speed);*/
        agent.SetDestination(Player.position);
    }
    void Chasing_Condition()
    {
        if (Player.GameObject == null) { ChangeState(EnemyState.PlayerNotFound); return; }
        float dist = Vector3.Distance(Player.position, transform.position);
       

        if (dist > 15.0f || lastHit)
        {
            ChangeState(EnemyState.Idle);
        }
        else if (dist < 1.5f) 
        {
            ChangeState(EnemyState.Attacking);
        }
    }
    void Attacking()
    {
        Vector2 PlayerPos = new Vector2(Player.position.x, Player.position.z);
        Vector2 MyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 Dir = PlayerPos - MyPos;
        Vector2 DirNorm = Dir.normalized;
        rb.AddForce(new Vector3(DirNorm.x, 0, DirNorm.y) * speed);
        
        if (attackCooldown <= 0)
        {
            if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
            Debug.Log("Attack!");
            Player.Instance.TakeDamage(damage,gameObject);
            attackCooldown = defaultAttackCooldown;
        }
    }
    void Attacking_Condition()
    {
        if (Player.GameObject == null) { CurrentState = EnemyState.PlayerNotFound; return; }
        float dist = Vector3.Distance(Player.position, transform.position);
        if (dist > 2.0f)
        {
            Debug.Log("I cant attack you!");
            CurrentState = EnemyState.Chasing;
        }

    }

    void onPlayerNoLongerExists()
    {
        CurrentState = EnemyState.PlayerNotFound;
    }

    void PeriodicPlayerRaycast()
    {
        if (Player.GameObject == null) { return; }
        float dist = Vector3.Distance(Player.position, transform.position);
        traceInterval -= Time.deltaTime;
        if (traceInterval <= 0)
        {
            traceInterval = 1;
            RaycastHit hit;
            bool res = Physics.Raycast(transform.position, Player.position - transform.position, out hit, obstacleLayer);
            if (res && hit.collider.gameObject.CompareTag("Obstacle"))
            {
                Debug.Log("Can not see player");

                lastHit = true;
                return;
            }
            else
            {
                Debug.Log("Can see player");
                lastHit = false;
            }
            
        }
    }
    void ChangeState(EnemyState toState)
    {
        OnStateChanged(toState);
        CurrentState = toState;
    }

    void OnStateChanged(EnemyState changedTo)
    {
        Debug.Log($"Changed to {changedTo}");
        switch (changedTo)
        {
            case EnemyState.Attacking:
                attackCooldown = 0;
                break;
            case EnemyState.Idle:
                timeIdle = 0;
                break;
            default:
                break;
        }
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
                Gizmos.DrawWireSphere(transform.position, 1.5f);
                break;
            case EnemyState.Attacking:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 2f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1.5f);
                break;
        }
    }
}
