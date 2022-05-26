using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{

    [CustomEditor(typeof(Rhythms.RhythmController))]
    public class RhythmControllerEditor : Editor
    {
        private SerializedObject _so;
        private SerializedProperty _prop_Sequence;

        private bool _foldout_SequenceInformation = false;

        public void OnEnable()
        {
            _so = serializedObject;
            _prop_Sequence = _so.FindProperty("ActiveSequence");
        }

        public override void OnInspectorGUI()
        {  
            _so.Update();

            Rhythms.RhythmController controller = target as Rhythms.RhythmController;

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    controller.ActiveSequence.Name = EditorGUILayout.TextField(controller.ActiveSequence.Name);

                    if (GUILayout.Button("Edit", GUILayout.Width(60f)))
                    {
                        //Open rhythm editor
                        var editor = RhythmSequenceEditor.OpenWindow();
                        editor.OpenSequenceFromController(controller, controller.ActiveSequence);
                    }
                }

                EditorGUI.indentLevel++;
                _foldout_SequenceInformation = EditorGUILayout.Foldout(_foldout_SequenceInformation, "Information");
                EditorGUI.indentLevel--;

                if (_foldout_SequenceInformation)
                {
                    if (controller.ActiveSequence != null)
                    {
                        if (controller.ActiveSequence.Audio != null)
                        {
                            Rhythms.AudioData audioData = controller.ActiveSequence.Audio;
                            using (new GUILayout.VerticalScope("Audio data", EditorStyles.helpBox))
                            {
                                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                                if (audioData.Song)
                                    GUILayout.Label("Song: " + audioData.Song?.name);
                                else
                                    GUILayout.Label("No song assigned");
                                GUILayout.Label("Is Looping: " + audioData.Loop.ToString());
                                GUILayout.Label("Bpm: " + audioData.Bpm);
                            }
                        }

                        if (controller.ActiveSequence.Tracks != null)
                        {
                            using (new GUILayout.VerticalScope("Tracks (" + controller.ActiveSequence.Tracks.Count + ")", EditorStyles.helpBox))
                            {
                                GUILayout.Space(EditorGUIUtility.singleLineHeight);

                                for (int i = 0; i < controller.ActiveSequence.Tracks.Count; i++)
                                {
                                    GUILayout.Label(i + ": " + controller.ActiveSequence.Tracks[i].States.Count + " States");
                                }
                            }
                        }
                    }
                }
            }

            if (_so.ApplyModifiedProperties())
            {
                //Something changed
            }
        }
    }
}