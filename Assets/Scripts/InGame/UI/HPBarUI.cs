using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Slider hpBar;

    Character character;

    public void Initialize(Character character)
    {
        this.character = character;
    }

    void Update()
    {
        if (null == character)
        {
            return;
        }

        hpBar.value = character.HPRatio;
    }
}
