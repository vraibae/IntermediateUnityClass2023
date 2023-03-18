using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//QUESTION: Why did we make another class within UISystem to do this? What are common applications for this sort of structure?
class StringReveal
{
    string textToReveal = null;

    float currentTime;
    float secondsPerChar;
    int currentStringIndex = 0;

    public void StartReveal(string text, float duration)
    {
        //So since there are set wait times, we are dividing the given wait time by the number of letters so that we can get the time per char.
        secondsPerChar = duration / text.Length;
        textToReveal = text;

        currentStringIndex = 0;
        currentTime = 0.0f;
    }

    public bool IsDone()
    {
        //Checks if textToReveal is null or if we've already reached the end of the word. 
        return (textToReveal == null || currentStringIndex == (textToReveal.Length - 1));
    }

    public void ForceFinish()
    {
        currentStringIndex = (textToReveal.Length - 1);
        currentTime = 0.0f;
    }


    public string GetCurrentRevealedText()
    {
        currentTime += Time.deltaTime;

        //Increments the index for as long as there are letters left and resets the timer for each character.
        if (currentTime >= secondsPerChar && currentStringIndex < (textToReveal.Length - 1))
        {
            currentStringIndex++;
            currentTime = 0.0f;
        }

        //This could also work with Substring() instead of AsSpan(), but since that version makes a copy of the string to pass in
        //and we're updating the string so often, AsSpan is better. (AsSpan doesn't allocate new memory.)
        //return textToReveal.AsSpan(0, currentStringIndex).ToString();
        //QUESTION: I keep getting an error that string does not contain a function named AsSpan, even though I'm using the System namespace...

        return textToReveal.Substring(0, currentStringIndex);
     }
}

public class UISystem : Singleton<UISystem>
{
    public TMPro.TextMeshProUGUI dialogueText;

    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;

    private StringReveal typewriter = new StringReveal();

    private Color[] karmaColors = { Color.red, Color.green };

    private void Start()
    {
        activeButtons = new List<GameObject>();

        //Subscribe to DialogueManager's event, and call the ShowUI function once the event is called.
        EvtSystem.EventDispatcher.AddListener<ShowDialogueText>(ShowUI);
        EvtSystem.EventDispatcher.AddListener<DisableUI>(HideUI);
        EvtSystem.EventDispatcher.AddListener<ShowResponses>(ShowResponseButtons);

        //If there is a button in the buttonPool, the button will be assigned to something from the pool. If not, one will be instantiated.
        buttonPool = new Queue<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform); //Instantiate a button as a child under the buttonContainer.
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
    }

    private void ShowUI(ShowDialogueText eventData)
    {
        UIRoot.SetActive(true);

        typewriter.StartReveal(eventData.text, eventData.duration);
    }

    private void HideUI(DisableUI data)
    {
        //Cleans up the buttons and sticks them back into the pool.
        foreach (GameObject button in activeButtons)
        {
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }

        //Disables the parent root of the UI.
        UIRoot.SetActive(false);
    }

    private void ShowResponseButtons(ShowResponses eventData)
    {
        //Force the dialogue line to display fully.
        if (!typewriter.IsDone())
        {
            typewriter.ForceFinish();
            dialogueText.text = typewriter.GetCurrentRevealedText();
        }

        //Rather than deleting and remaking the buttons each time, we'll use object pooling with a queue to reuse the buttons.
        foreach (ResponseData response in eventData.responses)
        {
            GameObject button = null;
            button = buttonPool.Dequeue();

            if (button == null)
            {
                button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform); //Instantiate a button as a child under the buttonContainer.
            }

            button.SetActive(true);

            //Add the button's label.
            TMPro.TextMeshProUGUI label = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
            {
                label.text = response.text;
            }

            //Based on the karma score, decide which color to use. (0 is red here.)
            int colorIndex = 0;
            if (response.karmaScore > 0) //When karma score is positive, set color to green. 
            {
                colorIndex = 1; //1 is green.
            }

            Button uiButton = button.GetComponent<Button>();

            //Set the colors of the button based on the karma score.
            ColorBlock block = uiButton.colors;
            block.normalColor = karmaColors[colorIndex]; //Replace the normal color of the button with the color I've selected.
            uiButton.colors = block;

            uiButton.onClick.RemoveAllListeners(); //Clears out all previous listeners so there aren't duplicates from previous runs.
            uiButton.onClick.AddListener(response.buttonAction);
        }
    }

    private void OnDisable()
    {
        EvtSystem.EventDispatcher.RemoveListener<ShowDialogueText>(ShowUI);
    }

    private void Update()
    {
        if (UIRoot.activeSelf && !typewriter.IsDone())
        {
            dialogueText.text = typewriter.GetCurrentRevealedText();
        }
    }
}



