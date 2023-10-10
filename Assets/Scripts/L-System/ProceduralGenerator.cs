using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProceduralGenerator
{
    public static Ruleset GenerateRuleSet(int lenght, Vector2 maxAngles) // parametrisize when we have kinnetc inputs :)
    {
        Dictionary<Word, float> wordProbability = new Dictionary<Word, float>() {
            { new Word() { character = 'F', action = RuleAction.MoveForward, value = 5 }, 0.4f },
            { new Word() { character = 'Z', action = RuleAction.RotateZ, value = Random.Range(0, maxAngles.y) }, 0.1f},
            { new Word() { character = 'z', action = RuleAction.RotateZ, value = Random.Range(-maxAngles.y, 0) }, 0.1f },
            { new Word() { character = 'X', action = RuleAction.RotateX, value = Random.Range(0,maxAngles.x) }, 0.1f },
            { new Word() { character = 'x', action = RuleAction.RotateX, value = Random.Range(-maxAngles.x, 0) }, 0.1f },
            { new Word() { character = '{', action = RuleAction.PushBranch, value = 0}, 0.1f },
            { new Word() { character = '}', action = RuleAction.PopBranch, value = 0}, 0.1f },
        };
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

        ruleset.words = wordProbability.Keys.ToArray();
        for (int i = 0; i < lenght; i++)
        {
            float index = 0;
            float probab = Random.Range(0f, 1f);
            foreach(KeyValuePair<Word, float> keyValuePair in wordProbability)
            {
                index += keyValuePair.Value;
                if(probab < index)
                {
                    rules[0].conversion += keyValuePair.Key.character;
                    break;
                }
            }
            

        }
        ruleset.rules = rules.ToArray();
        return ruleset;

    }
}
