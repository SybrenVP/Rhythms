using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Rhythm
{
    [System.Serializable]
    public class SequenceStringContainer : SequenceVariableContainer
    {
        public List<R_StringSO> Values = new List<R_StringSO>();

        public override int Count() => Values.Count;

        public override void Add(R_VariableSO var)
        {
            Values.Add(var as R_StringSO);
        }

        public override void Remove(R_VariableSO var)
        {
            Values.Remove(var as R_StringSO);
        }

        public override void Clear()
        {
            Values.Clear();
        }

        public override R_VariableSO CreateNew(VariableType type)
        {
            R_StringSO result = ScriptableObject.CreateInstance<R_StringSO>();
            result.Init(type, string.Empty);

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
            (variable as R_StringSO).Value = EditorGUILayout.TextField("Value", (variable as R_StringSO).Value);
        }

#endif
    }
}