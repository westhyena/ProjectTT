using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    GameObject hpBarPrefab;
    public HPBarUI CreateHPBar(Transform parent)
    {
        return Instantiate(hpBarPrefab, parent).GetComponent<HPBarUI>();
    }

    [SerializeField]
    GameObject playerHPBarPrefab;
    public HPBarUI CreatePlayerHPBar(Transform parent)
    {
        return Instantiate(playerHPBarPrefab, parent).GetComponent<HPBarUI>();
    }

    [SerializeField]
    GameObject damagePrefab;
}
