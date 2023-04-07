using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStarter : MonoBehaviour
{
    /* ASSIGNMENT
     * Create a new item type that is a dialogue starter
     * Player interacts with this new item, and it starts a dialogue.
     * Extra credit for interactable sound when interacting.
     * 
     * PSEUDOCODE / GENERAL PLAN OF ATTACK
     * 1) Make a new game object called DialogueStarter that places a UI button on the screen. 
     * 2) Grab the button component in the Start() function.
     * 3) Write a function that basically just... calls the DialogueManager's StartDialogue() function? Passing in "DialogueOne".
     * 4) Do all that event listening magic that ties the button to the function... Or just do the OnClick() thing because sometimes we gotta stick to what we know best
     * 5) EXTRA CREDIT: Make it play a sound in the TriggerStartDialogue function.
     */

    #region Public Variables
    public DialogueLineData lineToStart;
    public GameObject dialogueManager;
    //public AudioClip soundOnStart;
    #endregion

    #region Private Variables
    private Button startButton;
    private DialogueManager dialogueManagerReference;
    #endregion

    void Start()
    {
        startButton = this.GetComponent<Button>();
        dialogueManagerReference = dialogueManager.GetComponent<DialogueManager>();
    }

    //This function is called when the Start button is pressed. 
    public void TriggerStartDialogue()
    {
        this.gameObject.SetActive(false);

        //TODO: Fix how the sound stops short for some reason... Probably because the sound from the dialogueLineData overrides it?
        ////Play a sound when you interact
        //if (soundOnStart != null)
        //{
        //    PlayAudio audioMessage = new PlayAudio();
        //    audioMessage.clipToPlay = soundOnStart;
        //    EvtSystem.EventDispatcher.Raise<PlayAudio>(audioMessage);
        //}

        dialogueManagerReference.StartDialogue(lineToStart);
    }
}
