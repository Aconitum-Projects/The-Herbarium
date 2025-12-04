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

    public Sprite sprite;
        
    private Button button;
    private SceneChanger sceneChanger;
    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;

    void OnValidate()
    {
        #if UNITY_EDITOR
                if (sceneAsset != null)
                {
                    var path = AssetDatabase.GetAssetPath(sceneAsset);
                    sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                }
        #endif
    }


    void Start()
    {
        button = GetComponent<Button>();
        sceneChanger = FindAnyObjectByType<SceneChanger>();

        if (sceneChanger == null)
        {
            Debug.LogError("SceneChanger non trouvé dans la scène !");
            return;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
            originalScale = button.transform.localScale;

            buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }
        else
        {
            Debug.LogError("Aucun bouton assigné !");
        }
    }

    void OnButtonClick()
    {
        if (buttonImage != null)
        {
            buttonImage.DOColor(Color.gray, 0.2f).OnComplete(() =>
            {
                buttonImage.DOColor(originalColor, 0.2f);
            });
        }

        button.transform.DOScale(originalScale * 1.2f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            button.transform.DOScale(originalScale, 0.1f).SetEase(Ease.InQuad);
        });

        sceneChanger.ChangeSprite(sprite);
        SceneManager.LoadScene(sceneName);
    }
}