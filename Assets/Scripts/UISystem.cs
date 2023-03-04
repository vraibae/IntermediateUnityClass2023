using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public TMPro.TextMeshProUGUI dialogueText;

    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;

    private void Start()
    {
        activeButtons = new List<GameObject>();

        //Subscribe to DialogueManager's event, and call the ShowUI function once the event is called.
        EvtSystem.EventDispatcher.AddListener<ShowDialogueText>(ShowUI);
        EvtSystem.EventDispatcher.AddListener<ShowResponses>(ShowResponseButtons);

        buttonPool = new Queue<GameObject>();

        for(int i = 0; i <4; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform); //Instantiate a button as a child under the buttonContainer.
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
    }

    private void ShowUI(ShowDialogueText eventData)
    {
        UIRoot.SetActive(true);
        dialogueText.text = eventData.text;
    }

    private void ShowResponseButtons(ShowResponses eventData)
    {
        //Rather than deleting and remaking the buttons each time, we'll use object pooling with a queue to reuse the buttons.
        foreach (ResponseData response in eventData.responses)
        {
            GameObject button = null;
            button = buttonPool.Dequeue();
            //If there is a button in the buttonPool, the button will be assigned to something from the pool. If not, one will be instantiated.
            if ( button == null )
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

            Button uiButton = button.GetComponent<Button>();
            uiButton.onClick.AddListener(response.buttonAction);
        }
    }

    private void OnDisable()
    {
        EvtSystem.EventDispatcher.RemoveListener<ShowDialogueText>(ShowUI);
    }
}
