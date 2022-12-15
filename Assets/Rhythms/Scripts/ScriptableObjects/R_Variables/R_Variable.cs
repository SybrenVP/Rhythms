using UnityEngine;
using System.Collections;

namespace Rhythm
{
    #region Base

    /// <summary>
    /// This class is a wrapper for custom actions to be able to use 2 different types of variables. 
    /// 1. Just a value, this value is not saved in a scriptable object. 
    /// 2. A scriptable object, this value will be saved and can be used by later actions to adjust behaviour during a sequence
    /// </summary>
    [System.Serializable]
    public class R_Variable
    {
        public string Name = "New Variable";

        public bool UseConstant = false;

        public object Constant;
        public R_VariableSO Variable;

        public object RawObjectValue { get { return UseConstant ? Constant : Variable.Value; } }

        public object Value { get { return UseConstant ? Constant : Variable.Value; } }

        public VariableType Type
        {
            get;
            protected set;
        }
    
        public R_Variable()
        {
            Constant = null;
        }

        public R_Variable(object value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Bool

    [System.Serializable]
    public class R_Bool : R_Variable
    {
        public new bool Constant = false;
        public new bool Value
        {
            get
            {
                return UseConstant ? Constant : (bool)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Bool;

        public R_Bool()
        {
            Constant = false;
            Type = VariableType.Bool;
        }

        public R_Bool(bool value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Float

    [System.Serializable]
    public class R_Float : R_Variable
    {
        public new float Constant = 0f;
        public new float Value
        {
            get
            {
                return UseConstant ? (float)Constant : (float)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Float;

        public R_Float()
        {
            Constant = 0f;
            Type = VariableType.Float;
        }

        public R_Float(float value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Int

    [System.Serializable]
    public class R_Int : R_Variable
    {
        public new int Constant = 0;
        public new int Value
        {
            get
            {
                return UseConstant ? (int)Constant : (int)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Int;

        public R_Int()
        {
            Constant = 0;
            Type = VariableType.Int;
        }

        public R_Int(int value)
        {
            Constant = value;
        }
    }

    #endregion

    #region String

    [System.Serializable]
    public class R_String : R_Variable
    {
        public new string Constant = "";
        public new string Value
        {
            get
            {
                return UseConstant ? (string)Constant : (string)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.String;

        public R_String()
        {
            Constant = "";
            Type = VariableType.String;
        }

        public R_String(string value)
        {
            Constant = value;
        }
    }

    #endregion

    #region GameObject

    [System.Serializable]
    public class R_GameObject : R_Variable
    {
        public new GameObject Constant = null;
        public new GameObject Value
        {
            get
            {
                return UseConstant ? (GameObject)Constant : (GameObject)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.GameObject;

        public R_GameObject()
        {
            Constant = null;
            Type = VariableType.GameObject;
        }

        public R_GameObject(GameObject value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Vector2

    [System.Serializable]
    public class R_Vector2 : R_Variable
    {
        public new Vector2 Constant = Vector2.zero;
        public new Vector2 Value
        {
            get
            {
                return UseConstant ? (Vector2)Constant : (Vector2)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Vector2;

        public R_Vector2()
        {
            Constant = Vector2.zero;
            Type = VariableType.Vector2;
        }

        public R_Vector2(Vector2 value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Vector3

    [System.Serializable]
    public class R_Vector3 : R_Variable
    {
        public new Vector3 Constant = Vector3.zero;
        public new Vector3 Value
        {
            get
            {
                return UseConstant ? (Vector3)Constant : (Vector3)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Vector3;

        public R_Vector3()
        {
            Constant = Vector3.zero;
            Type = VariableType.Vector3;
        }

        public R_Vector3(Vector3 value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Color

    [System.Serializable]
    public class R_Color : R_Variable
    {
        public new Color Constant = Color.white;
        public new Color Value
        {
            get
            {
                return UseConstant ? (Color)Constant : (Color)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Color;

        public R_Color()
        {
            Constant = Color.white;
            Type = VariableType.Color;
        }

        public R_Color(Color value)
        {
            Constant = value;
        }
    }

    #endregion

    #region Rect

    [System.Serializable]
    public class R_Rect : R_Variable
    {
        public new Rect Constant = Rect.zero;
        public new Rect Value
        {
            get
            {
                return UseConstant ? (Rect)Constant : (Rect)Variable.Value;
            }
            set
            {
                if (UseConstant)
                    Constant = value;
                else
                    Variable.Value = value;
            }
        }

        public new VariableType Type = VariableType.Rect;

        public R_Rect()
        {
            Constant = Rect.zero;
            Type = VariableType.Rect;
        }

        public R_Rect(Rect value)
        {
            Constant = value;
        }
    }

    #endregion
}