using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float delay = 5f; // ������ �������� ���� �ð�

    void Start()
    {
        // delay �� �Ŀ� Destroy �޼��带 ȣ���Ͽ� �� ���� ������Ʈ�� ����
        Destroy(gameObject, delay);
    }
}
