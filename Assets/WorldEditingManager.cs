using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldEditingManager : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject wallPreviewPrefab;
    public LayerMask mask;

    private bool placingMode;
    private bool previousFramePlacingMode;
    private float placeRotation;
    public static WorldEditingManager instance;
    private GameObject wallPreview;


    public static Vector2 mousePos;
    private void Update()
    {
        //change from false to true
        if(previousFramePlacingMode == false && placingMode == true)
        {
            wallPreview = Instantiate(wallPreviewPrefab);
        }
        //change from true to false
        if (previousFramePlacingMode == true && placingMode == false)
        {
            Destroy(wallPreview);
        }
        previousFramePlacingMode = placingMode;
        if (placingMode)
        {
            
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            bool res = Physics.Raycast(ray, out info);
            if (res) mousePos = new Vector2(info.point.x, info.point.z);

            wallPreview.transform.position = new Vector3(mousePos.x,0.5f, mousePos.y);
            wallPreview.transform.rotation = Quaternion.Euler(0, placeRotation, 0);

            placeRotation += Input.mouseScrollDelta.y * 10;

            if (Input.GetMouseButtonDown(0)) { 
                bool success = Place();
                if (success)
                {
                    placingMode = false;
                    GameManagerScript.money -= 20;
                    GameManagerScript.moneyChanged.Invoke(gameObject);
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                CancelPlace();
                placingMode = false;
            }

        }
    }
    public void BeginPlacingWall()
    {
        placingMode = true;
    }
    bool Place()
    {
        Debug.Log("Attempting place");
        Instantiate(wallPrefab, new Vector3(mousePos.x, 0.5f, mousePos.y), Quaternion.Euler(0, placeRotation,0));
        return true;
    }
    void CancelPlace()
    {
        Debug.Log("Cancelling place");
    }
}
