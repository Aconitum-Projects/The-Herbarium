using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteController))]

[CanEditMultipleObjects]
public class SpriteControllerEditor : Editor
{
    SerializedProperty currentMode;
    SerializedProperty ignoreVictoryFreeze;

    // Follow Mouse
    SerializedProperty followX, followY, followSpeed, cutterCollider, limitX,
        xLimits, limitY, yLimits, useFollowPoints, followPoints, passesPerPoint,
        followValidated;
    
    // Follow Rotation Settings
    private SerializedProperty rotateInsteadOfFollow, rotationSpeedFollow,
        rotationMinFollow, rotationMaxFollow;
    
    // Detachable
    SerializedProperty maxScale, detachThreshold, scaleDuration, isDetached,
        detachType, keepDetachedScale, detachVisualMode;

    // Cuttable
    SerializedProperty cuttableCollider, isCut;

    // Fall
    SerializedProperty fallDistance, fallDuration, fallDirection;
    
    // Shakeable
    SerializedProperty isShakeable, shakeThreshold, shakeVisualMode,
        animatedShakeSprites, resetPositionWhenReleased;

    // Fillable
    SerializedProperty fillPosition, fillRotation, fillScale, fillOffsetPos,
        fillOffsetRot, fillOffsetScale, fillDuration, fillEase;
    
    // Erasable
    SerializedProperty eraserCollider, eraseSpeed, minAlpha;

    // Stoppable
    SerializedProperty stoppableTargetOffset, stoppableDuration, stoppableEase,
        stoppableInputDelay;

    // Moving
    SerializedProperty moveDirection, speedRange, collectCollider, destroyCollider,
        collectedSpriteRenderer;

    void OnEnable()
    {
        currentMode = serializedObject.FindProperty("currentMode");
        ignoreVictoryFreeze = serializedObject.FindProperty("ignoreVictoryFreeze");

        // Follow Mouse
        followX = serializedObject.FindProperty("followX");
        followY = serializedObject.FindProperty("followY");
        followSpeed = serializedObject.FindProperty("followSpeed");
        cutterCollider = serializedObject.FindProperty("cutterCollider");
        limitX = serializedObject.FindProperty("limitX");
        xLimits = serializedObject.FindProperty("xLimits");
        limitY = serializedObject.FindProperty("limitY");
        yLimits = serializedObject.FindProperty("yLimits");
        useFollowPoints   = serializedObject.FindProperty("useFollowPoints");
        followPoints      = serializedObject.FindProperty("followPoints");
        passesPerPoint    = serializedObject.FindProperty("passesPerPoint");
        followValidated   = serializedObject.FindProperty("followValidated");
        
        // Rotate instead of follow
        rotateInsteadOfFollow = serializedObject.FindProperty("rotateInsteadOfFollow");
        rotationSpeedFollow  = serializedObject.FindProperty("rotationSpeedFollow");
        rotationMinFollow  = serializedObject.FindProperty("rotationMinFollow");
        rotationMaxFollow  = serializedObject.FindProperty("rotationMaxFollow");

        // Detachable
        maxScale = serializedObject.FindProperty("maxScale");
        scaleDuration = serializedObject.FindProperty("scaleDuration");
        detachThreshold = serializedObject.FindProperty("detachThreshold");
        isDetached = serializedObject.FindProperty("isDetached");
        detachType = serializedObject.FindProperty("detachType");
        keepDetachedScale = serializedObject.FindProperty("keepDetachedScale");
        detachVisualMode = serializedObject.FindProperty("detachVisualMode");

        // Cuttable
        cuttableCollider = serializedObject.FindProperty("cuttableCollider");
        isCut = serializedObject.FindProperty("isCut");
        
        // Fall
        fallDistance = serializedObject.FindProperty("fallDistance");
        fallDuration = serializedObject.FindProperty("fallDuration");
        fallDirection = serializedObject.FindProperty("fallDirection");
        
        // Shakeable
        isShakeable = serializedObject.FindProperty("isShakeable");
        shakeThreshold = serializedObject.FindProperty("shakeThreshold");
        shakeVisualMode = serializedObject.FindProperty("shakeVisualMode");
        animatedShakeSprites = serializedObject.FindProperty("animatedShakeSprites");

        // Fillable
        fillPosition = serializedObject.FindProperty("fillPosition");
        fillRotation = serializedObject.FindProperty("fillRotation");
        fillScale    = serializedObject.FindProperty("fillScale");
        fillOffsetPos   = serializedObject.FindProperty("fillOffsetPos");
        fillOffsetRot   = serializedObject.FindProperty("fillOffsetRot");
        fillOffsetScale = serializedObject.FindProperty("fillOffsetScale");
        fillDuration    = serializedObject.FindProperty("fillDuration");
        fillEase        = serializedObject.FindProperty("fillEase");
        resetPositionWhenReleased = serializedObject.FindProperty("resetPositionWhenReleased");

        // Erasable
        eraserCollider = serializedObject.FindProperty("eraserCollider");
        eraseSpeed = serializedObject.FindProperty("eraseSpeed");
        minAlpha = serializedObject.FindProperty("minAlpha");

        // Stoppable
        stoppableTargetOffset = serializedObject.FindProperty("stoppableTargetOffset");
        stoppableDuration = serializedObject.FindProperty("stoppableDuration");
        stoppableEase = serializedObject.FindProperty("stoppableEase");
        stoppableInputDelay = serializedObject.FindProperty("stoppableInputDelay");
        
        // Moving
        moveDirection   = serializedObject.FindProperty("moveDirection");
        speedRange      = serializedObject.FindProperty("speedRange");
        collectCollider = serializedObject.FindProperty("collectCollider");
        destroyCollider = serializedObject.FindProperty("destroyCollider");
        collectedSpriteRenderer = serializedObject.FindProperty("collectedSpriteRenderer");

    }

