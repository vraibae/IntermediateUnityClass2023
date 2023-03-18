using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EvtSystem;

public class ShowDialogueText : EvtSystem.Event
{
    public string text;
    public CharacterID id;
    public float duration;
}

public class PlayAudio : EvtSystem.Event
{
    public string clipToPlay;
    public float lineWaitTime = 0.0f;
}

public struct ResponseData
{
    public string text;
    public int karmaScore;

    public UnityAction buttonAction;
}
public class ShowResponses : EvtSystem.Event
{
    public ResponseData[] responses;
}

public class DisableUI : EvtSystem.Event
{
    //QUESTION: Why is this blank? Would it not be the same to just, not have this class at all?
}