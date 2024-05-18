using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CompanionCallUI : MonoBehaviour
{
    Button button;

    [SerializeField]
    TMP_Text needPointText;
    Color originalColor;

    void Awake()
    {
        button = GetComponent<Button>();
        originalColor = needPointText.color;
    }

    void Start()
    {
        button.onClick.AddListener(GameManager.instance.CallCompanion);
    }

    void Update()
    {
        needPointText.text = GameManager.instance.CompanionCallPoint.ToString();
        if (GameManager.instance.CompanionCallPoint > GameManager.instance.CompanionPoints)
        {
            needPointText.color = Color.red;
        }
        else
        {
            needPointText.color = originalColor;
        }
    }
}
