using System.Security;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VictoryChecker))]
[CanEditMultipleObjects]
public class VictoryCheckerEditor : Editor
{
    SerializedProperty victoryType;

    SerializedProperty colorsDetectors;
    SerializedProperty detachDetectors;
    SerializedProperty cutedDetectors;
    SerializedProperty revealedDetectors;
    SerializedProperty shakeDetectors;
    SerializedProperty filledDetectors;
    SerializedProperty erasedDetectors;
    SerializedProperty stoppedDetectors;

    SerializedProperty victoryManager;

    void OnEnable()
    {
        victoryType = serializedObject.FindProperty("victoryType");

        colorsDetectors = serializedObject.FindProperty("colorsDetectors");
        detachDetectors = serializedObject.FindProperty("detachDetectors");
        cutedDetectors = serializedObject.FindProperty("cutDetectors");
        revealedDetectors = serializedObject.FindProperty("revealedDetectors");
        shakeDetectors = serializedObject.FindProperty("shakeDetectors");
        filledDetectors = serializedObject.FindProperty("filledDetectors");
        erasedDetectors = serializedObject.FindProperty("erasedDetectors");
        stoppedDetectors = serializedObject.FindProperty("stoppedDetectors");

        victoryManager = serializedObject.FindProperty("victoryManager");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft };

        serializedObject.Update();
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Victory Checker Settings", bigTitle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(victoryType);
        EditorGUILayout.Space(15);

        // Affiche seulement la liste correspondant au type sélectionné
        if (!victoryType.hasMultipleDifferentValues)
        {
            VictoryType type = (VictoryType)victoryType.enumValueIndex;

            switch (type)
            {
                case VictoryType.MatchingColors:
                    EditorGUILayout.LabelField("Matching Colors Detectors", middleTitle);
                    EditorGUILayout.PropertyField(colorsDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllDetached:
                    EditorGUILayout.LabelField("Detach Detectors", middleTitle);
                    EditorGUILayout.PropertyField(detachDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllCut:
                    EditorGUILayout.LabelField("Cuted Detectors", middleTitle);
                    EditorGUILayout.PropertyField(cutedDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllRevealed:
                    EditorGUILayout.LabelField("Revealed Detectors", middleTitle);
                    EditorGUILayout.PropertyField(revealedDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllShake:
                    EditorGUILayout.LabelField("Shake Detectors", middleTitle);
                    EditorGUILayout.PropertyField(shakeDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllFilled:
                    EditorGUILayout.LabelField("Filled Detectors", middleTitle);
                    EditorGUILayout.PropertyField(filledDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllErased:
                    EditorGUILayout.LabelField("Erased Detectors", middleTitle);
                    EditorGUILayout.PropertyField(erasedDetectors, true);
                    EditorGUILayout.Space(15);
                    break;

                case VictoryType.AllStopped:
                    EditorGUILayout.LabelField("Stopped Detectors", middleTitle);
                    EditorGUILayout.PropertyField(stoppedDetectors, true);
                    EditorGUILayout.Space(15);
                    break;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Les objets sélectionnés ont des types de victoire différents.", MessageType.Info);
            EditorGUILayout.Space(15);
        }

        EditorGUILayout.LabelField("Victory Manager", bigTitle);
        EditorGUILayout.PropertyField(victoryManager);
        EditorGUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}
