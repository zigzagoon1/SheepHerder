using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpeedSO")]
public class SpeedSO : ScriptableObject
{
    [SerializeField] int _value;
    public int Value { get { return _value; } }
}
