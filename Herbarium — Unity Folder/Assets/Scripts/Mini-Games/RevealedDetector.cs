using UnityEngine;

public class RevealedDetector : MonoBehaviour
{
    public bool validated = false;
    [Space(20)]
    
    [Header("Trigger Settings")]
    public float yThreshold = -15f;
    public float xThreshold = -17.5f;

    [Header("Teleport Target")]
    public Vector3 newPosition;

    [Header("Objects to Affect")]
    public GameObject objectToActivate;
    public GameObject objectToDesactivate;
    public Transform spriteToMove;
    public Vector3 spriteNewPosition;
    public Vector3 spriteNewRotation;
    
    void Update()
    {
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
                spriteToMove.GetComponent<SpriteController>().followX = true;
                spriteToMove.GetComponent<SpriteController>().followY = false;
            }
        }

        if (transform.position.x < xThreshold)
        {
            validated = true;
            var vc = transform.parent.GetComponent<VictoryChecker>();
            if (vc != null)
                vc.CheckAllRevealed();
        }
    }
}