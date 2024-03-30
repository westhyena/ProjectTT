using System.Collections;
using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text waveText;

    [SerializeField]
    float deactiveDuration = 5.0f;

    public void StartWave(int wave)
    {
        this.gameObject.SetActive(true);
        waveText.text = "Wave " + wave;
        StartCoroutine(DeactiveWaveText());
    }

    IEnumerator DeactiveWaveText()
    {
        yield return new WaitForSeconds(deactiveDuration);
        this.gameObject.SetActive(false);
    }
}
