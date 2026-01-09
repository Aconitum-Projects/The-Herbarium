using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public RectTransform panel;
    public Animator transitionAnimator;

    public Vector2 showPanel;
    public Vector2 hidePanel = new Vector2(3950, 0);

    [Header("Durations")]
    public float showDuration = 0.5f;
    public float stayDuration = 1f;
    public float hideDuration = 0.5f;

    public static SceneChanger Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    public void SetSceneCamera(Camera cam)
    {
        if (cam == null) return;
        var canvas = panel.GetComponentInParent<Canvas>(true);
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
        }
    }
    
    public void PlayTransition(string animationName)
    {
        if (transitionAnimator == null) return;
        transitionAnimator.Play(animationName, 0, 0f);
    }

    public void ChangeScene(string sceneName)
    {
        panel.DOAnchorPos(showPanel, showDuration)
            .OnComplete(() => StartCoroutine(LoadSceneAsync(sceneName)));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(stayDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncLoad.isDone);

        panel.DOAnchorPos(hidePanel, hideDuration);
    }

}