using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleCutscene : MonoBehaviour
{
    public CinemachineCamera playerCam;
    public CinemachineCamera plantCam;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    int lineIndex = 0;

    string[] lines = {
        "Human... you came.",
        "Yeah, what happened?",
        "The Zombies! they keep coming for me. I can’t hold them off much longer.",
        "Zombies??? Why do they want YOU?",
        "They feed on my energy. If they kill me, this whole forest will die.",
        "The Forest? Oh no.. tell me what to do.",
        "Help me survive these attacks. I’ll give you something precious in return.",
        "What?",
        "A golden seed. Plant it anywhere and it will grow into endless riches.",
        "Deal. Let’s save your roots first."
    };

    void Start()
    {
        SetActiveCamera(plantCam); // starts with PLANT
        UpdateDialogueUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            lineIndex++;

            if (lineIndex >= lines.Length)
            {
                SceneManager.LoadScene("StarterLevel");
                return;
            }

            bool isPlayerLine = (lineIndex % 2 != 0); // plant = even player = odd
            SetActiveCamera(isPlayerLine ? playerCam : plantCam);
            UpdateDialogueUI();
        }
    }

    void SetActiveCamera(CinemachineCamera active)
    {
        if (!playerCam || !plantCam) return;
        playerCam.gameObject.SetActive(active == playerCam);
        plantCam.gameObject.SetActive(active == plantCam);
    }

    void UpdateDialogueUI()
    {
        if (dialogueText) dialogueText.text = lines[lineIndex];
        if (nameText)
        {
            // Switch name
            if (lineIndex % 2 == 0)
                nameText.text = "Plant";
            else
                nameText.text = "You";
        }
    }
}
