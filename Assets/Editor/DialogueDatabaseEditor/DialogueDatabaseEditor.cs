using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

public class DialogueDatabaseEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default; //The m means that it is a member variable, aka local to this class. s can mean static, k can mean constant, etc.

    private DialogueLineData m_activeItem = null;
    private InspectorElement m_detailInspector = null;
    private GroupBox m_listBox = null;
    private ListView m_listView = null;
    private DialogueDatabase m_currentDatabase = null;

    [MenuItem("Window/MyTools/DialogueDatabaseEditor")]
    public static void ShowExample()
    {
        DialogueDatabaseEditor wnd = GetWindow<DialogueDatabaseEditor>();
        wnd.titleContent = new GUIContent("DialogueDatabaseEditor");
    }

    private void BindFunc(VisualElement e, int i)
    {
        Label item = e as Label;
        item.text = m_currentDatabase.data[i].name;
    }

    public void PopulateDialogueList()
    {
        m_listBox.Clear();

        if(m_currentDatabase != null)
        {
            //QUESTION: if both makeItem and bindItem are things that already exist in the documentation, why are we making new functions for them?
            //Func is an internal type that specifies a type of a function, and what's inside of the <> is the return type. And we use that here because ListView.makeItem has that as its type.
            Func<VisualElement> makeItem = () => new Label(); //Another anonymous function, which in this case takes no parameters but returns a label. We're using this since we need to show the items in the list

            //bindItem is defined as an action that takes two parameters: what you want to bind to & the index of the list. So we're trying to create a function that can help us do that.
            Action<VisualElement, int> bindItem = BindFunc;

            m_listView = new ListView(m_currentDatabase.data, (int)EditorGUIUtility.singleLineHeight, makeItem, bindItem); //For some reason it made me cast singleLineHeight to an int

            m_listView.selectionType = SelectionType.Single;
            m_listBox.Add(m_listView);

            //When the selected object changes in the editor (you click on something else), this function will be called.
            m_listView.onSelectionChange += DialogueListSelectionChanged;
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var objectField = new ObjectField();
        objectField.label = "Dialogue Database";
        objectField.objectType = typeof(DialogueDatabase);

        //When this event changes, passes in the changed object. 'e' is the event. So the code inside here won't be called until the callback is registered.
        //This is an anonymous function, as in it's a function with no name. AKA lambda function. An anonymous function can be useful if you're only ever going to use in one place, one time, so you can see what you're doing inline. 
        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e =>
        {
            if (objectField.value != null)
            {
                m_currentDatabase = objectField.value as DialogueDatabase;
            }
            else
            {
                m_currentDatabase = null;
            }

            PopulateDialogueList();
        });

        root.Add(objectField);

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueDatabaseEditor/DialogueDatabaseEditor.uxml");

        VisualElement uxmlData = visualTree.Instantiate();
        root.Add(uxmlData);

        m_listBox = root.Query<GroupBox>("MainContent").First();
    }

    //Will go through all of the items that have been selected, then save the active item.
    private void DialogueListSelectionChanged(IEnumerable<object> selectedItems)
    {
        foreach(DialogueLineData line in selectedItems)
        {
            m_activeItem = line;

        }

        //"Lazily" instantiates the inspector
        if(m_detailInspector == null)
        {
            m_detailInspector = new InspectorElement();
            m_detailInspector.style.flexGrow = 1.0f;
            m_listBox.Add(m_detailInspector);
        }

        if (selectedItems.Count() > 0)
        {
            m_detailInspector.Bind(new SerializedObject(m_activeItem)); //QUESTION: What's a serialized object?
        }
    }
}