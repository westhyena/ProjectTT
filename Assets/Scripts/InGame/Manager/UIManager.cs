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

    protected GameObject hpBarPrefab;
    public HPBarUI CreateHPBar(Transform parent)
    {
        if (null == hpBarPrefab)
        {
            hpBarPrefab = Resources.Load<GameObject>("UI/HPBar");
        }
        return Instantiate(hpBarPrefab, parent).GetComponent<HPBarUI>();
    }
}
