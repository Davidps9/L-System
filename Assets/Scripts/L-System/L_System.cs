using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_System : MonoBehaviour
{

    /*
    case 't': //translate Center Downard
    case 'T': //translate Center Upward
    case 's': //scale radius smaller
    case 'S': //scale radius larger
    case 'x': //Counter-Clockwise around X axis
    case 'X': //Clockwise around X axis
    case 'y': //Counter-Clockwise around Y axis
    case 'Y': //Clockwise around Y axis
    case 'z': //Counter-Clockwise around Z axis
    case 'Z': //Clockwise around Z axis
    case '[': //push point onto the stack
    case ']': //pop point from the stack
    case 'C': //'Closes' a point to stop it from drawing bad geometry
    case '+': //Adds a center point for mesh Generation
    case 'F': //Combines 'T' and '+' for continuity with L-System
     */

    /*
     * 
     * 'F': End transformations
     * '-': Rotate CCW
     * '+': Rotate CW
     * '[': Set as child of previous object
     * ']': Go up 1 level in hierarchy
     * 
     * */
    [Header("L-System Parameters")]
    [SerializeField] private Ruleset ruleset;
    [SerializeField] private bool procedurallyGenerate = false;
    [SerializeField] private GameObject branchPrefab;

    [Header("Rendering Parameters")]
    [SerializeField] private float radius;
    [SerializeField] private float numberOfStages;
    [SerializeField] private int sideCount;
    [SerializeField] private Material material;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool showProgress = false;
#endif

    private float iterations = 1;
    private string sentence = "", previousSentence = "";
    private List<Branch> pushedBranches = new();

    void Start()
    {
        GenerateLSystem();
    }

    private void TurtleConversion()
    {
        if (sentence.Length > 0)
        {
            previousSentence = sentence;
        }
        else
        {
            sentence = ruleset.axiom;
        }

        string newSentence = "";
        foreach (char word in sentence)
        {
            bool found = false;

            foreach (Rule rule in ruleset.rules)
            {
                if (word == rule.originalCase)
                {
                    found = true;
                    newSentence += rule.conversion;
                    break;
                }
            }

            if (!found)
            {
                newSentence += word;
            }
        }
        sentence = newSentence;
        StartCoroutine(CreateTree());
    }

    private IEnumerator CreateTree()
    {
        Node currentNode = null;
        string workingSentence = sentence;

        // check if current sentence starts like previous sentence
        if (previousSentence.Length > 0 && sentence.Substring(0, previousSentence.Length) == previousSentence)
        {
            workingSentence = sentence.Substring(previousSentence.Length);
        }
        else
        {
            pushedBranches.Clear();

            currentNode = Node.Zero;
            currentNode.radius = radius;

            Branch newBranch = CreateBranch(transform, currentNode, "Axiom");
            pushedBranches.Add(newBranch);
            currentNode = null;
        }

        int count = 0;
        foreach (char word in workingSentence)
        {
            Word wordInfo = Array.Find(ruleset.words, w => w.character == word);
            if (wordInfo == null)
            {
                Debug.LogError("Word not defined: " + word);
                yield return 0;
            }

            currentNode ??= pushedBranches.Last().CreateNode(radius);

            switch (wordInfo.action)
            {
                case RuleAction.MoveForward:
                    currentNode.localPosition += Node.RotatedPosition(Vector3.up * wordInfo.value, currentNode.localRotation);
                    pushedBranches.Last().ApplyNode(currentNode);

                    currentNode = null;
                    break;

                case RuleAction.RotateZ:
                    currentNode.localRotation += Vector3.forward * wordInfo.value;
                    break;

                case RuleAction.RotateX:
                    currentNode.localRotation += Vector3.right * wordInfo.value;
                    break;

                case RuleAction.PushBranch:

                    pushedBranches.Last().CreateMesh(sideCount, material);

                    //Debug.Log("creanding branch " + (pushedBranches.Count + 1));
                    Branch newBranch = CreateBranch(pushedBranches.Last().transform, currentNode, "Branch lvl " + (pushedBranches.Count + 1));
                    pushedBranches.Add(newBranch);

                    currentNode = null;

                    yield return new WaitForEndOfFrame();
                    break;

                case RuleAction.PopBranch:

                    if (pushedBranches.Count <= 1) { break; }
                    //Debug.Log("Ending branch " + pushedBranches.Count);
                    pushedBranches.Last().CreateMesh(sideCount, material);

                    pushedBranches.RemoveAt(pushedBranches.Count - 1);
                    currentNode = null;

                    yield return new WaitForEndOfFrame();
                    break;
            }

#if UNITY_EDITOR
            if (showProgress)
            {
                count++;
                Debug.Log("Iteration " + iterations + " progress: " + 100.0f * count / workingSentence.Length + "%");
            }
#endif
        }
        pushedBranches.Last().CreateMesh(sideCount, material);

        // cleanup extra iterations
        if (transform.childCount > 1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        yield return new WaitForEndOfFrame();

        if (iterations < numberOfStages)
        {
            iterations++;
            TurtleConversion();
        }
    }

    Branch CreateBranch(Transform parent, Node rootNode, string name = "Branch")
    {
        GameObject newBranch = Instantiate(branchPrefab);
        newBranch.name = name;
        if (newBranch.TryGetComponent<Branch>(out var branchScript))
        {
            branchScript.Initialize(parent, rootNode);
            return branchScript;
        }

        Debug.LogError("The prefab does not have a Branch script attached to it.");
        return null;
    }

    public void GenerateLSystem()
    {
        if (procedurallyGenerate)
        {
            ruleset = ProceduralGenerator.GenerateRuleSet(10, new Vector2(25, 45));
        }

        TurtleConversion();
    }
}
