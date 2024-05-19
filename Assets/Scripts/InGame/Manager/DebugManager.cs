using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager _instance;
    public static DebugManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<DebugManager>();
            }
            return _instance;
        }
    }

    public bool isDebugMode = false;

    public GameObject circleRangePrefab;

    public bool isPlayerAuto = false;
}
