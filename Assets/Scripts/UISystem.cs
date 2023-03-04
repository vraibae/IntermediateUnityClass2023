using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    /* Want it to be 
     */

    public TMPro.TextMeshProUGUI dialogueText;

    private List<GameObject> activeButtons;
    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private void Start()
    {
        activeButtons = new List<GameObject>();

        //Subscribe to DialogueManager's event, and call the ShowUI function once the event is called.
        EvtSystem.EventDispatcher.AddListener<ShowDialogueText>(ShowUI);
        EvtSystem.EventDispatcher.AddListener<ShowResponses>(ShowResponseButtons);
    }

    private void ShowUI(ShowDialogueText eventData)
    {
        UIRoot.SetActive(true);
        dialogueText.text = eventData.text;
    }

    private void ShowResponseButtons(ShowResponses eventData)
    {
        foreach (ResponseData response in eventData.responses)
        {
            var button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform); //Instantiate a button as a child under the buttonContainer.

            //Add the button's label.
            TMPro.TextMeshProUGUI label = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
            {
                label.text = response.text;
            }

            Button uiButton = button.GetComponent<Button>();
            uiButton.onClick.AddListener(response.buttonAction);
        }
    }

    private void OnDisable()
    {
        EvtSystem.EventDispatcher.RemoveListener<ShowDialogueText>(ShowUI);
    }
}
