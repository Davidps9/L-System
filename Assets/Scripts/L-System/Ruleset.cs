using System;
using UnityEngine;

[CreateAssetMenu(menuName = "L-System/Ruleset")]
public class Ruleset : ScriptableObject
{
    public string axiom;
    public Rule[] rules;
    public Word[] words;
}

[Serializable]
public class Rule
{
    public char originalCase;
    public string conversion;
}

[Serializable]
public class Word
{
    public char character;
    public RuleAction action;
    public float value;
}

public enum RuleAction
{
    MoveForward,
    RotateZ,
    RotateX,
    PushBranch,
    PopBranch
}