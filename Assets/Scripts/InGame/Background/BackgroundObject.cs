using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObject : MonoBehaviour
{
    int indexX;
    int indexY;

    float scale;

    public void Initialize(Transform mapRoot, float scale)
    {
        this.transform.parent = mapRoot;
        this.transform.localScale = Vector3.one * scale;
        this.scale = scale;
    }

    public void SetPosition(int x, int y)
    {
        this.indexX = x;
        this.indexY = y;
        this.transform.localPosition = new Vector3(
            x * scale,
            y * scale,
            0.0f
        );
    }
}
