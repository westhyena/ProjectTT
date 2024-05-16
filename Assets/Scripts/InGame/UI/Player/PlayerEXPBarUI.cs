using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEXPBarUI : MonoBehaviour
{
    [SerializeField]
    Slider expBar;

    void Update()
    {
        expBar.value = (float)GameManager.instance.PlayerExp / (float)GameManager.instance.MaxExp;
    }
}
