using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public GameObject promptRoot;   // parent panel/object to toggle on/off
    public TextMeshProUGUI promptLabel;

    void Awake()
    {
        Hide();
    }

    public void Show(string text)
    {
        if (promptLabel != null) promptLabel.text = text;
        if (promptRoot != null) promptRoot.SetActive(true);
    }

    public void Hide()
    {
        if (promptRoot != null) promptRoot.SetActive(false);
    }
}