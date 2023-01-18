using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class R_VariablePropertyDrawer : PropertyDrawer
{   
    SerializedProperty _useConstant_prop;
    SerializedProperty _variable_prop;
    SerializedProperty _constant_prop;

    SerializedProperty _varType_prop;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect varRect = position;
        varRect.width = (position.width - 30f);
    
        Rect buttonRect = position;
        buttonRect.x = varRect.width + 10f;
        buttonRect.width = 20f;
    
        _useConstant_prop = property.FindPropertyRelative("UseConstant");
    
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
    
        if (_useConstant_prop.boolValue)
        {
            _constant_prop = property.FindPropertyRelative("Constant");
            EditorGUI.PropertyField(varRect, _constant_prop, new GUIContent(property.displayName));
        }
        else
        {
            var seq = Selection.activeGameObject.GetComponent<Rhythm.RhythmController>().ActiveSequence;

            _varType_prop = property.FindPropertyRelative("Type");
            if (_varType_prop == null)
            {
                Debug.LogError("Could not find the Type of this R_Variable");
                return;
            }
            List<Rhythm.R_VariableSO> possibleVars = GetAllVariablesOfType(seq);
            List<string> options = ConvertVariableListToOptionList(possibleVars);
    
            _variable_prop = property.FindPropertyRelative("Variable");

            Rhythm.R_VariableSO selectedVar =  possibleVars.Find(result => result != null && _variable_prop.objectReferenceValue == result);
            int i = possibleVars.IndexOf(selectedVar);
            if (i < 0)
                i = 0;
    
            int newSelection = EditorGUI.Popup(varRect, property.displayName, i, options.ToArray());
            if (newSelection != i)
            {
                _variable_prop.objectReferenceValue = seq.Variables.GetVarByName((Rhythm.VariableType)_varType_prop.enumValueIndex, options[newSelection]);
            }
        }
    
        if (GUI.Button(buttonRect, "-"))
        {
            _useConstant_prop.boolValue = !_useConstant_prop.boolValue;
        }
    
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    
        EditorGUI.EndProperty();
    }
    
    protected List<Rhythm.R_VariableSO> GetAllVariablesOfType(Rhythm.Sequence seq)
    {
        List<Rhythm.R_VariableSO> result = new List<Rhythm.R_VariableSO>();
        result.Add(null);
    
        foreach (Rhythm.R_VariableSO variable in seq.Variables)
        {
            if (variable == null)
            {
                Debug.Log("found a null variable in the sequence.Variables");
                continue;
            }

            if (IsType(variable))
                result.Add(variable);
        }
    
        return result;
    }

    protected bool IsType(Rhythm.R_VariableSO var)
    {
        return var.Type == (Rhythm.VariableType)_varType_prop.enumValueIndex;
    }
    
    protected List<string> ConvertVariableListToOptionList(List<Rhythm.R_VariableSO> vars)
    {
        List<string> result = new List<string>();
        result.Add("None");
    
        foreach (Rhythm.R_VariableSO var in vars)
        {
            if (var == null) //The first entry is null
            {
                continue;
            }

            result.Add(var.name);
        }
    
        return result;
    }
}