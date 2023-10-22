using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    [SerializeField] private GameObject lsystemPrefab;
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

    public void Generate(Vector3 position, Vector2 lean, string name = "L-System")
    {
        GameObject lsystemObject = Instantiate(lsystemPrefab, transform);
        lsystemObject.name = name;
        lsystemObject.transform.position = new Vector3(position.x, 0, position.z);

        if (lsystemObject.TryGetComponent<LSystem>(out var lsystem))
        {
            Ruleset ruleset = RulesetGenerator.GenerateRuleSet(6, 0.1f, lean, lean);
            lsystem.GenerateLSystem(ruleset);
        }
        else
        {
            Debug.LogError("LSystem script not found in prefab");
        }
    }
}
