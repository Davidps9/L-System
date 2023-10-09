using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerator
{
    public static Ruleset GenerateRuleSet(int lenght) // parametrisize when we have kinnetc inputs :)
    {
        Ruleset ruleset = new Ruleset();
        ruleset.axiom = "F";
        /*
         * 
         * F
         * +
         * -
         * 
         */
        List<Rule> rules = new List<Rule>();
        rules.Add(new Rule() { originalCase = 'F', conversion = "FF" });

        ruleset.words = new Word[] {
            new Word() { character = 'F', action = RuleAction.MoveForward, value = 5 },
            new Word() { character = '+', action = RuleAction.RotateZ, value = 45 } ,
            new Word() { character = '-', action = RuleAction.RotateZ, value = -45 } ,
        };
        for (int i = 0; i < lenght; i++)
        {
            rules[0].conversion += ruleset.words[Random.Range(0, ruleset.words.Length)].character;

        }
        ruleset.rules = rules.ToArray();
        return ruleset;

    }
}
