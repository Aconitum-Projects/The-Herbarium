using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickFeedback : MonoBehaviour
{
    public GameObject feedbackPanel;
    public float feedbackDuration = 1f;
    public float scaleDuration = 0.5f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowFeedback(Input.mousePosition);
        }
    }

    private void ShowFeedback(Vector3 clickPosition)
    {
        feedbackPanel.SetActive(true);

        RectTransform rt = feedbackPanel.GetComponent<RectTransform>();
        rt.position = clickPosition;

        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, scaleDuration).SetEase(DG.Tweening.Ease.OutBack);

        StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(feedbackDuration);

        RectTransform rt = feedbackPanel.GetComponent<RectTransform>();
        rt.DOScale(Vector3.zero, scaleDuration).SetEase(DG.Tweening.Ease.InBack);

        yield return new WaitForSeconds(scaleDuration);

        feedbackPanel.SetActive(false);
    }
}