using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    private Dictionary<string, TransformData> savedTransforms = new Dictionary<string, TransformData>();

    [SerializeField] private string sceneToSave = "MainScene"; // Change par le nom de ta scène

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
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveTransform(string objectName, Transform transform)
    {
        savedTransforms[objectName] = new TransformData(transform);
    }

    public bool LoadTransform(string objectName, Transform transform)
    {
        if (savedTransforms.ContainsKey(objectName))
        {
            transform.position = savedTransforms[objectName].position;
            transform.rotation = savedTransforms[objectName].rotation;
            return true;
        }
        return false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToSave)
        {
            RestoreTransforms();
        }
    }

    private void RestoreTransforms()
    {
        foreach (var obj in FindObjectsOfType<CharacterMovement>())
        {
            LoadTransform(obj.gameObject.name, obj.transform);
        }
    }
}

[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}