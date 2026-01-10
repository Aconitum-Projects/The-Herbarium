#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneStateManager : MonoBehaviour
{
    #if UNITY_EDITOR
    public SceneAsset sceneAsset;
    #endif

    [SerializeField] private string sceneName;

    public static SceneStateManager Instance;

    private Dictionary<string, TransformData> savedTransforms = new Dictionary<string, TransformData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
    #endif

    // ------------------ Sauvegarde / Chargement ------------------
    public void SaveTransform(string objectName, Transform transform)
    {
        savedTransforms[objectName] = new TransformData(transform);
    }

    public bool LoadTransform(string objectName, Transform transform)
    {
        if (savedTransforms.TryGetValue(objectName, out TransformData data))
        {
            transform.position = data.position;
            transform.rotation = data.rotation;
            transform.localScale = data.scale;
            return true;
        }
        return false;
    }

    // ------------------ Callback scène ------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestoreTransforms();
    }

    public delegate void TransformsRestored();
    public event TransformsRestored OnTransformsRestored;

    private void RestoreTransforms()
    {
        foreach (var kvp in savedTransforms)
        {
            GameObject obj = GameObject.Find(kvp.Key);
            if (obj != null)
            {
                LoadTransform(kvp.Key, obj.transform);
            }
        }

        OnTransformsRestored?.Invoke();
    }

}

[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
    }
}
