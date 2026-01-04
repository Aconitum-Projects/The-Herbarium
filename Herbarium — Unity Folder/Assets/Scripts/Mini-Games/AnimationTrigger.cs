using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Transform target;      // L'objet dont on suit le X
    public Transform animatedObj; // L'objet animé
    public AnimationCurve curve;  // Curve pour interpoler selon le X
    public AnimationClip animToPlay;

    public float minX = 0f;       // X min du target
    public float maxX = 10f;      // X max du target
    public float minTime = 0f;    // temps min de l'anim
    public float maxTime = 1f;    // temps max de l'anim

    private Animator animator;

    void Start()
    {
        animator = animatedObj.GetComponent<Animator>();
    }

    void Update()
    {
        // Normaliser la position X du target entre 0 et 1
        float normalizedX = Mathf.InverseLerp(minX, maxX, target.localPosition.x);
        
        // Optionnel : appliquer une curve pour lissage
        float curveValue = curve.Evaluate(normalizedX);

        // Convertir en temps de l'animation
        float animTime = Mathf.Lerp(minTime, maxTime, curveValue);

        // Jouer l'animation à ce temps précis
        animator.Play(animToPlay.name, 0, animTime);
        animator.speed = 0; // freeze pour que l'anim suive exactement
    }
}