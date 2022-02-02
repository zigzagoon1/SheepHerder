using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="HitsToDefeatSO")]
public class HitsToDefeat : ScriptableObject
{
    [SerializeField]
    int _value;
    public int Value { get { return _value; } }
}
