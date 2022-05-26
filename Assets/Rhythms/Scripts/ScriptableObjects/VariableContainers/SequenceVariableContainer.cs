using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Rhythms
{
    [System.Serializable]
    public abstract class SequenceVariableContainer : ScriptableObject
    {
        public abstract int Count();

        public abstract void Add(R_VariableSO var);

        public abstract void Remove(R_VariableSO var);

        public abstract void Clear();

        public abstract R_VariableSO CreateNew(VariableType type);

        public abstract R_VariableSO FindVarByName(string name);

        public abstract bool ContainsName(string name, R_VariableSO var);

        public abstract R_VariableSO VariableAt(int id);

#if UNITY_EDITOR

        public abstract void GUIField(string label, R_VariableSO variable);

#endif
    }
}