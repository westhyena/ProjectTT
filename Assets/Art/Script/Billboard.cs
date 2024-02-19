using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        // ���� ī�޶��� ��ġ�� ���ɴϴ�.
        Transform cameraTransform = Camera.main.transform;

        // �����尡 �׻� ī�޶� ���ϵ��� �մϴ�.
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
            cameraTransform.rotation * Vector3.up);
    }
}
