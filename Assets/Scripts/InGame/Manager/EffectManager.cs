using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;
    public static EffectManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<EffectManager>();
            }
            return _instance;
        }
    }

    public GameObject followEffectPrefab;
}
