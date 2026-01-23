using UnityEngine;
using TMPro;
using System.Collections;

public class TypingDots : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public string baseText = "Chargement"; // ton texte sans les points
    public float interval = 0.5f; // temps entre chaque point

    private void OnEnable()
    {
        if (targetText != null)
            StartCoroutine(DotsCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DotsCoroutine()
    {
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4; // 1, 2, 3, 0
            targetText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(interval);
        }
    }
}