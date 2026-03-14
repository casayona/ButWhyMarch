using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("Texts")]
    [TextArea(3, 5)] public string turkishText;  // Türkçe metin
    [TextArea(3, 5)] public string englishText;  // Ýngilizce metin
    // Ýleride Almanca istersen buraya: public string germanText; eklersin.

    [Header("Settings")]
    public float fadeInDuration = 1f;
    public float displayDuration = 3f;
    public float fadeOutDuration = 1f;
}

[CreateAssetMenu(fileName = "NewStorySequence", menuName = "Story/Story Sequence")]
public class StorySequence : ScriptableObject
{
    public DialogueLine[] lines;
}