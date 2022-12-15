using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Rhythm
{
    [System.Serializable]
    public class SequenceGameObjectContainer : SequenceVariableContainer
    {
        public List<R_GameObjectSO> Values = new List<R_GameObjectSO>();

        public override int Count() => Values.Count;

        public override void Add(R_VariableSO var)
        {
            Values.Add(var as R_GameObjectSO);
        }

        public override void Remove(R_VariableSO var)
        {
            Values.Remove(var as R_GameObjectSO);
        }

        public override void Clear()
        {
            Values.Clear();
        }

        public override R_VariableSO CreateNew(VariableType type)
        {
            R_GameObjectSO result = ScriptableObject.CreateInstance<R_GameObjectSO>();
            result.Init(type, null);

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
            (variable as R_GameObjectSO).Value = (GameObject)EditorGUILayout.ObjectField("Value", (variable as R_GameObjectSO).Value, typeof(GameObject), true);
        }

#endif
    }
}