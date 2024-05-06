using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffCardUI : MonoBehaviour
{
    public Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
}