    public override void OnInspectorGUI()
    {
        GUIStyle bigTitle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
        GUIStyle middleTitle = new GUIStyle(EditorStyles.miniBoldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft };

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(currentMode, new GUIContent("Mode", "Mode de fonctionnement du sprite"));
        EditorGUILayout.PropertyField(ignoreVictoryFreeze, new GUIContent("Ignore Freeze in Victory"));
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
                case SpriteController.Mode.Moving:
                    DrawMoving(bigTitle);
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
        if (GUI.changed)
            Repaint();

    }

    void DrawShakeable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Shakeable Settings", bigTitle);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(isShakeable, new GUIContent("Shakeable", "Le sprite peut être secoué pour déclencher un événement"));

        if (!isShakeable.hasMultipleDifferentValues && isShakeable.boolValue)
        {
            EditorGUILayout.PropertyField(shakeThreshold, new GUIContent("Shake Threshold", "Vitesse minimale pour déclencher le shake"));

            // Gestion du flag
            SpriteController.ShakeVisualMode visualMode = (SpriteController.ShakeVisualMode)shakeVisualMode.intValue;
            visualMode = (SpriteController.ShakeVisualMode)EditorGUILayout.EnumFlagsField(
                new GUIContent("Shake Visual Mode", "Animated = animation via sprites"),
                visualMode
            );
            shakeVisualMode.intValue = (int)visualMode;

            bool useAnim = visualMode.HasFlag(SpriteController.ShakeVisualMode.Animated);

            if (useAnim)
            {
                EditorGUILayout.LabelField("Animated Shake", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(animatedShakeSprites, new GUIContent("Animated Shake Sprites"), true);

                EditorGUILayout.Space(5);
            }

            if (visualMode == SpriteController.ShakeVisualMode.None)
            {
                EditorGUILayout.HelpBox("Aucun mode visuel sélectionné. Le shake n’aura pas de feedback.", MessageType.Warning);
            }
        }

        EditorGUILayout.Space(15);
    }

    void DrawDraggable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Draggable Settings", bigTitle);
        EditorGUILayout.Space(5);

        SerializedProperty rotateProp = serializedObject.FindProperty("rotateInsteadOfMove");
        EditorGUILayout.PropertyField(rotateProp, new GUIContent("Rotate Instead of Move", "Si vrai, bouger la souris fait tourner le sprite au lieu de le déplacer"));

        if (rotateProp.boolValue)
        {
            SerializedProperty speedProp = serializedObject.FindProperty("rotationSpeed");
            EditorGUILayout.PropertyField(speedProp, new GUIContent("Rotation Speed", "Sensibilité de rotation selon le mouvement de la souris"));

            SerializedProperty limitProp = serializedObject.FindProperty("limitRotation");
            EditorGUILayout.PropertyField(limitProp, new GUIContent("Limit Rotation", "Activer la limitation de rotation"));

            if (limitProp.boolValue)
            {
                SerializedProperty minProp = serializedObject.FindProperty("minRotation");
                SerializedProperty maxProp = serializedObject.FindProperty("maxRotation");

                EditorGUILayout.PropertyField(minProp, new GUIContent("Min Rotation", "Rotation minimale (°)"));
                EditorGUILayout.PropertyField(maxProp, new GUIContent("Max Rotation", "Rotation maximale (°)"));
            }
        }

