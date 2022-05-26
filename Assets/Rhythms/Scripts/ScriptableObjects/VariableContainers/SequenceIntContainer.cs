using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Rhythms
{
    [System.Serializable]
    public class SequenceIntContainer : SequenceVariableContainer
    {
        public List<R_IntSO> Values = new List<R_IntSO>();

        public override int Count() => Values.Count;

        public override void Add(R_VariableSO var)
        {
            Values.Add(var as R_IntSO);
        }

        public override void Remove(R_VariableSO var)
        {
            Values.Remove(var as R_IntSO);
        }

        public override void Clear()
        {
            Values.Clear();
        }

        public override R_VariableSO CreateNew(VariableType type)
        {
            R_IntSO result = ScriptableObject.CreateInstance<R_IntSO>();
            result.Init(type, 0);

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
            (variable as Rhythms.R_IntSO).Value = EditorGUILayout.IntField(label, (variable as Rhythms.R_IntSO).Value);
        }

#endif
    }
}