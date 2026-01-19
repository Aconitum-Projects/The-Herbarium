using UnityEngine;

public class RevealedDetector : MonoBehaviour
{
    [Header("===== Reveal Settings =====")]
    public bool validated = false;
    public bool MoveASprite = true;

    [Space(20)]
    [Header("--- Trigger Settings ---")]
    public float yThreshold = -15f;
    public float xThreshold = -17.5f;
    public float thresholdTolerance = 0.2f;

    [Header("--- Teleport Target ---")]
    public Vector3 newPosition;

    [Header("--- Objects to Affect ---")]
    public GameObject objectToActivate;
    public GameObject objectToDesactivate;
    public Transform spriteToMove;
    public Vector3 spriteNewPosition;
    public Vector3 spriteNewRotation;

    [Space(20)]
    [Header("===== Trail Settings =====")]
    public MaskSpriteTrail2D trail;
    
    bool yTriggered = false;
    bool xTriggered = false;

    void Update()
    {
        if (MoveASprite)
        {
            // ===== Y trigger =====
            if (!yTriggered &&
                Mathf.Abs(transform.position.y - yThreshold) <= thresholdTolerance)
            {
                yTriggered = true;

                transform.position = newPosition;

                if (objectToActivate != null)
                    objectToActivate.SetActive(true);

                if (objectToDesactivate != null)
                    objectToDesactivate.SetActive(false);

                if (spriteToMove != null)
                {
                    spriteToMove.position = spriteNewPosition;
                    spriteToMove.rotation = Quaternion.Euler(spriteNewRotation);

                    var sc = spriteToMove.GetComponent<SpriteController>();
                    if (sc != null)
                    {
                        sc.followX = true;
                        sc.followY = false;
                    }
                }
            }

            // ===== X trigger =====
            if (!xTriggered &&
                Mathf.Abs(transform.position.x - xThreshold) <= thresholdTolerance)
            {
                xTriggered = true;
                validated = true;
                var vc = GetComponentInParent<VictoryChecker>();

                if (vc != null)
                    vc.CheckAllRevealed();
            }
        }
        else
        {
            // nouvelle logique : check trail mask
            if (trail != null && trail.FullyRevealed)
            {
                validated = true;
                var vc = GetComponentInParent<VictoryChecker>();
                if (vc != null)
                    vc.CheckAllRevealed();
            }
        }
    }
}