        EditorGUILayout.Space(10);
    }

    void DrawFollowMouse(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Follow Mouse Settings", bigTitle);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(rotateInsteadOfFollow, new GUIContent("Rotate Instead of Follow", "Faire tourner le sprite selon la souris au lieu de suivre la position"));
        if (rotateInsteadOfFollow.boolValue)
        {
            EditorGUILayout.PropertyField(rotationSpeedFollow, new GUIContent("Rotation Speed", "Vitesse de rotation selon mouvement de la souris"));

            SerializedProperty limitProp = serializedObject.FindProperty("limitRotation");
            EditorGUILayout.PropertyField(limitProp, new GUIContent("Limit Rotation", "Limiter la rotation"));

            if (limitProp.boolValue)
            {
                EditorGUILayout.PropertyField(rotationMinFollow, new GUIContent("Min Rotation Follow", "Rotation minimale pour le mode Follow"));
                EditorGUILayout.PropertyField(rotationMaxFollow, new GUIContent("Max Rotation Follow", "Rotation maximale pour le mode Follow"));
            }
        }
        else
        {
            EditorGUILayout.Space(20);
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
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Follow Points Mode", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(useFollowPoints, new GUIContent("Use Follow Points", "Active la validation via plusieurs points à toucher"));
        if (useFollowPoints.boolValue)
        {
            EditorGUILayout.PropertyField(followPoints, new GUIContent("Follow Points", "Liste de colliders à toucher dans l'ordre ou plusieurs fois"), true);
            EditorGUILayout.PropertyField(passesPerPoint, new GUIContent("Passes Per Point", "Nombre de passages requis pour valider chaque point"));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(followValidated, new GUIContent("Follow Validated", "Indique si tous les points ont été validés"));
            EditorGUI.EndDisabledGroup();
        }

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
                    DrawFall(middleTitle);
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

        DrawFall(middleTitle);
    }

    void DrawFall(GUIStyle middleTitle)
    {
        EditorGUILayout.LabelField("Fall Settings", middleTitle);
        EditorGUILayout.PropertyField(fallDistance, new GUIContent("Fall Distance", "Distance de chute après coupe"));
        EditorGUILayout.PropertyField(fallDuration, new GUIContent("Fall Duration", "Durée de la chute après coupe"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(fallDirection, new GUIContent("Fall Direction", "Direction normalisée de la chute"));
        EditorGUILayout.Space(15);
    }

    void DrawFillable(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Fillable Settings", bigTitle);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Maintenir clic gauche pour attirer lentement le sprite vers la cible selon les transformations activées.", MessageType.Info);

        EditorGUILayout.PropertyField(fillPosition, new GUIContent("Fill Position", "Appliquer le remplissage sur la position"));
        if (fillPosition.boolValue)
            EditorGUILayout.PropertyField(fillOffsetPos, new GUIContent("Offset Position", "Décalage local cible"));

        EditorGUILayout.PropertyField(fillRotation, new GUIContent("Fill Rotation", "Appliquer le remplissage sur la rotation Z"));
        if (fillRotation.boolValue)
            EditorGUILayout.PropertyField(fillOffsetRot, new GUIContent("Offset Rotation", "Rotation relative (°)"));

        EditorGUILayout.PropertyField(fillScale, new GUIContent("Fill Scale", "Appliquer le remplissage sur le scale"));
        if (fillScale.boolValue)
            EditorGUILayout.PropertyField(fillOffsetScale, new GUIContent("Offset Scale", "Scale relatif cible"));

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(fillDuration, new GUIContent("Fill Duration", "Temps du mouvement vers la cible"));
        EditorGUILayout.PropertyField(fillEase, new GUIContent("Fill Ease", "Interpolation du mouvement"));

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(resetPositionWhenReleased, new GUIContent("Reset When Released", "Revenir à la position/rotation/scale initiale quand le clic est relâché"));

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
        EditorGUILayout.PropertyField(stoppableInputDelay, new GUIContent("Input Delay", "Durée d'attente avant click autorisé"));
        EditorGUILayout.Space(15);
    }
    
    void DrawMoving(GUIStyle bigTitle)
    {
        EditorGUILayout.LabelField("Moving Settings", bigTitle);
        EditorGUILayout.Space(5);

        EditorGUILayout.HelpBox(
            "Le sprite se déplace en continu dans une direction donnée avec une vitesse aléatoire.",
            MessageType.Info
        );
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Collected Visual", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            collectedSpriteRenderer,
            new GUIContent(
                "Collected Sprite Renderer",
                "SpriteRenderer activé lorsque l'objet est collecté"
            )
        );

        EditorGUILayout.PropertyField(
            moveDirection,
            new GUIContent("Move Direction", "Direction du déplacement (sera normalisée)")
        );

        EditorGUILayout.PropertyField(
            speedRange,
            new GUIContent("Speed Range", "Vitesse min / max du déplacement")
        );

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Collision", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            collectCollider,
            new GUIContent("Collect Collider", "Collider qui déclenche l'état collected")
        );

        EditorGUILayout.PropertyField(
            destroyCollider,
            new GUIContent("Destroy Collider", "Collider qui détruit le sprite")
        );
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