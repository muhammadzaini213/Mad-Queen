using TMPro;
using UnityEngine;

public class InteractionHUD : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI label;

    private string currentClue;

    private void Awake()
    {
        Hide();
    }

    public void Show(string clue)
    {
        if (!container.activeSelf)
            container.SetActive(true);

        if (currentClue == clue) return;

        currentClue = clue;
        label.text = $"(E) {clue}";
    }

    public void Hide()
    {
        currentClue = null;

        if (container.activeSelf)
            container.SetActive(false);
    }
}