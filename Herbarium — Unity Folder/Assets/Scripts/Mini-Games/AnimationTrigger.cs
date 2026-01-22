using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Transform target;
    public Animator animator;
    public string stateName;

    public AnimationCurve curve;

    public float minY = -15f;
    public float maxY = 0f;

    void Start()
    {
        animator.Play(stateName, 0, 0f);
        animator.speed = 0f;
    }

    void Update()
    {
        float t = Mathf.InverseLerp(minY, maxY, target.position.y);
        t = Mathf.Clamp01(t);

        if (curve != null)
            t = curve.Evaluate(t);

        animator.Play(stateName, 0, t);
    }
}