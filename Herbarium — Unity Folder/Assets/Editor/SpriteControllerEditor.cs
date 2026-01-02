using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteController))]
[CanEditMultipleObjects]
public class SpriteControllerEditor : Editor
{
    SerializedProperty currentMode;

    // Follow Mouse
    SerializedProperty followX, followY, followSpeed, cutterCollider, limitX, xLimits, limitY, yLimits;

    // Detachable
    SerializedProperty maxScale, detachThreshold, scaleDuration, isDetached, detachType, fallDistance, fallDuration, keepDetachedScale, detachVisualMode;

    // Cuttable
    SerializedProperty cuttableCollider, isCut;

    // Shakeable
    SerializedProperty isShakeable, isShaken, shakeThreshold, shakeMultiplier, shakeResetTime;

    // Fillable
    SerializedProperty fillX, fillY, fillOffsetX, fillOffsetY, fillDuration, fillEase;

    // Erasable
    SerializedProperty eraserCollider, eraseSpeed, minAlpha;

    // Stoppable
    SerializedProperty stoppableTargetOffset, stoppableDuration, stoppableEase;

    void OnEnable()
    {
        currentMode = serializedObject.FindProperty("currentMode");

        // Follow Mouse
        followX = serializedObject.FindProperty("followX");
        followY = serializedObject.FindProperty("followY");
        followSpeed = serializedObject.FindProperty("followSpeed");
        cutterCollider = serializedObject.FindProperty("cutterCollider");
        limitX = serializedObject.FindProperty("limitX");
        xLimits = serializedObject.FindProperty("xLimits");
        limitY = serializedObject.FindProperty("limitY");
        yLimits = serializedObject.FindProperty("yLimits");

        // Detachable
        maxScale = serializedObject.FindProperty("maxScale");
        scaleDuration = serializedObject.FindProperty("scaleDuration");
        detachThreshold = serializedObject.FindProperty("detachThreshold");
        isDetached = serializedObject.FindProperty("isDetached");
        detachType = serializedObject.FindProperty("detachType");
        fallDistance = serializedObject.FindProperty("fallDistance");
        fallDuration = serializedObject.FindProperty("fallDuration");
        keepDetachedScale = serializedObject.FindProperty("keepDetachedScale");
        detachVisualMode = serializedObject.FindProperty("detachVisualMode");

        // Cuttable
        cuttableCollider = serializedObject.FindProperty("cuttableCollider");
        isCut = serializedObject.FindProperty("isCut");

        // Shakeable
        isShakeable = serializedObject.FindProperty("isShakeable");
        isShaken = serializedObject.FindProperty("isShaken");
        shakeThreshold = serializedObject.FindProperty("shakeThreshold");
        shakeMultiplier = serializedObject.FindProperty("shakeMultiplier");
        shakeResetTime = serializedObject.FindProperty("shakeResetTime");

        // Fillable
        fillX = serializedObject.FindProperty("fillX");
        fillY = serializedObject.FindProperty("fillY");
        fillOffsetX = serializedObject.FindProperty("fillOffsetX");
        fillOffsetY = serializedObject.FindProperty("fillOffsetY");
        fillDuration = serializedObject.FindProperty("fillDuration");
        fillEase = serializedObject.FindProperty("fillEase");

        // Erasable
        eraserCollider = serializedObject.FindProperty("eraserCollider");
        eraseSpeed = serializedObject.FindProperty("eraseSpeed");
        minAlpha = serializedObject.FindProperty("minAlpha");

        // Stoppable
        stoppableTargetOffset = serializedObject.FindProperty("stoppableTargetOffset");
        stoppableDuration = serializedObject.FindProperty("stoppableDuration");
        stoppableEase = serializedObject.FindProperty("stoppableEase");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft };

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(currentMode, new GUIContent("Mode", "Mode de fonctionnement du sprite"));
        EditorGUILayout.Space(15);

        DrawShakeable(bigTitle);

