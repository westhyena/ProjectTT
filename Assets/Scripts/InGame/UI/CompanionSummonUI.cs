using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionSummonUI : MonoBehaviour
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
        button.onClick.AddListener(GameManager.instance.SummonCompanion);
    }

    void Update()
    {
        needPointText.text = GameManager.instance.CompanionSummonPoint.ToString();
        if (GameManager.instance.CompanionSummonPoint > GameManager.instance.CompanionPoints)
        {
            needPointText.color = Color.red;
        }
        else
        {
            needPointText.color = originalColor;
        }
    }
}
