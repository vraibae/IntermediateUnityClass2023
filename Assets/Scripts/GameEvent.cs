using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EvtSystem;

public class ShowDialogueText : EvtSystem.Event
{
    public string text;
    public CharacterID id;
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

}