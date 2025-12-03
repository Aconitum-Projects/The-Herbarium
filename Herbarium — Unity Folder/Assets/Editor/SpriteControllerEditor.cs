using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteController))]
[CanEditMultipleObjects]
public class SpriteControllerEditor : Editor
{
    SerializedProperty currentMode;

    SerializedProperty followX;
    SerializedProperty followY;
    SerializedProperty followSpeed;

    SerializedProperty maxScale;
    SerializedProperty distanceMultiplier;
    SerializedProperty scaleDuration;

    SerializedProperty detachType;
    SerializedProperty fallDistance;
    SerializedProperty fallDuration;

    void OnEnable()
    {
        currentMode = serializedObject.FindProperty("currentMode");

        followX = serializedObject.FindProperty("followX");
        followY = serializedObject.FindProperty("followY");
        followSpeed = serializedObject.FindProperty("followSpeed");

        maxScale = serializedObject.FindProperty("maxScale");
        distanceMultiplier = serializedObject.FindProperty("distanceMultiplier");
        scaleDuration = serializedObject.FindProperty("scaleDuration");

        detachType = serializedObject.FindProperty("detachType");
        fallDistance = serializedObject.FindProperty("fallDistance");
        fallDuration = serializedObject.FindProperty("fallDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ------------------------------------------------ MODE ------------------------------------------------
        EditorGUILayout.PropertyField(currentMode);
        EditorGUILayout.Space();

        // Si plusieurs valeurs → pas de logique conditionnelle
        if (!currentMode.hasMultipleDifferentValues)
        {
            SpriteController.Mode mode =
                (SpriteController.Mode)currentMode.enumValueIndex;

            // ------------------------------------------------ FOLLOW MOUSE ------------------------------------------------
            if (mode == SpriteController.Mode.FollowMouse)
            {
                EditorGUILayout.LabelField("Follow Mouse Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(followX);
                EditorGUILayout.PropertyField(followY);
                EditorGUILayout.PropertyField(followSpeed);
            }

            // ------------------------------------------------ DETACHABLE ------------------------------------------------
            if (mode == SpriteController.Mode.Detachable)
            {
                EditorGUILayout.LabelField("Detach Settings", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(detachType);
                EditorGUILayout.Space();

                // Pas de logique si multi-values
                if (!detachType.hasMultipleDifferentValues)
                {
                    SpriteController.DetachType type =
                        (SpriteController.DetachType)detachType.enumValueIndex;

                    if (type == SpriteController.DetachType.Fall)
                    {
                        EditorGUILayout.LabelField("Fall Settings", EditorStyles.miniBoldLabel);
                        EditorGUILayout.PropertyField(fallDistance);
                        EditorGUILayout.PropertyField(fallDuration);
                        EditorGUILayout.Space();
                    }

                    if (type == SpriteController.DetachType.Draggable)
                    {
                        EditorGUILayout.LabelField("Draggable Settings", EditorStyles.miniBoldLabel);
                        EditorGUILayout.HelpBox("Pas de settings spécifiques.", MessageType.Info);
                        EditorGUILayout.Space();
                    }

                    if (type == SpriteController.DetachType.None)
                    {
                        EditorGUILayout.LabelField("None Settings", EditorStyles.miniBoldLabel);
                        EditorGUILayout.HelpBox("Aucun comportement particulier.", MessageType.Info);
                        EditorGUILayout.Space();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("DetachType diffère entre les objets sélectionnés.", MessageType.Info);
                    EditorGUILayout.Space();
                }

                // ---------------- COMMON ----------------
                EditorGUILayout.LabelField("General Detach Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(maxScale);
                EditorGUILayout.PropertyField(distanceMultiplier);
                EditorGUILayout.PropertyField(scaleDuration);
            }
        }
        else
        {
            // mode = mixed
            EditorGUILayout.HelpBox("Les objets sélectionnés ont des modes différents.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
