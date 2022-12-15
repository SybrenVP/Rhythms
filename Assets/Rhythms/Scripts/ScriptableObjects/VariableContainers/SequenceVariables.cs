using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rhythm
{
    /// <summary>
    /// This class will hold lists of all variables
    /// </summary>
    public class SequenceVariables : ScriptableObject, IEnumerable
    {
        public SequenceVariableContainer[] Variables;

        public int Count
        {
            get
            {
                int result = 0;
                Init();

                foreach(SequenceVariableContainer container in Variables)
                {
                    result += container.Count();
                }
                return result;
            }
        }

        #region Helpers

        public void Init()
        {
            if (Variables != null && Variables.Length == System.Enum.GetValues(typeof(VariableType)).Length && Variables[0] != null)
                return;

            Variables = new SequenceVariableContainer[9];

            Variables[0] = CreateInstance<SequenceBoolContainer>();
            Variables[1] = CreateInstance<SequenceFloatContainer>();
            Variables[2] = CreateInstance<SequenceIntContainer>();
            Variables[3] = CreateInstance<SequenceStringContainer>();
            Variables[4] = CreateInstance<SequenceGameObjectContainer>();
            Variables[5] = CreateInstance<SequenceVector2Container>();
            Variables[6] = CreateInstance<SequenceVector3Container>();
            Variables[7] = CreateInstance<SequenceColorContainer>();
            Variables[8] = CreateInstance<SequenceRectContainer>();
        }

        public SequenceVariableContainer GetContainerForType(VariableType type)
        {
            Init();

            if ((int)type >= Variables.Length)
                return null;

            return Variables[(int)type];
        }

        public int AmountOfVariableOfType(VariableType type)
        {
            Init();

            if ((int)type >= Variables.Length)
                return -1;

            return Variables[(int)type].Count();
        }

        public R_VariableSO CreateNewVariable(VariableType type)
        {
            Init();

            return Variables[(int)type].CreateNew(type);
        }

        public void Remove(R_VariableSO var)
        {
            Init();

            Variables[(int)var.Type].Remove(var);
        }

        public void Clear()
        {
            Init();

            foreach (SequenceVariableContainer container in Variables)
            {
                container.Clear();
            }
        }

        public R_VariableSO GetVarByName(VariableType type, string name)
        {
            Init();

            return Variables[(int)type].FindVarByName(name);
        }

        public bool DoesNameExist(VariableType type, string name, R_VariableSO var)
        {
            Init();

            return Variables[(int)type].ContainsName(name, var);
        }

        public SeqVarEnum GetEnumerator()
        {
            return new SeqVarEnum(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }


    public class SeqVarEnum : IEnumerator
    {
        SequenceVariables _variables = null;

        int _arrayPosition = -1;
        VariableType _currentType = VariableType.Bool;

        public SeqVarEnum(SequenceVariables var)
        {
            _variables = var;
        }

        public object Current
        {
            get
            {
                SequenceVariableContainer container = _variables.GetContainerForType(_currentType);
                if (container != null && container.Count() > 0)
                {
                    return container.VariableAt(_arrayPosition);
                }
                return null;
            }
        }

        public bool MoveNext()
        {
            _arrayPosition++;
            SequenceVariableContainer container = _variables.GetContainerForType(_currentType);
            int count = container.Count();

            while (_arrayPosition >= count || count <= 0)
            {
                _currentType = _currentType + 1;
                count = _variables.AmountOfVariableOfType(_currentType);
                _arrayPosition = 0;

                if (count <= 0)
                    return false;
            }

            return (_arrayPosition < count);
        }

        public void Reset()
        {
            _arrayPosition = -1;
            _currentType = VariableType.Bool;
        }
    }

}