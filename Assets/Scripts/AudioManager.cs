using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    /* PSEUDOCODE (bc. i am a design major)
     * Make this script inherit from the Singleton class
     * Open Brackey's tutorial on audiomanagers to make this script play sounds. 
     * Change the DialogueLineData's input from an audioClip to an enum so I can just pick a reaction sound from a dropdown. 
     * Fix the GameEvent format so it takes a Sound and not an AudioClip.
     * Add the audioManager into the scene under an empty game object. 
     */
    public Sound[] sounds;

    void Awake()
    {
        //Subscribe to DialogueManager's PlayAudio event, and call the PlaySound function once the event is called.
        EvtSystem.EventDispatcher.AddListener<PlayAudio>(PlaySound);

        //Makes an audiosource for each sound, ready to be used with their specified volume and pitch. 
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            //s.source.playOnAwake = false;
        }

        //If each line of dialogue has a Sound attached to it... Why not just take that Sound and make that play on an audiosource attached here? 
        //Couldnt play sounds simultaneously... 
    }

    private void PlaySound(PlayAudio eventData)
    {
        //Look for the sound in the array that matches the sound given by the event.
        Sound s = Array.Find(sounds, sound => sound.name == eventData.clipToPlay);
        //TODO: Add a warning if a sound by that name doesn't exist.

        s.source.Play();
        eventData.lineWaitTime = s.source.clip.length;
        Debug.Log("audioManager: " + eventData.lineWaitTime);
    }
}
