using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    /// <summary>
    /// This a scriptable object class for variables, this is the class used by R_Variable to save away the variable data if requested by designers
    /// </summary>
    [System.Serializable]
    public class R_VariableSO : ScriptableObject
    {
        public VariableType Type = VariableType.Bool;

        //the type object cannot be serialized properly by Unity, which is why we have to create child classes with the actual serializable type
        //Because of this issue we inherently have to create separate classes for each type we want to support
        public object Value;

        public virtual void Init(VariableType type, object value)
        {
            name = "New " + type.ToString();
            TempName = name;
            Type = type;

            Value = value;
        }

#if UNITY_EDITOR

        public bool ChangingName = false;
        public string TempName;

        public bool EditingValue = false;

#endif
    }
}