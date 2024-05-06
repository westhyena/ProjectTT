using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSelectUI : MonoBehaviour
{
    [SerializeField]
    BuffCardUI[] buffCardUIs;

    void Start()
    {
        foreach (BuffCardUI buffCardUI in buffCardUIs)
        {
            buffCardUI.button.onClick.AddListener(() =>
            {
                OnClickBuffCard(buffCardUI);
            });
        }
    }

    void OnClickBuffCard(BuffCardUI buffCardUI)
    {
        GameManager.instance.OnSelectBuffCard();
    }
}
