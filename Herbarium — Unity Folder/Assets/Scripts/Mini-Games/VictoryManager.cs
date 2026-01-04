using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI victoryText;
    public Volume globalVolume;

    [Header("Victory Text Anim")]
    public float tweenDuration = 0.5f;
    public Vector3 scaleFrom = Vector3.zero;
    public Vector3 scaleTo = Vector3.one;
    public Ease textEaseAnim = Ease.OutBack;
    public Ease fadeEaseAnim = Ease.OutQuad;

    [Header("Global Volume Anim")]
    public float volumeDuration = 0.4f;
    public Ease volumeEase = Ease.OutSine;

    [Header("MiniGames Sequence")]
    public GameObject[] miniGames;
    
    [Header("Input Delay")]
    public float clickDelay = 0.5f;

    private bool canClick = false;
    private int currentIndex = 0;
    private bool victoryActive = false;

    Sequence victorySequence;

    void Start()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            victoryText.alpha = 0f;
            victoryText.transform.localScale = scaleFrom;
        }

        if (globalVolume != null)
            globalVolume.weight = 0f;

        for (int i = 0; i < miniGames.Length; i++)
            miniGames[i].SetActive(i == currentIndex);
    }

    void Update()
    {
        if (victoryActive && canClick && Input.GetMouseButtonDown(0))
            HideVictoryAndContinue();
    }
    
    public void TriggerVictory()
    {
        if (victoryText == null) return;

        victoryActive = true;
        canClick = false;

        Invoke(nameof(EnableClick), clickDelay);

        victorySequence?.Kill();

        victoryText.gameObject.SetActive(true);
        victoryText.alpha = 0f;
        victoryText.transform.localScale = scaleFrom;

        victorySequence = DOTween.Sequence();

        victorySequence.Join(
            victoryText.DOFade(1f, tweenDuration * 0.6f)
                .SetEase(fadeEaseAnim)
        );

        victorySequence.Join(
            victoryText.transform
                .DOScale(scaleTo, tweenDuration)
                .SetEase(textEaseAnim)
        );

        if (globalVolume != null)
        {
            victorySequence.Join(
                DOTween.To(
                    () => globalVolume.weight,
                    x => globalVolume.weight = x,
                    1f,
                    volumeDuration
                ).SetEase(volumeEase)
            );
        }
    }

    void HideVictoryAndContinue()
    {
        victoryActive = false;

        victorySequence?.Kill();

        Sequence hideSeq = DOTween.Sequence();

        hideSeq.Join(
            victoryText.DOFade(0f, tweenDuration * 0.4f)
                .SetEase(Ease.InQuad)
        );

        hideSeq.Join(
            victoryText.transform
                .DOScale(scaleFrom, tweenDuration * 0.5f)
                .SetEase(Ease.InBack)
        );

        if (globalVolume != null)
        {
            hideSeq.Join(
                DOTween.To(
                    () => globalVolume.weight,
                    x => globalVolume.weight = x,
                    0f,
                    volumeDuration * 0.8f
                ).SetEase(Ease.InSine)
            );
        }

        hideSeq.OnComplete(() =>
        {
            victoryText.gameObject.SetActive(false);
            ActivateNextMiniGame();
        });
    }

    private void ActivateNextMiniGame()
    {
        if (miniGames.Length == 0) return;

        if (currentIndex < miniGames.Length)
            miniGames[currentIndex].SetActive(false);

        currentIndex++;

        if (currentIndex < miniGames.Length)
            miniGames[currentIndex].SetActive(true);
        else
            Debug.Log("Tous les mini-jeux sont complétés.");
    }
    
    void EnableClick()
    {
        canClick = true;
    }

}