using System.Collections.Generic;
using UnityEngine;

public enum VictoryType
{
    MatchingColors,
    AllDetached,
    AllCut,
    AllRevealed,
    AllShake,
    AllFilled,
    AllErased,
    AllStopped
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

    public VictoryManager victoryManager;

    private bool victoryTriggered = false;

    void Start()
    { 
        if (victoryManager == null)
            victoryManager = GetComponentInParent<VictoryManager>();

        if (victoryManager == null)
            victoryManager = FindFirstObjectByType<VictoryManager>();

        if (victoryManager == null)
            Debug.LogWarning("VictoryChecker : Aucun VictoryManager trouvé dans la scène.");
    }
    void TriggerIfComplete(bool condition)
    {
        if (victoryTriggered) return;
        if (!condition) return;
        if (victoryManager == null) return;

        victoryTriggered = true;
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

}
