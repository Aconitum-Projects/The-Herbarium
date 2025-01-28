using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public Transform[] targets; 
    public float smoothSpeed = 0.125f;
    public float maxDistance = 20f;
    public float minDistance = 10f;

    private CinemachineVirtualCamera virtualCamera;
    private Vector3 currentVelocity;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (targets.Length == 0) return;

        Vector3 averagePosition = GetAveragePositionOfTargets();

        float distanceToTargets = GetDistanceBetweenTargets();
        float targetDistance = Mathf.Clamp(distanceToTargets, minDistance, maxDistance);

        Vector3 desiredPosition = averagePosition - virtualCamera.transform.forward * targetDistance;
        virtualCamera.transform.position = Vector3.SmoothDamp(virtualCamera.transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
    }

    private Vector3 GetAveragePositionOfTargets()
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in targets)
        {
            sum += target.position;
        }
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
                {
                    maxDistance = distance;
                }
            }
        }
        return maxDistance;
    }
}
