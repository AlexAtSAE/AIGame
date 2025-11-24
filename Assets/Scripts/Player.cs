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
    private void Awake()
    {
        Instance = this;
        GameObject = gameObject;
        rb = GetComponent<Rigidbody>();
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
    }
}
