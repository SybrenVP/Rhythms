using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Rhythm
{
    [System.Serializable]
    public class SequenceColorContainer : SequenceVariableContainer
    {
        public List<R_ColorSO> Values = new List<R_ColorSO>();

        public override int Count() => Values.Count;

        public override void Add(R_VariableSO var)
        {
            Values.Add(var as R_ColorSO);
        }

        public override void Remove(R_VariableSO var)
        {
            Values.Remove(var as R_ColorSO);
        }

        public override void Clear()
        {
            Values.Clear();
        }

        public override R_VariableSO CreateNew(VariableType type)
        {
            R_ColorSO result = ScriptableObject.CreateInstance<R_ColorSO>();
            result.Init(type, Color.white);

            Add(result);

            return result;
        }


        public override R_VariableSO FindVarByName(string name)
        {
            return Values.Find(so => so.name == name);
        }

        public override bool ContainsName(string name, R_VariableSO var)
        {
            return Values.FindIndex(so => so.name == name && so != var) >= 0;
        }

        public override R_VariableSO VariableAt(int id)
        {
            return Values[id];
        }


#if UNITY_EDITOR

        public override void GUIField(string label, R_VariableSO variable)
        {
            (variable as R_ColorSO).Value = EditorGUILayout.ColorField("Value", (variable as R_ColorSO).Value);
        }

#endif
    }
}