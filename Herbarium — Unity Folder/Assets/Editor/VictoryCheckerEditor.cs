using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VictoryChecker))]
[CanEditMultipleObjects]
public class VictoryCheckerEditor : Editor
{
    SerializedProperty victoryType, victoryManager;

    private SerializedProperty
        colorsDetectors,
        detachDetectors,
        cutedDetectors,
        revealedDetectors,
        shakeDetectors,
        filledDetectors,
        erasedDetectors,
        stoppedDetectors,
        trailDetectors,
        collectedDetectors,
        pointsDetectors,
        cookingControllers;
    
    SerializedProperty
        instructionCanvas,
        instructionRect,
        animDuration,
        animEase;

    void OnEnable()
    {
        victoryType = serializedObject.FindProperty("victoryType");

        colorsDetectors   = serializedObject.FindProperty("colorsDetectors");
        detachDetectors   = serializedObject.FindProperty("detachDetectors");
        cutedDetectors    = serializedObject.FindProperty("cutDetectors");
        revealedDetectors = serializedObject.FindProperty("revealedDetectors");
        shakeDetectors    = serializedObject.FindProperty("shakeDetectors");
        filledDetectors   = serializedObject.FindProperty("filledDetectors");
        erasedDetectors   = serializedObject.FindProperty("erasedDetectors");
        stoppedDetectors  = serializedObject.FindProperty("stoppedDetectors");
        trailDetectors    = serializedObject.FindProperty("trailDetectors");
        collectedDetectors= serializedObject.FindProperty("collectedDetectors");
        pointsDetectors   = serializedObject.FindProperty("pointsDetectors");
        cookingControllers  = serializedObject.FindProperty("cookingControllers");

        victoryManager = serializedObject.FindProperty("victoryManager");

        // Instruction UI
        instructionCanvas = serializedObject.FindProperty("instructionCanvas");
        instructionRect   = serializedObject.FindProperty("instructionRect");
        animDuration      = serializedObject.FindProperty("animDuration");
        animEase          = serializedObject.FindProperty("animEase");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleLeft
        };

        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleLeft
        };

        serializedObject.Update();
        EditorGUILayout.Space(10);

        // --------------------
        // Victory
        // --------------------
        EditorGUILayout.LabelField("Victory Checker Settings", bigTitle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(victoryType);
        EditorGUILayout.Space(15);

        if (!victoryType.hasMultipleDifferentValues)
        {
            VictoryType type = (VictoryType)victoryType.enumValueIndex;

            switch (type)
            {
                case VictoryType.MatchingColors:
                    EditorGUILayout.LabelField("Matching Colors Detectors", middleTitle);
                    EditorGUILayout.PropertyField(colorsDetectors, true);
                    break;

                case VictoryType.AllDetached:
                    EditorGUILayout.LabelField("Detach Detectors", middleTitle);
                    EditorGUILayout.PropertyField(detachDetectors, true);
                    break;

                case VictoryType.AllCut:
                    EditorGUILayout.LabelField("Cut Detectors", middleTitle);
                    EditorGUILayout.PropertyField(cutedDetectors, true);
                    break;

                case VictoryType.AllRevealed:
                    EditorGUILayout.LabelField("Revealed Detectors", middleTitle);
                    EditorGUILayout.PropertyField(revealedDetectors, true);
                    break;

                case VictoryType.AllShake:
                    EditorGUILayout.LabelField("Shake Detectors", middleTitle);
                    EditorGUILayout.PropertyField(shakeDetectors, true);
                    break;

                case VictoryType.AllFilled:
                    EditorGUILayout.LabelField("Filled Detectors", middleTitle);
                    EditorGUILayout.PropertyField(filledDetectors, true);
                    break;

                case VictoryType.AllErased:
                    EditorGUILayout.LabelField("Erased Detectors", middleTitle);
                    EditorGUILayout.PropertyField(erasedDetectors, true);
                    break;

                case VictoryType.AllStopped:
                    EditorGUILayout.LabelField("Stopped Detectors", middleTitle);
                    EditorGUILayout.PropertyField(stoppedDetectors, true);
                    break;

                case VictoryType.TrailValidated:
                    EditorGUILayout.LabelField("Trail Detectors", middleTitle);
                    EditorGUILayout.PropertyField(trailDetectors, true);
                    break;

                case VictoryType.AllCollected:
                    EditorGUILayout.LabelField("Collected Detectors", middleTitle);
                    EditorGUILayout.PropertyField(collectedDetectors, true);
                    break;

                case VictoryType.AllPoints:
                    EditorGUILayout.LabelField("Points Detectors", middleTitle);
                    EditorGUILayout.PropertyField(pointsDetectors, true);
                    break;

                case VictoryType.Cooked:
                    EditorGUILayout.LabelField("Cooking Controllers", middleTitle);
                    EditorGUILayout.PropertyField(cookingControllers, true);
                    break;
            }

            EditorGUILayout.Space(15);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Les objets sélectionnés ont des types de victoire différents.",
                MessageType.Info
            );
            EditorGUILayout.Space(15);
        }

        // --------------------
        // Instruction UI
        // --------------------
        EditorGUILayout.LabelField("Instruction Canvas (On Victory)", bigTitle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(instructionCanvas);
        EditorGUILayout.PropertyField(instructionRect);

        if (instructionCanvas.objectReferenceValue != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Animation", middleTitle);
            EditorGUILayout.PropertyField(animDuration);
            EditorGUILayout.PropertyField(animEase);
        }

        EditorGUILayout.Space(20);

        // --------------------
        // Victory Manager
        // --------------------
        EditorGUILayout.LabelField("Victory Manager", bigTitle);
        EditorGUILayout.PropertyField(victoryManager);

        EditorGUILayout.Space(20);
        serializedObject.ApplyModifiedProperties();
    }
}