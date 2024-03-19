using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Transform character;
    public float smoothSpeed = 0.125f; // 카메라가 캐릭터를 따라가는 속도
    private Vector3 previousCharacterPosition;
    private Vector3 targetCameraPosition;

    void Start()
    {
        character = GameManager.instance.Player.transform;
        if (character != null)
        {
            previousCharacterPosition = character.position;
            targetCameraPosition = transform.position;
        }
    }

    void LateUpdate()
    {
        if (character == null) return;

        // 캐릭터의 현재 위치와 이전 위치의 차이를 계산
        Vector3 characterMovement = character.position - previousCharacterPosition;

        // 카메라가 도달해야 할 목표 위치 계산
        targetCameraPosition += characterMovement;

        // 카메라 위치 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, smoothSpeed);

        // 캐릭터의 현재 위치를 저장
        previousCharacterPosition = character.position;
    }
}