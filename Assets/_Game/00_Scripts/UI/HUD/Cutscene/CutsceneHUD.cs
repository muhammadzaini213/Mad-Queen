using UnityEngine;
using UnityEngine.Events;
using Slafurry.Utils.UI;

public class CutsceneHUD : MonoBehaviour
{
    [SerializeField] private UISlide[] uiSlides;
    [SerializeField] private UnityEvent onCutsceneStart;
    [SerializeField] private UnityEvent onCutsceneEnd;

    public void StartCutscene()
    {
        Show();
        onCutsceneStart?.Invoke();
    }

    public void EndCutscene()
    {
        onCutsceneEnd?.Invoke();
        Hide();
    }

    private void Show()
    {

        foreach (var uiSlide in uiSlides)
        {
            uiSlide.SlideIn();
        }
    }

    private void Hide()
    {
        foreach (var uiSlide in uiSlides)
        {
            uiSlide.SlideOut();
        }

    }
}