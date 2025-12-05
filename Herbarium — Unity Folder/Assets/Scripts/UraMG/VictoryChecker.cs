using System.Collections.Generic;
using UnityEngine;

public enum VictoryType
{
    MatchingColors,
    AllDetached,
    AllCut,
    AllRevealed
}

public class VictoryChecker : MonoBehaviour
{
    public VictoryType victoryType;

    public List<ColorDetector> colorsDetectors;
    public List<DetachDetector> detachDetectors;
    public List<CutDetector> cutDetectors;
    public List<RevealedDetector> revealedDetectors;

    public VictoryManager victoryManager;

    private bool victoryTriggered = false;

    void Start()
    { 
        if (victoryManager == null)
            victoryManager = GetComponentInParent<VictoryManager>();

        if (victoryManager == null)
            victoryManager = FindObjectOfType<VictoryManager>();

        if (victoryManager == null)
            Debug.LogWarning("VictoryChecker : Aucun VictoryManager trouvé dans la scène.");
    }

    // --- MAIN CHECK ---
    void TriggerIfComplete(bool condition)
    {
        if (victoryTriggered) return;
        if (!condition) return;
        if (victoryManager == null) return;

        victoryTriggered = true;
        victoryManager.TriggerVictory();
    }

    // ------------------------------------------------------
    // Called by one of the detectors whenever it validates.
    // ------------------------------------------------------

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
}
