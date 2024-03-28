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

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        button.onClick.AddListener(GameManager.instance.SummonCompanion);
    }

    void Update()
    {
        needPointText.text = GameManager.instance.CompanionSummonPoint.ToString();
    }
}
