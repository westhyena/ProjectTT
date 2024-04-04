using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EffectViewer : MonoBehaviour
{
    public List<GameObject> Prjectile_Effects; // A 이펙트 리스트
    public List<GameObject> Hit_Effects; // F 이펙트 리스트
    public List<Button> effectButtons; // 각 이펙트에 해당하는 버튼 리스트
    public List<TMP_InputField> inputFields; // 각 버튼 아래의 입력 필드 리스트
    public GameObject targetObject; // 타겟 오브젝트
    public GameObject spawnPointObject; // 이펙트 생성 위치 오브젝트
    public Slider speedSlider; // 이동 속도를 조절하는 슬라이더
    public float speed = 10.0f; // 이펙트 속도

    void Start()
    {
        InitializeEffectNames();
        speedSlider.minValue = 1f;
        speedSlider.maxValue = 50f;
        speedSlider.value = speed;
        speedSlider.onValueChanged.AddListener(delegate { SpeedValueChanged(); });
    }

    private void InitializeEffectNames()
    {
        Debug.Log("Initializing Effect Names...");

        for (int i = 0; i < effectButtons.Count; i++)
        {
            TextMeshProUGUI textComponent = effectButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent == null)
            {
                Debug.LogError("No TextMeshProUGUI component found for button at index " + i);
                continue;
            }

            if (i < Prjectile_Effects.Count && Prjectile_Effects[i] != null)
            {
                textComponent.text = Prjectile_Effects[i].name;
                Debug.Log("Set text for button " + i + ": " + Prjectile_Effects[i].name);

                // 입력 필드에 A EFFECT 이름 설정
                if (i < inputFields.Count)
                {
                    inputFields[i].text = Prjectile_Effects[i].name;
                }
                else
                {
                    Debug.LogWarning("No input field found for button at index " + i);
                }
            }
            else
            {
                textComponent.text = "";
                Debug.LogWarning("No effect found for button at index " + i);
            }
        }
    }

    //public void SetName(string targetName , string name, ref List<GameObject> list)
    //{
    //    foreach (var obj in list)
    //    {
    //        if(obj.name)
    //    }
          
    //}

    public void TriggerEffectFromButton(int index)
    {
        if (index >= 0 && index < Prjectile_Effects.Count)
        {
            LaunchEffect(index);
        }
    }

    private void LaunchEffect(int index)
    {
        if (index < 0 || index >= Prjectile_Effects.Count) return;

        GameObject effect = Instantiate(Prjectile_Effects[index], spawnPointObject.transform.position, Quaternion.identity);
        StartCoroutine(MoveEffectToTarget(effect, index));
    }

    private IEnumerator MoveEffectToTarget(GameObject effect, int index)
    {
        while (effect != null && targetObject != null && Vector3.Distance(effect.transform.position, targetObject.transform.position) > 0.1f)
        {
            effect.transform.position = Vector3.MoveTowards(effect.transform.position, targetObject.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (effect != null && targetObject != null)
        {
            TriggerFEffectAtTarget(index);
            Destroy(effect);
        }
    }

    private void TriggerFEffectAtTarget(int aEffectIndex)
    {
        if (aEffectIndex < Hit_Effects.Count)
        {
            GameObject fEffectInstance = Instantiate(Hit_Effects[aEffectIndex], targetObject.transform.position, Quaternion.identity);
            Destroy(fEffectInstance, 3f);
        }
    }

    public void SpeedValueChanged()
    {
        speed = speedSlider.value;
    }
}
