using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Targets")]
    public Transform[] targets; 

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;
    public float maxDistance = 20f;
    public float minDistance = 10f;

    [Header("Depth of Field")]
    public Volume globalVolume;
    public float focusSmoothSpeed = 5f;
    public float extraFocusMargin = 2f;

    private CinemachineCamera virtualCamera;
    private Vector3 currentVelocity;

    private DepthOfField dof;
    private float currentFocusDistance;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();

        if (globalVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            dof = depthOfField;
            currentFocusDistance = dof.focusDistance.value;
        }
    }

    void Update()
    {
        if (targets.Length == 0 || GameController.instance == null) return;

        Transform controlledTarget = GameController.instance.characters[GameController.instance.currentCharacterIndex].transform;

        Vector3 averagePosition = GetAveragePositionOfTargets();
        float distanceToTargets = GetDistanceBetweenTargets();
        float targetDistance = Mathf.Clamp(distanceToTargets, minDistance, maxDistance);

        Vector3 desiredPosition = averagePosition - virtualCamera.transform.forward * targetDistance;
        virtualCamera.transform.position = Vector3.SmoothDamp(virtualCamera.transform.position, desiredPosition, ref currentVelocity, smoothSpeed);

        UpdateDepthOfField(controlledTarget);
    }

    private Vector3 GetAveragePositionOfTargets()
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in targets)
            sum += target.position;

        return sum / targets.Length;
    }

    private float GetDistanceBetweenTargets()
    {
        float maxDistance = 0f;
        for (int i = 0; i < targets.Length; i++)
        {
            for (int j = i + 1; j < targets.Length; j++)
            {
                float distance = Vector3.Distance(targets[i].position, targets[j].position);
                if (distance > maxDistance)
                    maxDistance = distance;
            }
        }
        return maxDistance;
    }

    private void UpdateDepthOfField(Transform controlledTarget)
    {
        if (dof == null || targets.Length == 0) return;

        dof.focusDistance.overrideState = true;

        float mainFocus = Vector3.Distance(virtualCamera.transform.position, controlledTarget.position);

        float minDist = mainFocus;
        foreach (Transform t in targets)
        {
            if (t == controlledTarget) continue;
            float dist = Vector3.Distance(virtualCamera.transform.position, t.position);
            if (dist > minDist) minDist = dist + extraFocusMargin;
        }

        currentFocusDistance = Mathf.Lerp(currentFocusDistance, minDist, Time.deltaTime * focusSmoothSpeed);

        dof.focusDistance.value = currentFocusDistance;
    }

}