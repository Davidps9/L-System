using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RulesetGenerator
{
    public static Ruleset GenerateRuleSet(int rulesetLength, float branchLength, Vector2 minAngle, Vector2 maxAngle) // parametrisize when we have kinnetc inputs :)
    {
        Dictionary<Word, float> wordProbability = new Dictionary<Word, float>() {
            { new Word() { character = 'F', action = RuleAction.MoveForward, value = branchLength }, 0.4f },
            { new Word() { character = 'Z', action = RuleAction.RotateZ, value = Random.Range(minAngle.y, maxAngle.y) }, 0.1f},
            { new Word() { character = 'z', action = RuleAction.RotateZ, value = Random.Range(-maxAngle.y, -minAngle.y) }, 0.1f },
            { new Word() { character = 'X', action = RuleAction.RotateX, value = Random.Range(minAngle.x,maxAngle.x) }, 0.1f },
            { new Word() { character = 'x', action = RuleAction.RotateX, value = Random.Range(-maxAngle.x, -minAngle.x) }, 0.1f },
            { new Word() { character = '{', action = RuleAction.PushBranch, value = 0}, 0.1f },
            { new Word() { character = '}', action = RuleAction.PopBranch, value = 0}, 0.1f },
        };
        Ruleset ruleset = ScriptableObject.CreateInstance<Ruleset>();
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
        for (int i = 0; i < rulesetLength; i++)
        {
            float index = 0;
            float probab = Random.Range(0f, 1f);
            foreach (KeyValuePair<Word, float> keyValuePair in wordProbability)
            {
                index += keyValuePair.Value;
                if (probab < index)
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
