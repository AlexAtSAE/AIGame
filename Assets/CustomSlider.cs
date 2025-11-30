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
    public float percentFull;
    //private float barWidth;
    //private float barHeight;

    private float roundnessGauge;

    private void Awake()
    {
        canvasRenderer = GetComponent<CanvasRenderer>();
        rect = GetComponent<RectTransform>();
        Mesh mesh = new Mesh();
        mat = new Material(mat);
        //mat = new Material(Shader.Find("Unlit/SliderShader"));
        mesh.vertices = new Vector3[4] {
            new Vector3(-.5f*rect.rect.width, -.5f*rect.rect.height),
            new Vector3(-.5f*rect.rect.width, .5f*rect.rect.height),
            new Vector3(.5f*rect.rect.width, .5f*rect.rect.height),
            new Vector3(.5f*rect.rect.width, -.5f*rect.rect.height)
        };
        mesh.uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        }; 
        mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };

        canvasRenderer.SetMesh(mesh);
        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(mat,0);

        Debug.Log("set mesh");
        
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
}
