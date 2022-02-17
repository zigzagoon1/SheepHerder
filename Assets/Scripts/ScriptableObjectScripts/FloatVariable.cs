using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "FloatVariableSO")]
public class FloatVariable : ScriptableObject
{
    public float Value;
}


[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;

    public FloatVariable Variable;

    public float Value { get { return UseConstant ? ConstantValue : Variable.Value; } }
}