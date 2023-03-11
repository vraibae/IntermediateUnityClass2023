using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterID
{
    PLAYER,
    CHARACTER1,
    CHARACTER2
}

[CreateAssetMenu(menuName = "Dialogue System/Line Data")]
public class DialogueLineData : ScriptableObject
{
    public string text;

    public DialogueLineData[] responses;
    public int karmaScore;

    public string dialogueAudio; //TODO: Turn this into an enum so you don't have to drag sounds into every individual line... Can just pick from a dropdown.
    //How would we get the enum to update here if I added new sounds in the AudioManager though? Would be tedious to change two scripts each time.

    public float lineWaitTime = 1.0f;

    public CharacterID character;
}