using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Transform mapRoot;
    public Player player;
    public GameObject backgroundPrefab;

    public float backgroundScale = 200.0f;

    List<GameObject> backgroundList;
    List<GameObject> objectPool;

    void Awake()
    {
        backgroundList = new List<GameObject>();
        objectPool = new List<GameObject>();

        CreateObject(0, 0);
        CreateObject(0, 1);
        CreateObject(0, -1);
        CreateObject(1, 0);
        CreateObject(1, 1);
        CreateObject(1, -1);
        CreateObject(-1, 0);
        CreateObject(-1, 1);
        CreateObject(-1, -1);
    }

    void CreateObject(int positionX, int positionY)
    {
        GameObject background = Instantiate(backgroundPrefab);
        BackgroundObject backgroundObject = background.AddComponent<BackgroundObject>();
        backgroundObject.Initialize(mapRoot, backgroundScale);
        backgroundObject.SetPosition(positionX, positionY);
        backgroundList.Add(background);
    }

    void UpdateBackground()
    {
        int minIndexX = GetMinIndexX(player.transform.position);
        int maxIndexX = GetMaxIndexX(player.transform.position);

        int minIndexY = GetMinIndexY(player.transform.position);
        int maxIndexY = GetMaxIndexY(player.transform.position);

        int k = 0;
        for (int i = minIndexX; i <= maxIndexX; ++i)
        {
            for (int j = minIndexY; j <= maxIndexY; ++j)
            {
                BackgroundObject backgroundObject = backgroundList[k].GetComponent<BackgroundObject>();
                backgroundObject.SetPosition(i, j);
                k += 1;
            }
        }
    }

    void Update()
    {
        UpdateBackground();
    }

    int GetMinIndexX(Vector3 position)
    {
        return Mathf.FloorToInt((position.x - backgroundScale * 1.5f) / backgroundScale) + 1;
    }

    int GetMaxIndexX(Vector3 position)
    {
        return Mathf.FloorToInt((position.x + backgroundScale * 1.5f) / backgroundScale);
    }

    int GetMinIndexY(Vector3 position)
    {
        return Mathf.FloorToInt((position.y - backgroundScale * 1.5f) / backgroundScale) + 1;
    }

    int GetMaxIndexY(Vector3 position)
    {
        return Mathf.FloorToInt((position.y + backgroundScale * 1.5f) / backgroundScale);
    }
}
