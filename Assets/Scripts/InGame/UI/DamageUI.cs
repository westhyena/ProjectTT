using System.Collections;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    [SerializeField]
    float destroyTime = 2.0f;

    [SerializeField]
    TMP_Text damageText;

    public void Initialize(float damage)
    {
        damageText.text = ((int)damage).ToString();
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
