using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteController))]
[CanEditMultipleObjects]
public class SpriteControllerEditor : Editor
{
    SerializedProperty currentMode;

    // Follow Mouse
    SerializedProperty followX;
    SerializedProperty followY;
    SerializedProperty followSpeed;
    SerializedProperty cutterCollider;

    // Detachable
    SerializedProperty maxScale;
    SerializedProperty distanceMultiplier;
    SerializedProperty scaleDuration;
    SerializedProperty isDetached;
    SerializedProperty detachType;
    SerializedProperty fallDistance;
    SerializedProperty fallDuration;
    SerializedProperty keepDetachedScale;

    // Cuttable
    SerializedProperty cuttableCollider;
    SerializedProperty isCut;

    void OnEnable()
    {
        currentMode = serializedObject.FindProperty("currentMode");

        followX = serializedObject.FindProperty("followX");
        followY = serializedObject.FindProperty("followY");
        followSpeed = serializedObject.FindProperty("followSpeed");
        cutterCollider = serializedObject.FindProperty("cutterCollider");

        maxScale = serializedObject.FindProperty("maxScale");
        distanceMultiplier = serializedObject.FindProperty("distanceMultiplier");
        scaleDuration = serializedObject.FindProperty("scaleDuration");
        isDetached = serializedObject.FindProperty("isDetached");
        detachType = serializedObject.FindProperty("detachType");
        fallDistance = serializedObject.FindProperty("fallDistance");
        fallDuration = serializedObject.FindProperty("fallDuration");
        keepDetachedScale = serializedObject.FindProperty("keepDetachedScale");

        cuttableCollider = serializedObject.FindProperty("cuttableCollider");
        isCut = serializedObject.FindProperty("isCut");
    }

    public override void OnInspectorGUI()
    {
        // Styles pour gros titres
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft };

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(currentMode);
        EditorGUILayout.Space(15);

        if (!currentMode.hasMultipleDifferentValues)
        {
            SpriteController.Mode mode = (SpriteController.Mode)currentMode.enumValueIndex;

            // ---------------------- FOLLOW MOUSE ----------------------
            if (mode == SpriteController.Mode.FollowMouse)
            {
                EditorGUILayout.LabelField("Follow Mouse Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(followX);
                EditorGUILayout.PropertyField(followY);
                EditorGUILayout.PropertyField(followSpeed);
                EditorGUILayout.Space(15);
            }

            // ---------------------- DETACHABLE ----------------------
            if (mode == SpriteController.Mode.Detachable)
            {
                EditorGUILayout.LabelField("Detach Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(detachType);
                EditorGUILayout.Space(10);

                if (!detachType.hasMultipleDifferentValues)
                {
                    SpriteController.DetachType type = (SpriteController.DetachType)detachType.enumValueIndex;

                    if (type == SpriteController.DetachType.Fall)
                    {
                        EditorGUILayout.LabelField("Fall Settings", middleTitle);
                        EditorGUILayout.PropertyField(fallDistance);
                        EditorGUILayout.PropertyField(fallDuration);
                        EditorGUILayout.Space(10);
                    }
                    else if (type == SpriteController.DetachType.Draggable)
                    {
                        EditorGUILayout.LabelField("Draggable Settings", middleTitle);
                        EditorGUILayout.HelpBox("Pas de settings spécifiques.", MessageType.Info);
                        EditorGUILayout.Space(10);
                    }
                    else if (type == SpriteController.DetachType.None)
                    {
                        EditorGUILayout.LabelField("None Settings", middleTitle);
                        EditorGUILayout.PropertyField(keepDetachedScale);
                        EditorGUILayout.Space(10);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("DetachType diffère entre les objets sélectionnés.", MessageType.Info);
                    EditorGUILayout.Space(10);
                }

                EditorGUILayout.LabelField("General Detach Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(maxScale);
                EditorGUILayout.PropertyField(distanceMultiplier);
                EditorGUILayout.PropertyField(scaleDuration);
                EditorGUILayout.PropertyField(isDetached);
                EditorGUILayout.Space(15);
            }

            // ---------------------- CUTTABLE ----------------------
            if (mode == SpriteController.Mode.Cuttable)
            {
                EditorGUILayout.LabelField("Cuttable Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(cuttableCollider);
                EditorGUILayout.PropertyField(isCut);
                EditorGUILayout.PropertyField(cutterCollider);
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Fall Settings", middleTitle);
                EditorGUILayout.PropertyField(fallDistance);
                EditorGUILayout.PropertyField(fallDuration);
                EditorGUILayout.Space(15);
            }

            // ---------------------- NONE ----------------------
            if (mode == SpriteController.Mode.None)
            {
                EditorGUILayout.LabelField("None Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(isCut);
                EditorGUILayout.PropertyField(isDetached);
                EditorGUILayout.Space(15);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Les objets sélectionnés ont des modes différents.", MessageType.Info);
            EditorGUILayout.Space(15);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
