using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Kinetix/EmoteContexts")]
public class ContextualEmoteSO : ScriptableObject
{
    [SerializeField]
    public ContextualEmote[] contexts;
}

[Serializable]
public class ContextualEmote
{
    public string ContextName;
    public string ContextDescription;
    [HideInInspector]
    public string EmoteID;

    public ContextualEmote Clone()
    {
        return new ContextualEmote() {
            ContextName = this.ContextName,
            ContextDescription = this.ContextDescription,
            EmoteID = this.EmoteID,
        };
    }
}
