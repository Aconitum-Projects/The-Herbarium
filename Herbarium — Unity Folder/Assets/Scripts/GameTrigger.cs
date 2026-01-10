using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public CinemachineCamera newCamera;
    public CinemachineCamera currentCamera;
    public string question = "Wanna play 's mini-game ?";
    public string sceneToPlay = "MiniGame_";
    public AnimationClip transitionAnim;

    private GameController gameController;
    private CharacterMovement currentCharacterMovement;
    private DialogueBox dialogueBox;

    public bool isInTrigger = false;

    private Collider triggerCollider;

    void Awake()
    {

        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
            Debug.LogWarning(name + " has no Collider!");

        dialogueBox = FindAnyObjectByType<DialogueBox>();
        if (dialogueBox == null) Debug.LogWarning("DialogueBox not found");

        gameController = FindAnyObjectByType<GameController>();
        if (gameController == null) Debug.LogWarning("GameController not found");
    }

    void OnEnable()
    {
        if (SceneStateManager.Instance != null)
        {
            SceneStateManager.Instance.OnTransformsRestored += CheckPlayerInsideAndMoveOut;
        }
    }

    void OnDisable()
    {
        if (SceneStateManager.Instance != null)
        {
            SceneStateManager.Instance.OnTransformsRestored -= CheckPlayerInsideAndMoveOut;
        }
    }

    private void CheckPlayerInsideAndMoveOut()
    {
        GameObject player = GameObject.FindGameObjectWithTag(triggerTag);
        if (player == null)
        {
            Debug.Log(name + ": no player found");
            return;
        }

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.Log(player.name + ": has no Collider");
            return;
        }

        // Utiliser Physics.ComputePenetration pour savoir si le joueur est dans le trigger
        Collider triggerCol = triggerCollider;
        Vector3 direction;
        float distance;
        bool isInside = Physics.ComputePenetration(
            playerCollider, player.transform.position, player.transform.rotation,
            triggerCol, triggerCol.transform.position, triggerCol.transform.rotation,
            out direction, out distance
        );

        if (isInside)
        {
            Vector3 safePos = player.transform.position + direction * (distance + 0.5f);

            CharacterMovement cm = player.GetComponent<CharacterMovement>();
            if (cm != null)
                cm.MoveOutOfTrigger(safePos);
            else
                player.transform.position = safePos;
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;
        if (isInTrigger) return;

        currentCharacterMovement = other.GetComponent<CharacterMovement>();

        if (gameController != null) gameController.enabled = false;
        if (currentCharacterMovement != null) currentCharacterMovement.enabled = false;

        SwitchCamera();
        isInTrigger = true;

        dialogueBox.transitionAnim = transitionAnim;
        dialogueBox.ShowDialogue(question, sceneToPlay);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        isInTrigger = false;
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.Escape))
            PlayerPlayable();
    }

    public void PlayerPlayable()
    {
        if (gameController != null) gameController.enabled = true;
        if (currentCharacterMovement != null) currentCharacterMovement.enabled = true;

        SwitchBackToInitialCamera();

        isInTrigger = false;
    }

    private void SwitchCamera()
    {
        if (currentCamera != null) currentCamera.gameObject.SetActive(false);
        if (newCamera != null) newCamera.gameObject.SetActive(true);
    }

    public void SwitchBackToInitialCamera()
    {
        if (!isInTrigger) return;

        if (newCamera != null) newCamera.gameObject.SetActive(false);
        if (currentCamera != null) currentCamera.gameObject.SetActive(true);
    }
}
