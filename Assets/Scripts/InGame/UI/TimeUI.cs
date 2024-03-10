using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text timeText;

    void Update()
    {
        int intTime = Mathf.FloorToInt(GameManager.instance.GameTime);
        int minute = intTime / 60;
        int second = intTime % 60;
        timeText.text = string.Format("{0:00}:{1:00}", minute, second);
    }
}
