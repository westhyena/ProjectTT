using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float delay = 5f; // 삭제될 때까지의 지연 시간

    void Start()
    {
        // delay 초 후에 Destroy 메서드를 호출하여 이 게임 오브젝트를 삭제
        Destroy(gameObject, delay);
    }
}
