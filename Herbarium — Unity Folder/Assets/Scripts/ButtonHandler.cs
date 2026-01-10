#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonHandler : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;
    [SerializeField] private AnimationClip transitionAnim;
    
    private string transitionAnimName;
    private Button button;
    private SceneChanger sceneChanger;
    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        sceneChanger = FindAnyObjectByType<SceneChanger>();

        button.onClick.AddListener(OnButtonClick);

        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
        
        transitionAnimName = transitionAnim.name;
    }

    void OnButtonClick()
    {
        buttonImage.DOColor(Color.gray, 0.2f)
            .OnComplete(() => buttonImage.DOColor(originalColor, 0.2f));

        transform.DOScale(originalScale * 1.2f, 0.1f)
            .OnComplete(() => transform.DOScale(originalScale, 0.1f));

        sceneChanger.PlayTransition(transitionAnimName);
        sceneChanger.ChangeScene(sceneName);
    }
}