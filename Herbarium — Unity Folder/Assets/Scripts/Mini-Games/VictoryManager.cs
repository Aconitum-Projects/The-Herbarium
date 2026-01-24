using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class VictoryManager : MonoBehaviour
{
    
    [Header("Pause")]
    public bool isPaused = false;
    
    [Header("UI Elements")]
    public TextMeshProUGUI victoryText, subtitleText;
    public string[] victoryMessages;
    public string[] pauseMessages =
    {
        "Need a break?",
        "Take a breath.",
        "Paused. No rush.",
        "Still here when you are."
    };
    public float endGameYOffset = -120f;
    public string pauseSubtitleMessage = "Click anywhere to get back to the game";
    public UnityEngine.UI.Button victoryButton;
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
    [Header("End Game Object")]
    public GameObject endGameObject;
    public string mainSceneName = "Gameplay";
    
    [Header("Input Delay")]
    public float startDelay = 1.0f;
    public float clickDelay = 0.5f;
    
    bool endGameActive = false;
    bool canClick = false;
    int currentIndex = 0;
    bool victoryActive = false;
    Sequence victorySequence;
    SpriteController[] disabledSpriteControllers;
    CookingController[] disabledCookingController;
    VictoryChecker[] victoryCheckers;
    Vector2 victoryTextBasePos;
    RectTransform victoryTextRect;

    void Start()
    {
        for (int i = 0; i < miniGames.Length; i++)
            miniGames[i].SetActive(i == currentIndex);
    }
    void Awake()
    {
        canClick = false;

        disabledSpriteControllers = FindObjectsByType<SpriteController>(0);
        foreach (var sc in disabledSpriteControllers)
        {
            if (sc == null || sc.ignoreVictoryFreeze) continue;
            sc.enabled = false;
        }

        disabledCookingController = FindObjectsByType<CookingController>(0);
        foreach (var cc in disabledCookingController)
        {
            if (cc == null || cc.ignoreVictoryFreeze) continue;
            cc.enabled = false;
        }

        Invoke(nameof(EnableGameplayScripts), startDelay);

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            victoryText.alpha = 0f;
            victoryText.transform.localScale = scaleFrom;
            victoryTextRect = victoryText.GetComponent<RectTransform>();
            victoryTextBasePos = victoryTextRect.anchoredPosition;
        }

        if (globalVolume != null)
            globalVolume.weight = 0f;
        
        if (victoryButton != null)
        {
            victoryButton.gameObject.SetActive(false);
            victoryButton.onClick.RemoveAllListeners();
        }
        
        victoryCheckers = FindObjectsByType<VictoryChecker>(FindObjectsSortMode.None);

    }

    void EnableGameplayScripts()
    {
        canClick = true;

        if (disabledSpriteControllers != null)
        {
            foreach (var sc in disabledSpriteControllers)
            {
                if (sc != null)
                    sc.enabled = true;
            }
        }

        if (disabledCookingController != null)
        {
            foreach (var cc in disabledCookingController)
            {
                if (cc != null)
                    cc.enabled = true;
            }
        }
    }

    void Update()
    {
        if (!victoryActive && Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPause();
            return;
        }

        if (!victoryActive || !canClick) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (endGameActive)
                {
                    if (victoryTextRect != null)
                    {
                        victoryTextRect.anchoredPosition = victoryTextBasePos;
                    }
                    UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
                }
                else
                {
                    HideVictoryAndContinue();
                }
            }
        }
    }
    
    public void TriggerVictory()
    {
        if (victoryText == null) return;

        victoryActive = true;
        canClick = false;

        DisableGameplayScripts();
        Invoke(nameof(EnableClick), clickDelay);

        victorySequence?.Kill();
        
        if (victoryMessages != null && victoryMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, victoryMessages.Length);
            victoryText.text = victoryMessages[randomIndex];
        }
        
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
        
        if (victoryButton != null)
        {
            Transform bt = victoryButton.transform;
            bt.DOKill();

            bt.localScale = scaleFrom;
            victoryButton.gameObject.SetActive(true);

            bt.DOScale(scaleTo, tweenDuration)
                .SetEase(textEaseAnim);
        }
        
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
    
    void TriggerPause()
    {
        isPaused = true;
        
        if (victoryCheckers != null)
        {
            foreach (var vc in victoryCheckers)
                vc.HideInstructions();
        }

        victoryActive = true;
        canClick = false;

        DisableGameplayScripts();
        Invoke(nameof(EnableClick), clickDelay);

        victorySequence?.Kill();

        int randomIndex = Random.Range(0, pauseMessages.Length);
        victoryText.text = pauseMessages[randomIndex];
        subtitleText.text = pauseSubtitleMessage;

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

        if (victoryButton != null)
        {
            Transform bt = victoryButton.transform;
            bt.DOKill();

            bt.localScale = scaleFrom;
            victoryButton.gameObject.SetActive(true);

            bt.DOScale(scaleTo, tweenDuration)
                .SetEase(textEaseAnim);
        }

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
    
    void DisableGameplayScripts()
    {
        disabledSpriteControllers = FindObjectsByType<SpriteController>(0);
        foreach (var sc in disabledSpriteControllers)
        {
            if (sc == null) continue;
            if (sc.ignoreVictoryFreeze) continue;

            sc.enabled = false;
        }
        
        disabledCookingController = FindObjectsByType<CookingController>(0);
        foreach (var sc in disabledCookingController)
        {
            if (sc == null) continue;
            if (sc.ignoreVictoryFreeze) continue;

            sc.enabled = false;
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
        
        if (victoryButton != null)
        {
            Transform bt = victoryButton.transform;
            bt.DOKill();

            bt.DOScale(scaleFrom, tweenDuration * 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    victoryButton.gameObject.SetActive(false);
                });
        }

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

            if (isPaused)
            {
                isPaused = false;

                if (victoryCheckers != null)
                {
                    foreach (var vc in victoryCheckers)
                        vc.ShowInstructions();
                }

                Invoke(nameof(EnableGameplayScripts), clickDelay);
            }

            else
            {
                ActivateNextMiniGame();

                if (disabledSpriteControllers != null)
                {
                    foreach (var sc in disabledSpriteControllers)
                    {
                        if (sc != null)
                            sc.enabled = true;
                    }
                }

                if (disabledCookingController != null)
                {
                    foreach (var sc in disabledCookingController)
                    {
                        if (sc != null)
                            sc.enabled = true;
                    }
                }
            }
        });

    }

    private void ActivateNextMiniGame()
    {
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

            if (endGameObject != null)
                endGameObject.SetActive(true);

            TriggerEndGameVictory();
            endGameActive = true;
        }
    }
    
    void EnableClick()
    {
        canClick = true;
    }
    private void TriggerEndGameVictory()
    {
        if (victoryText == null) return;

        victoryActive = true;
        canClick = false;

        DisableGameplayScripts();
        Invoke(nameof(EnableClick), clickDelay);

        victorySequence?.Kill();

        victoryText.text = "Mini-game completed!";
        subtitleText.text = "Click anywhere to get back to the shop";

        victoryText.gameObject.SetActive(true);
        victoryText.alpha = 0f;
        victoryText.transform.localScale = scaleFrom;
        
        if (victoryTextRect != null)
        {
            victoryTextRect.anchoredPosition =
                victoryTextBasePos + Vector2.up * endGameYOffset;
        }

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

        if (victoryButton != null)
            victoryButton.gameObject.SetActive(false);

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
    

}