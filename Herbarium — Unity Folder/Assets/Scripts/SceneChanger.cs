using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public RectTransform panel;
    public Image targetImage;
    public Vector2 showPanel;
    public Vector2 hidePanel = new Vector2(3950, 0);
    public float slideDuration = 0.5f;

    private static SceneChanger instance;
    private Vector2 initialPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (panel != null)
            initialPosition = panel.anchoredPosition;
    }

    public void ChangeSprite(Sprite newSprite)
    {
        if (targetImage != null && newSprite != null)
        {
            targetImage.sprite = newSprite;
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (panel == null)
        {
            Debug.LogError("Panel non assigné !");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Le nom de la scène est vide ou null !");
            return;
        }

        panel.anchoredPosition = initialPosition;
        panel.DOAnchorPos(showPanel, slideDuration)
            .OnComplete(() => StartCoroutine(LoadSceneAsync(sceneName)));
    }


    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncLoad.isDone);

        panel.DOAnchorPos(hidePanel, slideDuration);
    }
}