using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionPointUI : MonoBehaviour
{
    [SerializeField]
    Slider gaugeBar;
    [SerializeField]
    TMP_Text pointText;

    void Update()
    {
        gaugeBar.value = GameManager.instance.CompanionGauge;
        pointText.text = GameManager.instance.CompanionPoints.ToString();
    }
}
