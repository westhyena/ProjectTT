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

    void Awake()
    {
        backgroundList = new List<GameObject>();

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
        background.transform.parent = mapRoot;
        background.transform.localScale = Vector3.one * backgroundScale;
        background.transform.localPosition = new Vector3(
            positionX * backgroundScale,
            positionY * backgroundScale,
            0.0f
        );
        backgroundList.Add(background);
    }

    void Update()
    {
        foreach (GameObject backgroundObject in backgroundList)
        {
            //player.transform.position
        }
    }
}
