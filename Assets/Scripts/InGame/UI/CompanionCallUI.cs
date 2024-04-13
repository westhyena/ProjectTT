using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CompanionCallUI : MonoBehaviour
{
    Button button;

    [SerializeField]
    TMP_Text needPointText;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        button.onClick.AddListener(GameManager.instance.CallCompanion);
    }

    void Update()
    {
        needPointText.text = GameManager.instance.CompanionCallPoint.ToString();
    }
}
