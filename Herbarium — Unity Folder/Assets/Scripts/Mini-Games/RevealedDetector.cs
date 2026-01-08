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


    void Update()
    {
        if (MoveASprite)
        {
            // ancienne logique
            if (transform.position.y < yThreshold)
            {
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

            if (transform.position.x < xThreshold)
            {
                validated = true;
                var vc = transform.parent.GetComponentInParent<VictoryChecker>();
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
                var vc = transform.parent.GetComponentInParent<VictoryChecker>();
                if (vc != null)
                    vc.CheckAllRevealed();
            }
        }
    }
}
