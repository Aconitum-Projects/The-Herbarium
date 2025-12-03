using System.Collections.Generic;
using UnityEngine;

public class VictoryChecker : MonoBehaviour
{
    [Header("Color Detectors")]
    public List<ColorDetector> detectors;

    [Header("Victory Manager")]
    public VictoryManager victoryManager;

    private bool victoryTriggered = false;

    void Update()
    {
        if (victoryTriggered) return;

        if (detectors.Count == 0 || victoryManager == null) return;

        // Vérifie que tous les validated sont true
        bool allValidated = true;
        foreach (var detector in detectors)
        {
            if (detector == null || !detector.validated)
            {
                allValidated = false;
                break;
            }
        }

        if (allValidated)
        {
            victoryTriggered = true;
            victoryManager.TriggerVictory();
        }
    }
}