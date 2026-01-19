using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum VictoryType
{
    MatchingColors,
    AllDetached,
    AllCut,
    AllRevealed,
    AllShake,
    AllFilled,
    AllErased,
    AllStopped,
    TrailValidated,
    AllCollected,
    AllPoints,
    Cooked,
    AllTouched
}

public class VictoryChecker : MonoBehaviour
{
    public VictoryType victoryType;

    public List<ColorDetector> colorsDetectors;
    public List<DetachDetector> detachDetectors;
    public List<CutDetector> cutDetectors;
    public List<RevealedDetector> revealedDetectors;
    public List<ShakeDetector> shakeDetectors;
    public List<FilledDetector> filledDetectors;
    public List<ErasedDetector> erasedDetectors;
    public List<StoppableDetector> stoppedDetectors;
    public List<TrailDetector> trailDetectors;
    public List<CollectedDetector> collectedDetectors;
    public List<PointsDetector> pointsDetectors;
    public List<CookingController> cookingControllers;
    public List<TouchedDetector> touchedDetectors;

    public VictoryManager victoryManager;
    public CanvasGroup instructionCanvas;
    public RectTransform instructionRect;
    public float animDuration = 0.35f;
    public Ease animEase = Ease.OutQuad;

    private bool victoryTriggered = false;
    private Sequence instructionSequence;

    void Awake()
    {
        if (victoryManager == null)
            victoryManager = GetComponentInParent<VictoryManager>();

        if (victoryManager == null)
            victoryManager = FindFirstObjectByType<VictoryManager>();

        if (victoryManager == null)
            Debug.LogWarning("VictoryChecker : Aucun VictoryManager trouvé dans la scène.");

        InitInstructionCanvas();
        ShowInstructionCanvas();
    }

    // --------------------
    // INIT
    // --------------------
    void InitInstructionCanvas()
    {
        if (instructionCanvas == null || instructionRect == null) return;

        instructionCanvas.gameObject.SetActive(true);
        instructionCanvas.alpha = 0f;
        instructionCanvas.interactable = false;
        instructionCanvas.blocksRaycasts = false;

        instructionRect.localScale = new Vector3(0.8f, 1.2f, 1f);
    }

    // --------------------
    // SHOW
    // --------------------
    void ShowInstructionCanvas()
    {
        if (instructionCanvas == null || instructionRect == null) return;

        instructionSequence?.Kill();

        instructionCanvas.gameObject.SetActive(true);

        instructionSequence = DOTween.Sequence();

        instructionSequence.Join(
            instructionCanvas.DOFade(1f, animDuration)
        );

        instructionSequence.Join(
            instructionRect.DOScale(Vector3.one, animDuration)
                .SetEase(animEase)
        );

        instructionSequence.OnComplete(() =>
        {
            instructionCanvas.interactable = true;
            instructionCanvas.blocksRaycasts = true;
        });
    }

    // --------------------
    // HIDE
    // --------------------
    void HideInstructionCanvas()
    {
        if (instructionCanvas == null || instructionRect == null) return;

        instructionSequence?.Kill();

        instructionCanvas.interactable = false;
        instructionCanvas.blocksRaycasts = false;

        instructionSequence = DOTween.Sequence();

        instructionSequence.Join(
            instructionRect.DOScale(
                new Vector3(1.75f, 0.25f, 1f),
                animDuration * 0.5f
            ).SetEase(animEase)
        );

        instructionSequence.Join(
            instructionCanvas.DOFade(0f, animDuration)
        );

        instructionSequence.OnComplete(() =>
        {
            instructionCanvas.gameObject.SetActive(false);
        });
    }

    void TriggerIfComplete(bool condition)
    {
        if (victoryTriggered) return;
        if (!condition) return;

        victoryTriggered = true;

        HideInstructionCanvas();

        if (victoryManager != null)
            victoryManager.TriggerVictory();
    }

    public bool CheckMatchingColors()
    {
        if (colorsDetectors == null || colorsDetectors.Count == 0) 
            return false;

        foreach (var d in colorsDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }

    public bool CheckAllDetached()
    {
        if (detachDetectors == null || detachDetectors.Count == 0) 
            return false;

        foreach (var d in detachDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }

    public bool CheckAllCut()
    {
        if (cutDetectors == null || cutDetectors.Count == 0) 
            return false;

        foreach (var d in cutDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllRevealed()
    {
        if (cutDetectors == null || revealedDetectors.Count == 0) 
            return false;

        foreach (var d in revealedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllShake()
    {
        if (shakeDetectors == null || shakeDetectors.Count == 0) 
            return false;

        foreach (var d in shakeDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllFilled()
    {
        if (filledDetectors == null || filledDetectors.Count == 0)
            return false;

        foreach (var d in filledDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllErased()
    {
        if (erasedDetectors == null || erasedDetectors.Count == 0)
            return false;

        foreach (var d in erasedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllStopped()
    {
        if (stoppedDetectors == null || stoppedDetectors.Count == 0)
            return false;

        foreach (var d in stoppedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckTrailValidated()
    {
        if (trailDetectors == null || trailDetectors.Count == 0)
            return false;

        foreach (var d in trailDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllCollected()
    {
        if (collectedDetectors == null || collectedDetectors.Count == 0)
            return false;

        foreach (var d in collectedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllPoints()
    {
        if (pointsDetectors == null || pointsDetectors.Count == 0)
            return false;

        foreach (var d in pointsDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckCooked()
    {
        if (cookingControllers == null || cookingControllers.Count == 0)
            return false;

        foreach (var d in cookingControllers)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
    public bool CheckAllTouched()
    {
        if (touchedDetectors == null || touchedDetectors.Count == 0)
            return false;

        foreach (var d in touchedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }

}
