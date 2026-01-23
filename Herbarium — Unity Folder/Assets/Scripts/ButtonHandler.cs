using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonHandler : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    #region Scene
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    public AnimationClip transitionAnim;
    private string sceneName;
    #endregion

    [Header("Animator")]
    public Animator animator; // L'Animator du panel / bouton

    [Header("SceneChanger")]
    public SceneChanger sceneChanger;

    void Start()
    {
#if UNITY_EDITOR
        if (sceneAsset)
            sceneName = sceneAsset.name;
#endif
        if (!sceneChanger)
            sceneChanger = FindAnyObjectByType<SceneChanger>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator)
            animator.SetTrigger("OnHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator)
            animator.SetTrigger("OnExit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animator)
            animator.SetTrigger("OnClick");

        if (transitionAnim && sceneChanger)
        {
            sceneChanger.PlayTransition(transitionAnim.name);
            sceneChanger.ChangeScene(sceneName);
        }
    }
}