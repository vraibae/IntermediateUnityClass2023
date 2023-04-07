using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

public class DialogueWindow : EditorWindow
{
    private DialogueLineData m_activeItem = null;
    private InspectorElement m_detailInspector = null;
    private GroupBox m_listBox = null;
    private GroupBox m_detailBox = null;
    private ListView m_listView = null;
    private DialogueDatabase m_currentDatabase = null;

    [MenuItem("Window/MyTools/DialogueWindow")]
    public static void ShowExample()
    {
        DialogueWindow wnd = GetWindow<DialogueWindow>();
        wnd.titleContent = new GUIContent("DialogueWindow");
    }

    private void BindFunc(VisualElement e, int i)
    {
        Label item = e as Label;
        item.text = m_currentDatabase.list[i].name;
    }

    public void PopulateDialogueList()
    {
        m_listBox.Clear();

        if(m_currentDatabase != null)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = BindFunc;

            m_listView = new ListView(m_currentDatabase.list,
                EditorGUIUtility.singleLineHeight,
                makeItem, bindItem);

            m_listView.selectionType = SelectionType.Single;

            m_listBox.Add(m_listView);

            //Set the callback whenever the user clicks on an item.
            m_listView.onSelectionChange += DialogueListSlectionChanged;

            //Show the + and - buttons for add and remove of items.
            m_listView.showAddRemoveFooter = true;
            m_listView.itemsAdded += CreateNewDialogueLine;
            m_listView.itemsRemoved += DeleteSelectedDialogueLine;

            //Styling options
            m_listView.showBorder = true;
            m_listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;

            m_listView.RegisterCallback<FocusOutEvent>(e =>
            {
                //We lost focus onthe listView...did we click on anything in the detail box?
                //If not clear selection.
                if(!m_detailBox.Contains(e.relatedTarget as VisualElement))
                {
                    m_listView.ClearSelection();
                }
            }
            );
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        root.style.flexGrow = 1.0f;

        var objectField = new ObjectField();
        objectField.label = "Dialogue Database";
        objectField.objectType = typeof(DialogueDatabase);

        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e =>
        {
            if(objectField.value != null)
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

        var visualTree = AssetDatabase.
            LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueWindow.uxml");

        VisualElement uxmlData = visualTree.Instantiate();
        uxmlData.style.flexGrow = 1.0f;
        root.Add(uxmlData);

        m_listBox = root.Query<GroupBox>("MainContent").First();
        m_detailBox = root.Query<GroupBox>("DetailBox").First();
    }

    private void CreateNewDialogueLine(IEnumerable<int> itemIndices)
    {
        string path = EditorUtility.SaveFilePanelInProject("Create Dialogue File",
            "DefaultDialogueLine", "asset", "Please select a name for the dialogue file");

        if (path.Length != 0)
        {
            DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
            AssetDatabase.CreateAsset(line, path);
            AssetDatabase.Refresh();

            m_currentDatabase.list[itemIndices.First()] = line;
           // m_currentDatabase.list.Add(line);

            EditorUtility.SetDirty(m_currentDatabase);

            PopulateDialogueList();

            m_listView.SetSelection(m_currentDatabase.list.Count - 1);
        }
    }

    private void DeleteSelectedDialogueLine(IEnumerable<int> itemIndices)
    {
        //We don't need to check itemIndices because we are doing single item selection.
        //If we change that we need to iterate over the indices and convert them to items to remove.
        if(m_activeItem != null)
        {
            m_currentDatabase.list.Remove(m_activeItem);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(m_activeItem));
            EditorUtility.SetDirty(m_currentDatabase);

            m_activeItem = null;

            PopulateDialogueList();

            if (m_currentDatabase.list.Count > 0)
                m_listView.SetSelection(0);
        }
    }

    private void DialogueListSlectionChanged(IEnumerable<object> selectedItems)
    {
        m_activeItem = null;

        foreach(DialogueLineData line in selectedItems)
        {
            m_activeItem = line;
        }

        if(m_detailInspector == null)
        {
            m_detailInspector = new InspectorElement();
            m_detailInspector.style.flexGrow = 1.0f;
            m_detailBox.Add(m_detailInspector);
        }

        if (selectedItems.Count() > 0)
        {
            m_detailInspector.Bind(new SerializedObject(m_activeItem));
            m_detailInspector.visible = true;
        }
        else
        {
            m_detailInspector.visible = false;
        }
    }
}
