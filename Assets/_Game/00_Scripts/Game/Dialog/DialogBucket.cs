using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea(2, 5)] public string text;
}

[CreateAssetMenu(fileName = "DialogBucket", menuName = "Dialog/Dialog Bucket")]
public class DialogBucket : ScriptableObject
{
    [SerializeField] private List<DialogLine> lines = new List<DialogLine>();

    public IReadOnlyList<DialogLine> Lines => lines;
}