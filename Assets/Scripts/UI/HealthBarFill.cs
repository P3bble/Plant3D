using UnityEngine;
using UnityEngine.UI;

public class HealthBarFill : MonoBehaviour
{
    [SerializeField] Image fillImage;

    public void SetFill(int current, int max)
    {
        if (!fillImage || max <= 0) return;
        fillImage.fillAmount = Mathf.Clamp01((float)current / max);
    }
}
