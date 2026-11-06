
using UnityEngine;
using UnityEngine.Events;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogHUD dialogHUD;
    [SerializeField] private DialogBucket bucket;
    [SerializeField] private UnityEvent onFinished;

    [Header("Settings")]
    [SerializeField] private bool occupyPlayer = true;

    public void Trigger()
    {
        if (dialogHUD == null || bucket == null)
            return;

        dialogHUD.Play(
            bucket,
            occupyPlayer,
            finished: () => onFinished?.Invoke()
        );
    }
}
