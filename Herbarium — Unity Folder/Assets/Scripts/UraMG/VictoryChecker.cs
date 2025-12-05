using System.Collections.Generic;
using UnityEngine;

public enum VictoryType
{
    MatchingColors,
    AllDetached,
    AllCuted
}

public class VictoryChecker : MonoBehaviour
{
    [Header("Settings")]
    public VictoryType victoryType;

    [Header("Matching Colors")]
    public List<ColorDetector> colorsDetectors;

    [Header("All Detached")]
    public List<DetachDetector> detachDetectors;

    [Header("All Cuted")]
    public List<CutedDetector> cutedDetectors;

    [Header("Victory Manager")]
    public VictoryManager victoryManager;

    private bool victoryTriggered = false;

    void Start()
    { 
        // 1. Cherche dans les parents
        if (victoryManager == null)
            victoryManager = GetComponentInParent<VictoryManager>();

        // 2. Cherche globalement si toujours rien
        if (victoryManager == null)
            victoryManager = FindObjectOfType<VictoryManager>();

        // 3. Warn si toujours rien
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

    public bool CheckAllCuted()
    {
        if (cutedDetectors == null || cutedDetectors.Count == 0) 
            return false;

        foreach (var d in cutedDetectors)
        {
            if (d == null || !d.validated)
                return false;
        }

        TriggerIfComplete(true);
        return true;
    }
}
