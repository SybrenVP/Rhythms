using Rhythm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConnection : ScriptableObject
{
    public R_Variable Output = new R_Variable();
    public R_Variable Input = new R_Variable();

    public void Start()
    {
        //Output.OnChange += (object newValue) => Input.Variable.Value = newValue;
    }
}
