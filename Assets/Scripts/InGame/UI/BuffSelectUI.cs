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

    public void Initialize(List<UserSelectCardDataElement> cardList)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < cardList.Count; i++)
        {
            buffCardUIs[i].Initialize(cardList[i]);
        }
    }

    void OnClickBuffCard(BuffCardUI buffCardUI)
    {
        GameManager.instance.OnSelectBuffCard();
    }
}
