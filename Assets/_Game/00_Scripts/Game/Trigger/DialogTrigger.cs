using UnityEngine;
using UnityEngine.Events;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogHUD dialogHUD;
    [SerializeField] private DialogBucket bucket;
    [SerializeField] private UnityEvent onFinished;

    public void Trigger()
    {
        if (dialogHUD == null) return;

        dialogHUD.Play(bucket, onFinished.Invoke);
    }
}