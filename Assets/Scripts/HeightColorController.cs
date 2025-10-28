using UnityEngine;

[ExecuteAlways]
public class HeightColorController : MonoBehaviour
{
    [Header("Assign the material that uses Custom/HeightGradient")]
    public Material targetMaterial;

    [Header("Auto-detect from Terrain/Renderer bounds")]
    public bool autoDetectHeights = true;

    [Tooltip("Override if autoDetectHeights is off")]
    public float minWorldHeight = 0f;
    public float maxWorldHeight = 100f;

    [Header("Band positions (0-1 along min..max)")]
    [Range(0f, 1f)] public float lowStart = 0.25f;
    [Range(0f, 1f)] public float midStart = 0.5f;
    [Range(0f, 1f)] public float topStart = 0.75f;
    [Range(0f, 0.5f)] public float blend = 0.1f;

    void Update()
    {
        if (targetMaterial == null) return;

        float minY = minWorldHeight;
        float maxY = maxWorldHeight;

        if (autoDetectHeights)
        {
            // Try Terrain first
            var terrain = GetComponent<Terrain>();
            if (terrain != null)
            {
                var pos = terrain.transform.position;
                minY = pos.y;
                maxY = pos.y + terrain.terrainData.size.y;
            }
            else
            {
                // Fallback: Renderer bounds on this object
                var rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    var b = rend.bounds;
                    minY = b.min.y;
                    maxY = b.max.y;
                }
            }
        }

        // Safety: avoid division by zero
        if (Mathf.Approximately(minY, maxY)) maxY = minY + 0.001f;

        targetMaterial.SetFloat("_MinHeight", minY);
        targetMaterial.SetFloat("_MaxHeight", maxY);
        targetMaterial.SetFloat("_LowStart", lowStart);
        targetMaterial.SetFloat("_MidStart", midStart);
        targetMaterial.SetFloat("_TopStart", topStart);
        targetMaterial.SetFloat("_Blend", blend);
    }
}
