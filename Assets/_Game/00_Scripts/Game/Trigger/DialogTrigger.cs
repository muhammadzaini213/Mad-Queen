using UnityEngine;
using UnityEngine.Events;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogBucket bucket;
    [SerializeField] private UnityEvent onFinished;

    public void Trigger()
    {
        if (DialogHUD.Instance == null) return;

        DialogHUD.Instance.Play(bucket, onFinished.Invoke);
    }
}