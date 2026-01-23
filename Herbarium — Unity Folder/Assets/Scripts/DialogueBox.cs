using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public CanvasGroup dialogueCanvas;
    public TMP_Text dialogueText;
    public Button yesButton, noButton;
    public float animationDuration = 0.5f;
    
    private string nextSceneName;
    private SceneChanger sceneChanger;
    private List<GameTrigger> gameTrigger;
    
    [HideInInspector]
    public AnimationClip transitionAnim;

    void Awake()
    {
        gameTrigger = new List<GameTrigger>(FindObjectsByType<GameTrigger>(FindObjectsSortMode.None));
        sceneChanger = FindAnyObjectByType<SceneChanger>();
        dialogueCanvas.gameObject.SetActive(false);

        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);
    }


    public void ShowDialogue(string question, string sceneToPlay)
    {
        nextSceneName = sceneToPlay;
        dialogueText.text = question;
        dialogueCanvas.gameObject.SetActive(true);
        dialogueCanvas.alpha = 0;
        dialogueCanvas.transform.localScale = Vector3.zero;

        dialogueCanvas.blocksRaycasts = false;

        dialogueCanvas.DOFade(1, animationDuration);
        dialogueCanvas.transform.DOScale(1, animationDuration).SetEase(Ease.OutBack)
            .OnComplete(() => dialogueCanvas.blocksRaycasts = true);

        AnimateButton(yesButton);
        AnimateButton(noButton);
    }


    public void OnYesClicked()
    {
        HideDialogue();
        sceneChanger.PlayTransition(transitionAnim.name);
        sceneChanger.ChangeScene(nextSceneName);
    }

    public void OnNoClicked()
    {
        PlayerActivationAndDetection();
        HideDialogue();
    }

    public void PlayerActivationAndDetection()
    {
        if (gameTrigger != null)
        {
            foreach (var gameTrigger in gameTrigger)
            { 
                gameTrigger.PlayerPlayable();
            }
        }
    }

    private void HideDialogue()
    {
        dialogueCanvas.DOFade(0, animationDuration);
        dialogueCanvas.transform.DOScale(0, animationDuration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                dialogueCanvas.gameObject.SetActive(false);
            });
    }


    private void AnimateButton(Button button)
    {
        button.transform.localScale = Vector3.one;
        button.onClick.AddListener(() => button.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f));

        button.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0.8f), 0.2f).From();
    }
}
