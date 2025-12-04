using UnityEngine;
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

    [Header("MiniGames Sequence")]
    public GameObject[] miniGames;
    private int currentIndex = 0;

    private bool victoryActive = false;

    void Start()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            victoryText.transform.localScale = scaleFrom;
        }

        for (int i = 0; i < miniGames.Length; i++)
            miniGames[i].SetActive(i == currentIndex);
    }

    void Update()
    {
        if (victoryActive && Input.GetMouseButtonDown(0))
            ActivateNextMiniGame();
    }

    public void TriggerVictory()
    {
        if (victoryText == null) return;

        victoryActive = true;
        victoryText.gameObject.SetActive(true);
        victoryText.transform.localScale = scaleFrom;

        victoryText.transform.DOScale(scaleTo, tweenDuration)
            .SetEase(textEaseAnim);
    }

    private void ActivateNextMiniGame()
    {
        victoryActive = false;

        victoryText?.gameObject.SetActive(false);

        if (miniGames.Length == 0) return;

        if (currentIndex < miniGames.Length)
            miniGames[currentIndex].SetActive(false);

        currentIndex++;

        if (currentIndex < miniGames.Length)
        {
            miniGames[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("Tous les mini-jeux sont complétés.");
        }
    }
}