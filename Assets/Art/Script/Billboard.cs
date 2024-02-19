using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        // 현재 카메라의 위치를 얻어옵니다.
        Transform cameraTransform = Camera.main.transform;

        // 빌보드가 항상 카메라를 향하도록 합니다.
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
            cameraTransform.rotation * Vector3.up);
    }
}
