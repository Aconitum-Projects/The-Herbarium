using UnityEngine;
using Unity.Cinemachine;

public class GameTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public CinemachineCamera newCamera;
    public CinemachineCamera currentCamera;
    public string question = "Wanna play 's mini-game ?";
    public string sceneToPlay = "MiniGame_";
    public Sprite transitionSprite;

    private GameController gameController;
    private CharacterMovement currentCharacterMovement;
    private DialogueBox dialogueBox;

    public bool isInTrigger = false;

    void Start()
    {
        dialogueBox = FindAnyObjectByType<DialogueBox>();
        gameController = FindAnyObjectByType<GameController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        if (currentCharacterMovement == null)
        {
            currentCharacterMovement = other.GetComponent<CharacterMovement>();
        }
        
        if (gameController != null) gameController.enabled = false;
        if (currentCharacterMovement != null) currentCharacterMovement.enabled = false;

        SwitchCamera();
        isInTrigger = true;
        
        
        dialogueBox.transitionSprite = transitionSprite;
        dialogueBox?.ShowDialogue(question, sceneToPlay);
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPlayable();
        }
    }

    public void PlayerPlayable()
    {
        if (gameController != null) gameController.enabled = true;
        if (currentCharacterMovement != null) currentCharacterMovement.enabled = true;
            
        SwitchBackToInitialCamera();
    }
    
    private void SwitchCamera()
    {
        if (currentCamera != null) currentCamera.gameObject.SetActive(false);
        if (newCamera != null) newCamera.gameObject.SetActive(true);
    }

    public void SwitchBackToInitialCamera()
    {
        if (!isInTrigger) return;

        isInTrigger = false;

        if (newCamera != null) newCamera.gameObject.SetActive(false);
        if (currentCamera != null) currentCamera.gameObject.SetActive(true);
    }
}
