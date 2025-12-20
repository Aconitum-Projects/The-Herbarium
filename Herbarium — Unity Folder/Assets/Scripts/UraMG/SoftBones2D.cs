using System.Collections.Generic;
using UnityEngine;

public class SoftBones2D : MonoBehaviour
{
    [Header("Wind")]
    public float windStrength = 15f;
    public float windSpeed = 1f;
    public float randomness = 0.5f;

    [Header("Damping")]
    public float smooth = 5f;

    class BoneData
    {
        public Transform transform;
        public Quaternion baseRotation;
        public int depth;
        public float noiseOffset;
    }

    List<BoneData> bones = new();

    void Awake()
    {
        bones.Clear();

        foreach (Transform child in transform)
        {
            CollectChain(child, 0);
        }
    }

    void CollectChain(Transform current, int depth)
    {
        bones.Add(new BoneData
        {
            transform = current,
            baseRotation = current.localRotation,
            depth = depth,
            noiseOffset = Random.Range(0f, 1000f)
        });

        foreach (Transform child in current)
        {
            CollectChain(child, depth + 1);
        }
    }

    void Update()
    {
        float time = Time.time * windSpeed;

        foreach (var bone in bones)
        {
            float depthFactor = Mathf.Clamp01(bone.depth / 5f);
            float noise = Mathf.PerlinNoise(time + bone.noiseOffset, 0f);
            float angle = (noise - 0.5f) * 2f;

            angle *= windStrength * depthFactor * randomness;

            Quaternion targetRot = bone.baseRotation * Quaternion.Euler(0, 0, angle);

            bone.transform.localRotation = Quaternion.Slerp(
                bone.transform.localRotation,
                targetRot,
                Time.deltaTime * smooth
            );
        }
    }
}