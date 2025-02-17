using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private bool isControlled = false;
    private GameObject selectionEffect;
    
    public GameObject selectionEffectPrefab;
    public float rotationSpeed = 10f; // Vitesse de rotation
    
    private void OnDestroy()
    {
        SceneStateManager.Instance?.SaveTransform(gameObject.name, transform);
    }
    
    void Update()
    {
        if (isControlled)
        {
            HandleKeyboardMovement();
            HandleMouseClickMovement();
        }
    }

    public void SetControl(bool isControlled)
    {
        this.isControlled = isControlled;
    }

    public void SetSelectionEffect(bool isActive)
    {
        if (isActive)
        {
            if (selectionEffect == null)
            {
                selectionEffect = Instantiate(selectionEffectPrefab, transform.position, Quaternion.identity, transform);
                selectionEffect.transform.localScale = Vector3.zero;
                selectionEffect.transform.DOScale(Vector3.one, 0.5f);
            }
        }
        else
        {
            if (selectionEffect != null)
            {
                selectionEffect.transform.DOScale(Vector3.zero, 0.5f).OnKill(() => Destroy(selectionEffect));
                selectionEffect = null;
            }
        }
    }

    private void HandleKeyboardMovement()
    {
        if (!isMoving)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                // Rotation vers la direction du déplacement
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                // Déplacement du personnage
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    private void HandleMouseClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction.magnitude > 0.1f)
                {
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                }

                StartCoroutine(MoveToTarget());
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            if (direction.magnitude > 0.1f)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
