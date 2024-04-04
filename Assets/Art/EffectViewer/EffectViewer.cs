using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EffectViewer : MonoBehaviour
{
    public List<GameObject> Prjectile_Effects; // A ����Ʈ ����Ʈ
    public List<GameObject> Hit_Effects; // F ����Ʈ ����Ʈ
    public List<Button> effectButtons; // �� ����Ʈ�� �ش��ϴ� ��ư ����Ʈ
    public List<TMP_InputField> inputFields; // �� ��ư �Ʒ��� �Է� �ʵ� ����Ʈ
    public GameObject targetObject; // Ÿ�� ������Ʈ
    public GameObject spawnPointObject; // ����Ʈ ���� ��ġ ������Ʈ
    public Slider speedSlider; // �̵� �ӵ��� �����ϴ� �����̴�
    public float speed = 10.0f; // ����Ʈ �ӵ�

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

                // �Է� �ʵ忡 A EFFECT �̸� ����
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
