using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI victoryText; 
    public float tweenDuration = 0.5f;
    public Vector3 scaleFrom = Vector3.zero;
    public Vector3 scaleTo = Vector3.one;
    public Ease textEaseAnim = Ease.OutBack;

    [Header("Next Object")]
    public GameObject nextObject;

    private bool victoryActive = false;

    void Start()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            victoryText.transform.localScale = scaleFrom;
        }
    }

    void Update()
    {
        if (victoryActive && Input.GetMouseButtonDown(0))
        {
            ActivateNext();
        }
    }

    public void TriggerVictory()
    {
        if (victoryText == null) return;

        victoryActive = true;
        victoryText.gameObject.SetActive(true);
        victoryText.transform.localScale = scaleFrom;

        victoryText.transform.DOScale(scaleTo, tweenDuration).SetEase(textEaseAnim);
    }

    private void ActivateNext()
    {
        victoryActive = false;

        if (nextObject != null)
        {
            nextObject.SetActive(true);
        }

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
        }
    }
}