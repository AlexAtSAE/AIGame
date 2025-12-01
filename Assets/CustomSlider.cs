using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public class CustomSlider : MonoBehaviour
{
    private CanvasRenderer canvasRenderer;
    private RectTransform rect;
    [SerializeField] private Material mat;
    private Mesh mesh;
    public float percentFull;
    //public float barWidth { get { return barWidth; } set { barWidth = value; recalcMesh();  } }
    //public float barHeight { get { return barHeight; } set { barHeight = value; recalcMesh();  } }
    public float barWidth;
    public float barHeight;
    private float roundnessGauge;

    private void Awake()
    {
        
        
        mat = new Material(mat);
        float w = barWidth;
        float h = barHeight;
        canvasRenderer.SetMesh(mesh);
        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(mat,0);
        
    }

    private void Start()
    {
        Player.Instance.PlayerDamagedEvent += PlayerDamaged;
    }

    void Update()
    {
        //barWidth = rect.rect.width;
        //barHeight = rect.rect.height;
        roundnessGauge-=Time.deltaTime;

        mat.SetFloat("_Percent", percentFull);
        mat.SetFloat("_Roundness", roundnessGauge);
    }

    private void PlayerDamaged(GameObject obj)
    {
        percentFull = Player.Instance.Health/ Player.Instance.MaxHealth;
        roundnessGauge = 1.0f;
    }
    private void OnValidate()
    {
        canvasRenderer = GetComponent<CanvasRenderer>();
        rect = GetComponent<RectTransform>();
        
        recalcMesh();
        mat.SetFloat("_XYRatio", barWidth/barHeight);
        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(mat, 0);
        canvasRenderer.SetMesh(mesh);
    }

    void recalcMesh()
    {
        mesh = new Mesh();
        float w = barWidth;
        float h = barHeight;
        mesh.vertices = new Vector3[4] {
            new Vector3(-w/2, -h),
            new Vector3(-w/2, 0),
            new Vector3(w/2, -h),
            new Vector3(w/2, 0),
        };
        mesh.uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1),
        };
        mesh.triangles = new int[6] { 0, 1, 3, 0, 3, 2 };
    }
}
