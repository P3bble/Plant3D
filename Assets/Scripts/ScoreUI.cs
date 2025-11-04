using TMPro;
// using TMPUGI;
using UnityEngine;
// using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI label;

    void OnEnable() // to save memory
    {
        if (!label) label = GetComponent<TextMeshProUGUI>();

        // show current value right away
        var sm = ScoreManager.Instance;
        UpdateLabel(sm ? sm.Score : 0);

        // listen for changes
        if (sm) sm.OnScoreChanged += UpdateLabel;
    }

    void OnDisable()
    {
        var sm = ScoreManager.Instance;
        if (sm) sm.OnScoreChanged -= UpdateLabel;
    }

    void UpdateLabel(int value)
    {
        if (label) label.text = $"Score: {value:N0}";
    }
}
