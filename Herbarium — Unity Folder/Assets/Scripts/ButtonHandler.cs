using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Scene")]
    public string sceneName;

    [Header("Transition")]
    public AnimationClip transitionAnim;

    [Header("Animator")]
    public Animator animator;
    public string hoverTriggerName = "OnHover";
    public string exitTriggerName = "OnExit";
    public string clickTriggerName = "OnClick";

    [Header("SceneChanger")]
    public SceneChanger sceneChanger;

    private void Awake()
    {
        if (!sceneChanger)
            sceneChanger = FindAnyObjectByType<SceneChanger>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator)
            animator.SetTrigger(hoverTriggerName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator)
            animator.SetTrigger(exitTriggerName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("ButtonHandler: sceneName is EMPTY");
            return;
        }

        if (animator)
            animator.SetTrigger(clickTriggerName);

        if (transitionAnim && sceneChanger)
        {
            sceneChanger.PlayTransition(transitionAnim.name);
            sceneChanger.ChangeScene(sceneName);
        }
    }
}