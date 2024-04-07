using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterCountUI : MonoBehaviour
{
    public TMP_Text countText;

    void Update()
    {
        countText.text = EnemyManager.instance.AliveEnemyList.Count.ToString();
    }
}
