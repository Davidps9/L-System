using System.Collections.Generic;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    [SerializeField] private GameObject lsystemPrefab;
    [SerializeField] private float maxInstances = 5;
    private List<GameObject> instances = new List<GameObject>();
    public static LSystemGenerator instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Generate(Vector3 position, Vector2 lean, float height, string name = "L-System")
    {
        GameObject lsystemObject = Instantiate(lsystemPrefab, transform);
        lsystemObject.name = name;
        lsystemObject.transform.position = new Vector3(position.x, 0, position.z);

        if (lsystemObject.TryGetComponent<LSystem>(out var lsystem))
        {
            //Ruleset ruleset = RulesetGenerator.GenerateRuleSet(6, 0.1f, lean, lean);

            Ruleset ruleset = ScriptableObject.CreateInstance<Ruleset>();
            ruleset.axiom = "F";
            ruleset.words = new Word[]
            {
                new Word() { character = 'F', action = RuleAction.MoveForward, value = 0.04f * height },
                new Word() { character = 'Z', action = RuleAction.RotateZ, value = Random.Range(10 + lean.x * 20, 10 + lean.x * 20) },
                new Word() { character = 'z', action = RuleAction.RotateZ, value = Random.Range(-10 + lean.x * 20, -10 + lean.x * 20) },
                new Word() { character = 'X', action = RuleAction.RotateX, value = Random.Range(10 + lean.y * 20, 10 + lean.y * 20) },
                new Word() { character = 'x', action = RuleAction.RotateX, value = Random.Range(-10 + lean.y * 20, -10 + lean.y * 20) },
                new Word() { character = '[', action = RuleAction.PushBranch, value = 0},
                new Word() { character = ']', action = RuleAction.PopBranch, value = 0},
            };
            ruleset.rules = new Rule[]
            {
                new Rule() { originalCase = 'F', conversion = "FFZ[ZFzFzF]z[zFZFZF]" },
                new Rule() { originalCase = 'Z', conversion = "x" },
                new Rule() { originalCase = 'z', conversion = "X" },
                new Rule() { originalCase = 'X', conversion = "z" },
                new Rule() { originalCase = 'x', conversion = "Z" },
            };

            lsystem.GenerateLSystem(ruleset);
            instances.Add(lsystemObject);
            if (instances.Count > maxInstances)
            {
                Destroy(instances[0]);
                instances.RemoveAt(0);
            }
        }
        else
        {
            Destroy(lsystemObject);
            Debug.LogError("LSystem script not found in prefab");
        }
    }
}
