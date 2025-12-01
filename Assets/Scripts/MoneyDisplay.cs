using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    GameManagerScript gameManager;
    [SerializeField] private TextMeshProUGUI text;
    void Start()
    {
        gameManager = GameManagerScript.instance;
        GameManagerScript.moneyChanged += UpdateText;
    }

    
    void UpdateText(GameObject obj)
    {
        text.text = $"${GameManagerScript.money}";
    }
}
