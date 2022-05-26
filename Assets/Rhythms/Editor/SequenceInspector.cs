using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{
    public class SequenceInspector
    {
        public Rhythms.RhythmSequence Sequence;

        public Rect View;
        public Vector2 ScrollPosition = Vector2.zero;

        public int SelectedInspector = 0;
        private readonly string[] InspectorNames =
        {
            "State",
            "Variables",
            "Settings"
        };

        private GUIContent _actionMenuContent;
        private RhythmSequenceEditor _editor;

        private int _selectedVariableTypeId = 0;
        private int _variableNewName = 0;

        private GUIStyle _variableNameStyle;

        public static readonly float MINWIDTH = 400f;

        public SequenceInspector(Rhythms.RhythmSequence sequence, RhythmSequenceEditor editor)
        {
            Sequence = sequence;
            _editor = editor;

            //Create a GUIStyle with toolbar not centered text
            _variableNameStyle = new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft };

            _actionMenuContent = EditorGUIUtility.IconContent("_Menu");
        }

        public void OnGUI()
        {
            GUILayout.BeginArea(View);

            #region Inspector Toolbar

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            SelectedInspector = GUILayout.Toolbar(SelectedInspector, InspectorNames, EditorStyles.toolbarButton);

            EditorGUILayout.EndHorizontal();

            #endregion

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

            switch (SelectedInspector)
            {
                //State inspector
                case 0:
                    DrawStateInspector(StateDrawer.SelectedState);
                    break;

                //Variable inspector
                case 1:
                    DrawVariableInspector();
                    break;

                //Settings inspector
                case 2:
                    DrawSettingsInspector();
                    break;
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        #region StateInspector

        private void DrawStateInspector(Rhythms.RhythmState state)
        {
            if (!state)
                return;

            foreach (Rhythms.RhythmAction action in state.Actions)
            {
                //Create serializedObject and prepare for drawing the inspectors
                var so = new SerializedObject(action);
                so.Update();

                if (DrawActionHeader(so, action))
                    DrawActionBody(so);
            }

            DrawAddAction(state);
        }

        //Returns if the foldout is open
        private bool DrawActionHeader(SerializedObject so, Rhythms.RhythmAction action)
        {
            //Create a rect for the action header
            Rect horizontalRect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);

            SerializedProperty foldoutProp = so.FindProperty("Foldout");
            SerializedProperty enabledProp = so.FindProperty("Enabled");

            foldoutProp.boolValue = EditorGUILayout.Foldout(foldoutProp.boolValue, so.targetObject.name, true);
            enabledProp.boolValue = EditorGUILayout.Toggle(enabledProp.boolValue, GUILayout.Width(15f));
            if (GUILayout.Button(_actionMenuContent, EditorStyles.toolbarPopup, GUILayout.Width(30f)))
            {
                //Create a context menu for this action
                CreateActionContextMenu(action);
            }

            EditorGUILayout.EndHorizontal();

            Utility.DrawRectOutline(horizontalRect, 1, new Color(0.1f, 0.1f, 0.1f));

            if (!foldoutProp.boolValue)
            {
                if (GUI.changed)
                    so.ApplyModifiedProperties();
                return false;
            }

            return true;
        }

        private void DrawActionBody(SerializedObject so)
        {
            EditorGUI.indentLevel++;

            GUILayout.Space(3f);

            SerializedProperty it = so.GetIterator();
            it.NextVisible(true);

            while (it.NextVisible(false))
            {
                EditorGUILayout.PropertyField(it);
            }
            if (GUI.changed)
                so.ApplyModifiedProperties();

            EditorGUI.indentLevel--;

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        private void DrawAddAction(Rhythms.RhythmState state)
        {
            GUILayout.Space(30f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect dropDownRect = GUILayoutUtility.GetRect(View.width * 0.75f, EditorGUIUtility.singleLineHeight * 2f, EditorStyles.toolbarButton, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f), GUILayout.Width(View.width * 0.75f));
            if (state != null && GUI.Button(dropDownRect, "Add Action"))
            {
                var dropdownState = new UnityEditor.IMGUI.Controls.AdvancedDropdownState();
                var dropdown = new ActionDropdown(dropdownState, OnActionAdded, state);
                dropdown.Show(dropDownRect);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30f);
        }

        private void CreateActionContextMenu(Rhythms.RhythmAction action)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Remove Action"), false, RemoveAction, action);

            menu.ShowAsContext();
        }

        private void RemoveAction(object action)
        {
            StateDrawer.SelectedState.Actions.Remove((Rhythms.RhythmAction)action);
        }

        protected void OnActionAdded(System.Type type, Rhythms.RhythmState state)
        {
            Rhythms.RhythmAction action = (Rhythms.RhythmAction)ScriptableObject.CreateInstance(type);
            action.name = type.Name;
            state.Actions.Add(action);
        }

        #endregion

        #region Variable Inspector

        private void DrawVariableInspector()
        {
            if (Sequence.Variables.Variables == null || Sequence.Variables.Variables.Length == 0)
            {
                Sequence.Variables.Init();
                if (Sequence.Variables.Variables == null || Sequence.Variables.Variables.Length == 0)
                    return;
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Add variable", EditorStyles.toolbarButton))
            {
                Rhythms.R_VariableSO newVar = Sequence.Variables.CreateNewVariable((Rhythms.VariableType)_selectedVariableTypeId);
                int loopCount = 0;
                while (Sequence.Variables.DoesNameExist(newVar.Type, newVar.name, newVar))
                {
                    loopCount++;
                    newVar.name = "New " + newVar.Type + " " + loopCount.ToString();
                    newVar.TempName = newVar.name;
                }

                _editor.SaveSequence();
            }

            _selectedVariableTypeId = EditorGUILayout.Popup(_selectedVariableTypeId, Utility.GetVariableList().ToArray(), EditorStyles.toolbarPopup, GUILayout.Width(90f));

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(Sequence.Variables.Count > 0 ? EditorStyles.helpBox : GUIStyle.none);

            foreach (Rhythms.R_VariableSO var in Sequence.Variables)
            {
                if (var == null)
                {
                    Toolbar.SetErrorMessage("Found a null variable, clearing all", 10f);
                    Sequence.Variables.Clear();
                    break;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                if (var.ChangingName)
                {
                    var.TempName = GUILayout.TextField(var.TempName, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
                }
                else
                {
                    var.EditingValue = GUILayout.Toggle(var.EditingValue, var.name, _variableNameStyle); 
                }
                
                bool nameChangeToggle = EditorGUILayout.Toggle(var.ChangingName, EditorStyles.objectFieldMiniThumb ,GUILayout.Width(20f));

                if (var.ChangingName != nameChangeToggle)
                {
                    if (var.ChangingName && Sequence.Variables.DoesNameExist(var.Type, var.TempName, var))
                    {
                        Toolbar.SetErrorMessage("Name already exists", 5f);
                    }
                    else
                    {
                        var.ChangingName = nameChangeToggle;
                        var.name = var.TempName;
                        _editor.SaveSequence();
                    }
                }


                if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(20f)))
                {
                    Sequence.Variables.Remove(var);
                    _editor.SaveSequence();
                    break;
                }

                GUILayout.EndHorizontal();

                if (var.EditingValue)
                {
                    //Draw edit field
                    DrawEditFieldForVariable(var);
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawEditFieldForVariable(Rhythms.R_VariableSO variable)
        {
            EditorGUI.BeginChangeCheck();

            Sequence.Variables.Variables[(int)variable.Type].GUIField("Value", variable);

            if (EditorGUI.EndChangeCheck())
            {
                _editor.SaveSequence();
            }
        }

        #endregion

        #region Settings Inspector

        private void DrawSettingsInspector()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            //Draw audio data
            GUILayout.Label("Audio Data", EditorStyles.boldLabel);

            //Draw inspector of audio data
            var so = new SerializedObject(Sequence.Audio);
            so.Update();

            DrawActionBody(so);

            EditorGUILayout.EndVertical();
        }

        #endregion

    }
}