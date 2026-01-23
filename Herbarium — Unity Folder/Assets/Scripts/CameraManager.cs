using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Targets")]
    public Transform[] targets;

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;
    public float maxDistance = 20f;
    public float minDistance = 10f;

    private CinemachineCamera virtualCamera;
    private Vector3 currentVelocity;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (targets == null || targets.Length == 0 || GameController.instance == null)
            return;

        Vector3 averagePosition = GetAveragePositionOfTargets();
        float distanceToTargets = GetDistanceBetweenTargets();
        float targetDistance = Mathf.Clamp(distanceToTargets, minDistance, maxDistance);

        Vector3 desiredPosition =
            averagePosition - virtualCamera.transform.forward * targetDistance;

        virtualCamera.transform.position = Vector3.SmoothDamp(
            virtualCamera.transform.position,
            desiredPosition,
            ref currentVelocity,
            smoothSpeed
        );
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
        float maxDist = 0f;

        for (int i = 0; i < targets.Length; i++)
        {
            for (int j = i + 1; j < targets.Length; j++)
            {
                float dist = Vector3.Distance(targets[i].position, targets[j].position);
                if (dist > maxDist)
                    maxDist = dist;
            }
        }

        return maxDist;
    }
}