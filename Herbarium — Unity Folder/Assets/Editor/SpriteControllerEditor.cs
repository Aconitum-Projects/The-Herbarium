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

    // Shakeable
    SerializedProperty isShakeable;
    SerializedProperty isShaken;
    SerializedProperty shakeThreshold;
    SerializedProperty shakeMultiplier;
    SerializedProperty shakeResetTime;

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

        isShakeable = serializedObject.FindProperty("isShakeable");
        isShaken = serializedObject.FindProperty("isShaken");
        shakeThreshold = serializedObject.FindProperty("shakeThreshold");
        shakeMultiplier = serializedObject.FindProperty("shakeMultiplier");
        shakeResetTime = serializedObject.FindProperty("shakeResetTime");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft };

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(currentMode, new GUIContent("Mode", "Mode de fonctionnement du sprite"));
        EditorGUILayout.Space(15);

        if (!currentMode.hasMultipleDifferentValues)
        {
            SpriteController.Mode mode = (SpriteController.Mode)currentMode.enumValueIndex;

            // ---------------------- DRAGGABLE ----------------------
            if (mode == SpriteController.Mode.Draggable)
            {
                EditorGUILayout.LabelField("Draggable Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(isShakeable, new GUIContent("Shakeable", "Le sprite peut être secoué pour déclencher un événement"));

                if (isShakeable.boolValue)
                {
                    EditorGUILayout.PropertyField(shakeThreshold, new GUIContent("Shake Threshold", "Vitesse minimale du mouvement pour déclencher le shake"));
                    EditorGUILayout.PropertyField(shakeMultiplier, new GUIContent("Shake Multiplier", "Réduit ou augmente la sensibilité du shake"));
                    EditorGUILayout.PropertyField(shakeResetTime, new GUIContent("Shake Reset Time", "Durée avant que le shake soit réinitialisé"));

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(isShaken, new GUIContent("Is Shaken", "Indique si le sprite est actuellement secoué"));
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.Space(15);
            }

            // ---------------------- FOLLOW MOUSE ----------------------
            if (mode == SpriteController.Mode.FollowMouse)
            {
                EditorGUILayout.LabelField("Follow Mouse Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(followX, new GUIContent("Follow X", "Le sprite suit la souris sur l'axe X"));
                EditorGUILayout.PropertyField(followY, new GUIContent("Follow Y", "Le sprite suit la souris sur l'axe Y"));
                EditorGUILayout.PropertyField(followSpeed, new GUIContent("Follow Speed", "Vitesse à laquelle le sprite suit la souris"));
                EditorGUILayout.Space(15);
            }

            // ---------------------- DETACHABLE ----------------------
            if (mode == SpriteController.Mode.Detachable)
            {
                EditorGUILayout.LabelField("Detach Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(detachType, new GUIContent("Detach Type", "Type de détachement du sprite"));
                EditorGUILayout.Space(10);

                if (!detachType.hasMultipleDifferentValues)
                {
                    SpriteController.DetachType type = (SpriteController.DetachType)detachType.enumValueIndex;

                    if (type == SpriteController.DetachType.Fall)
                    {
                        EditorGUILayout.LabelField("Fall Settings", middleTitle);
                        EditorGUILayout.PropertyField(fallDistance, new GUIContent("Fall Distance", "Distance de chute du sprite"));
                        EditorGUILayout.PropertyField(fallDuration, new GUIContent("Fall Duration", "Durée de la chute"));
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
                        EditorGUILayout.PropertyField(keepDetachedScale, new GUIContent("Keep Detached Scale", "Garder l'échelle du sprite après détachement"));
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
                EditorGUILayout.PropertyField(maxScale, new GUIContent("Max Scale", "Échelle maximale du sprite lors du détachement"));
                EditorGUILayout.PropertyField(distanceMultiplier, new GUIContent("Distance Multiplier", "Multiplicateur de distance pour le détachement"));
                EditorGUILayout.PropertyField(scaleDuration, new GUIContent("Scale Duration", "Durée de l'animation d'échelle"));

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(isDetached, new GUIContent("Is Detached", "Indique si le sprite est actuellement détaché"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(15);
            }

            // ---------------------- CUTTABLE ----------------------
            if (mode == SpriteController.Mode.Cuttable)
            {
                EditorGUILayout.LabelField("Cuttable Settings", bigTitle);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(cuttableCollider, new GUIContent("Cuttable Collider", "Collider que le sprite peut être coupé"));

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(isCut, new GUIContent("Is Cut", "Indique si le sprite a été coupé"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(cutterCollider, new GUIContent("Cutter Collider", "Collider qui coupe le sprite"));
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Fall Settings", middleTitle);
                EditorGUILayout.PropertyField(fallDistance, new GUIContent("Fall Distance", "Distance de chute après coupe"));
                EditorGUILayout.PropertyField(fallDuration, new GUIContent("Fall Duration", "Durée de la chute après coupe"));
                EditorGUILayout.Space(15);
            }

            // ---------------------- NONE ----------------------
            if (mode == SpriteController.Mode.None)
            {
                EditorGUILayout.LabelField("None Settings", bigTitle);
                EditorGUILayout.Space(5);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(isCut, new GUIContent("Is Cut", "Indique si le sprite a été coupé"));
                EditorGUILayout.PropertyField(isDetached, new GUIContent("Is Detached", "Indique si le sprite est détaché"));
                EditorGUI.EndDisabledGroup();

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