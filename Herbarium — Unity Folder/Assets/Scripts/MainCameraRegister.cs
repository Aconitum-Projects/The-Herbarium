using UnityEngine;

public class MainCameraRegister : MonoBehaviour
{
    void Start()
    {
        SceneChanger.Instance?.SetSceneCamera(GetComponent<Camera>());
    }
}