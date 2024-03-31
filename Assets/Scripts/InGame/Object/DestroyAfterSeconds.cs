using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds = 3.0f;
    void Start()
    {
        Destroy(gameObject, seconds);
    }
}