        if (!currentMode.hasMultipleDifferentValues)
        {
            SpriteController.Mode mode = (SpriteController.Mode)currentMode.enumValueIndex;

            switch (mode)
            {
                case SpriteController.Mode.Draggable:
                    DrawDraggable(bigTitle);
                    break;
                case SpriteController.Mode.FollowMouse:
                    DrawFollowMouse(bigTitle);
                    break;
                case SpriteController.Mode.Detachable:
                    DrawDetachable(bigTitle, middleTitle);
                    break;
                case SpriteController.Mode.Cuttable:
                    DrawCuttable(bigTitle, middleTitle);
                    break;
                case SpriteController.Mode.Fillable:
                    DrawFillable(bigTitle);
                    break;
                case SpriteController.Mode.Erasable:
                    DrawErasable(bigTitle);
                    break;
                case SpriteController.Mode.Stoppable:
                    DrawStoppable(bigTitle);
                    break;
                case SpriteController.Mode.None:
                    DrawNone(bigTitle);
                    break;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Les objets sélectionnés ont des modes différents.", MessageType.Info);
            EditorGUILayout.Space(15);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawShakeable(GUIStyle bigTitle)
    {
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

    void DrawDraggable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Draggable Settings", bigTitle);
        EditorGUILayout.Space(5);
    }

    void DrawFollowMouse(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Follow Mouse Settings", bigTitle);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(followX, new GUIContent("Follow X", "Le sprite suit la souris sur l'axe X"));
        if (followX.boolValue)
        {
            EditorGUILayout.PropertyField(limitX, new GUIContent("Limit X", "Limiter le déplacement sur l'axe X"));
            if (limitX.boolValue)
                EditorGUILayout.PropertyField(xLimits, new GUIContent("X Limits", "Bornes min / max sur X"));
        }
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(followY, new GUIContent("Follow Y", "Le sprite suit la souris sur l'axe Y"));
        if (followY.boolValue)
        {
            EditorGUILayout.PropertyField(limitY, new GUIContent("Limit Y", "Limiter le déplacement sur l'axe Y"));
            if (limitY.boolValue)
                EditorGUILayout.PropertyField(yLimits, new GUIContent("Y Limits", "Bornes min / max sur Y"));
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(followSpeed, new GUIContent("Follow Speed", "Vitesse à laquelle le sprite suit la souris"));
        EditorGUILayout.Space(15);
    }

    void DrawDetachable(GUIStyle bigTitle, GUIStyle middleTitle)
    {
        EditorGUILayout.LabelField("Detach Settings", bigTitle);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(detachVisualMode, new GUIContent(
            "Visual Mode",
            "Scalable = change le scale\nAnimated = change le sprite selon la distance"
        ));

        EditorGUILayout.Space(5);

        if (!detachVisualMode.hasMultipleDifferentValues)
        {
            SpriteController.DetachVisualMode visualMode =
                (SpriteController.DetachVisualMode)detachVisualMode.intValue;

            bool useScale = visualMode.HasFlag(SpriteController.DetachVisualMode.Scalable);
            bool useAnim  = visualMode.HasFlag(SpriteController.DetachVisualMode.Animated);

            if (useScale)
            {
                EditorGUILayout.LabelField("Scalable Detach", middleTitle);
                EditorGUILayout.PropertyField(maxScale, new GUIContent("Max Scale"));
                EditorGUILayout.PropertyField(scaleDuration, new GUIContent("Scale Duration"));
                EditorGUILayout.Space(5);
            }

            if (useAnim)
            {
                EditorGUILayout.LabelField("Animated Detach", middleTitle);
                SerializedProperty animatedSpritesProp =
                    serializedObject.FindProperty("animatedDetachSprites");
                EditorGUILayout.PropertyField(animatedSpritesProp, new GUIContent("Animated Sprites"), true);
                EditorGUILayout.Space(5);
            }
            
            if (visualMode == SpriteController.DetachVisualMode.None)
            {
                EditorGUILayout.HelpBox(
                    "Aucun mode visuel sélectionné. Le detach n’aura pas de feedback.",
                    MessageType.Warning
                );
            }
            
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(detachThreshold, new GUIContent("Detach Threshold"));

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(isDetached, new GUIContent("Is Detached"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(detachType, new GUIContent("Detach Type"));
        EditorGUILayout.PropertyField(keepDetachedScale, new GUIContent("Keep Detached Scale"));
        EditorGUILayout.Space(10);


        if (!detachType.hasMultipleDifferentValues)
        {
            SpriteController.DetachType type = (SpriteController.DetachType)detachType.enumValueIndex;
            switch (type)
            {
                case SpriteController.DetachType.Fall:
                    EditorGUILayout.LabelField("Fall Settings", middleTitle);
                    EditorGUILayout.PropertyField(fallDistance, new GUIContent("Fall Distance", "Distance de chute du sprite"));
                    EditorGUILayout.PropertyField(fallDuration, new GUIContent("Fall Duration", "Durée de la chute"));
                    EditorGUILayout.Space(10);
                    break;
                case SpriteController.DetachType.Draggable:
                    EditorGUILayout.LabelField("Draggable Settings", middleTitle);
                    EditorGUILayout.HelpBox("Pas de settings spécifiques.", MessageType.Info);
                    EditorGUILayout.Space(10);
                    break;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("DetachType diffère entre les objets sélectionnés.", MessageType.Info);
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.Space(15);
    }

    void DrawCuttable(GUIStyle bigTitle, GUIStyle middleTitle)
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

    void DrawFillable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Fillable Settings", bigTitle);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Maintenir clic gauche pour attirer lentement le sprite vers la position relative sur les axes activés.", MessageType.Info);

        EditorGUILayout.PropertyField(fillX, new GUIContent("Fill X", "Appliquer le remplissage sur l'axe X"));
        if (fillX.boolValue)
            EditorGUILayout.PropertyField(fillOffsetX, new GUIContent("Offset X", "Déplacement relatif sur X"));

        EditorGUILayout.PropertyField(fillY, new GUIContent("Fill Y", "Appliquer le remplissage sur l'axe Y"));
        if (fillY.boolValue)
            EditorGUILayout.PropertyField(fillOffsetY, new GUIContent("Offset Y", "Déplacement relatif sur Y"));

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(fillDuration, new GUIContent("Fill Duration", "Temps du mouvement vers la cible"));
        EditorGUILayout.PropertyField(fillEase, new GUIContent("Fill Ease", "Interpolation du mouvement"));
        EditorGUILayout.Space(15);
    }

    void DrawErasable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Erasable Settings", bigTitle);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Frotter avec l'eraser pour réduire progressivement l'opacité du sprite.", MessageType.Info);

        EditorGUILayout.PropertyField(eraserCollider, new GUIContent("Eraser Collider", "Collider utilisé pour effacer le sprite"));
        EditorGUILayout.PropertyField(eraseSpeed, new GUIContent("Erase Speed", "Vitesse linéaire de diminution de l'opacité"));
        EditorGUILayout.PropertyField(minAlpha, new GUIContent("Min Alpha", "Opacité minimale avant destruction ou validation"));
        EditorGUILayout.Space(15);
    }

    void DrawStoppable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Stoppable Settings", bigTitle);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Le sprite oscille en continu entre sa position d'origine et la position cible.\nCliquer n'importe où arrête le mouvement à la position actuelle.", MessageType.Info);

        EditorGUILayout.PropertyField(stoppableTargetOffset, new GUIContent("Target Offset", "Déplacement relatif par rapport à la position d'origine"));
        EditorGUILayout.PropertyField(stoppableDuration, new GUIContent("Duration", "Durée du déplacement vers la cible"));
        EditorGUILayout.PropertyField(stoppableEase, new GUIContent("Ease", "Type d'interpolation du mouvement"));
        EditorGUILayout.Space(15);
    }

    void DrawNone(GUIStyle bigTitle)
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