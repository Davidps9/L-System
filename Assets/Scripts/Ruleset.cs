using System;
using UnityEngine;

[CreateAssetMenu(menuName = "L-System/Ruleset")]
public class Ruleset : ScriptableObject
{
    public string axiom;
    public Rule[] rules;
    public float rotationAngle;
}

[Serializable]
public class Rule
{
    public char originalCase;
    public string conversion;
}