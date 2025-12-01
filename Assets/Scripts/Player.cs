using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static GameObject GameObject;
    private Rigidbody rb;
    public float speed;
    public float maxSpeed;
    public float Health;
    public float MaxHealth;

    public Action<GameObject> PlayerDamagedEvent = (obj)=>{ };



    public static Vector3 position { get { return GameObject.transform.position; } private set {} }


    private void Awake()
    {
        Instance = this;
        GameObject = gameObject;
        rb = GetComponent<Rigidbody>();
        if (MaxHealth == 0) MaxHealth = Health;
    }
    void Start()
    {
        
    }

    Vector2 TargetPos = Vector2.zero;
    void Update()
    {
        Camera camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        
        bool res = Physics.Raycast(ray, out info);
        if(res) TargetPos = new Vector2(info.point.x,info.point.z);
        Debug.DrawLine(info.point, info.point + Vector3.up * 10, res? Color.red : Color.green); 
    }

    private void FixedUpdate()
    {
        Vector2 MyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 Dir = TargetPos - MyPos;
        Vector2 DirNorm = Dir.normalized;
        rb.AddForce(new Vector3(DirNorm.x,0, DirNorm.y)*speed);
        DampVelocity(0.9f, 0.9f);
        ClampVelocity(maxSpeed);
    }


    void DampVelocity(float factorX, float factorZ, float factorY = 1.0f)
    {
        Vector3 velocity = rb.velocity;
        velocity.x *= factorX;
        velocity.y *= factorY;
        velocity.z *= factorZ;
        rb.velocity = velocity;
    }

    void ClampVelocity(float maxSpeed)
    {
        Vector2 xzVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        if (xzVelocity.magnitude >= maxSpeed)
        {
            Vector2 xzVelNorm = xzVelocity.normalized;
            rb.velocity = new Vector3(xzVelNorm.x * maxSpeed, rb.velocity.y, xzVelNorm.y * maxSpeed);
        }
    }

    public bool TakeDamage(float amount, GameObject invoker)
    {
        Health-=amount;
        PlayerDamagedEvent.Invoke(invoker);
        return true;
    }
}
