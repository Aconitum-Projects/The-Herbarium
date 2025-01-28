using UnityEngine;
using Cinemachine;

public class GameTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public CinemachineVirtualCamera newCamera;
    public CinemachineVirtualCamera currentCamera;

    private CharacterMovement currentCharacterMovement;
    private bool isInTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            currentCharacterMovement = other.GetComponent<CharacterMovement>();
            if (currentCharacterMovement != null)
            {
                currentCharacterMovement.enabled = false;
            }

            SwitchCamera();
            isInTrigger = true;
        }
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentCharacterMovement != null)
            {
                currentCharacterMovement.enabled = true;
            }

            SwitchBackToInitialCamera();

            isInTrigger = false;
        }
    }

    private void SwitchCamera()
    {
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }

        if (newCamera != null)
        {
            newCamera.gameObject.SetActive(true);
        }
    }

    private void SwitchBackToInitialCamera()
    {
        if (newCamera != null)
        {
            newCamera.gameObject.SetActive(false);
        }

        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(true);
        }
    }
}
