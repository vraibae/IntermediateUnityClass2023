using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(DialogueLineData))] //Makes it so for every object of type DialogueLineData, this will be the editor.
public class DialogueLineEditor : Editor //Made this script by opening Create > UI Toolkit > Editor Window, then putting in the name we wanted. We changed it from EditorWindow to Editor though.
{
    /* NOTES:
     * You can build out the UI from code and/or build it within UI Builder!
     * 
     * [MenuItem("Window/UI Toolkit/DialogueLineEditor")] //This can add a menu item under Window > UI Toolkit.
     * 
     * // VisualElements objects can contain other VisualElement following a tree hierarchy.
     * VisualElement label = new Label("Hello World! From C#");
     * root.Add(label);
     */
    DialogueLineData targetLine = null; //Have a variable to store the line data that we're currently editing so that we can later get it's name.

    public override VisualElement CreateInspectorGUI()
    {
        // Each editor window contains a root VisualElement object by default. However, we're going to load our uxml file instead.
        var editorAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueLineEditor.uxml");

        //Create the actual data we want to interact with inside of this editor.
        var root = editorAsset.CloneTree();

        targetLine = target as DialogueLineData; //Casts our target as DialogueLineData before assigning it to targetLine so we can later that object's name.

        //Query<type>() finds all of the objects of this type that have this name and returns them as an iterator.
        //QUESTION: What is an iterator?
        var nameLabel = root.Query<Label>("NameLabel").First(); //First() just grabs the first item of the things returned by the Query.
        nameLabel.text = targetLine.name; //Makes it so the name of the DialogueLineData object itself will be at the top of our editor window!

        //Adds in the Audio Clip field.
        var audioClip = new ObjectField();
        //Makes it so this field will only take objects of the type specified.
        audioClip.objectType = typeof(AudioClip); //TODO: Standardize how audio is being handled (since I was originally using strings) so that this works within the custom editor.
        audioClip.bindingPath = "dialogueAudio";
        audioClip.Bind(serializedObject); //serializedObject is the actual DialogueLineData scriptable object that this editor is showing.
        root.Add(audioClip);

        return root;
    }
}