using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionGroupUI : MonoBehaviour
{
    public CompanionUI[] companionUIs;

    void Start()
    {
        for (int i = 0; i < companionUIs.Length; i++)
        {
            bool isActive = i < HeroManager.instance.companionCharacterIds.Length;
            companionUIs[i].gameObject.SetActive(isActive);
            if (isActive)
            {
                
                companionUIs[i].Initialize(HeroManager.instance.CompanionCharacterInfos[i]);
            }
        }
    }
}
