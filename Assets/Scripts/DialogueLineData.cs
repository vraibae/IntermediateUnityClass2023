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
    //This serves as the binding path for our custom editor so that we can directly pull this info out! 
    //If you want to use a property within a diff class instead, like an int randomNum from the class Test, of which there is an instance called myTest, your binding path would be "myTest.randomNum".
    public string text; 

    public DialogueLineData[] responses;
    public int karmaScore;

    public string dialogueAudio; //TODO: Turn this into an enum so you don't have to drag sounds into every individual line... Can just pick from a dropdown.
    //How would we get the enum to update here if I added new sounds in the AudioManager though? Would be tedious to change two scripts each time.

    public float lineWaitTime = 1.0f;

    public CharacterID character;
}