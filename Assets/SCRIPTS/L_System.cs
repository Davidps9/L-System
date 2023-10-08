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

    [Header("Rendering Parameters")]
    [SerializeField] private float radius;
    [SerializeField] private int sideCount;
    [SerializeField] private Material material;

    private string sentence;

    void Start()
    {
        sentence = ruleset.axiom;
        //TurtleConversion();
    }

    private void TurtleConversion()
    {
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
        List<Branch> pushedBranches = new List<Branch>();

        Node currentNode = Node.Zero;
        currentNode.radius = radius;

        Branch newBranch = CreateBranch(transform, currentNode, "Axiom");
        pushedBranches.Add(newBranch);
        currentNode = null;

        // float xangle = 0;
        int count = 0;
        foreach (char word in sentence)
        {
            yield return new WaitForSeconds(0.2f);

            Word wordInfo = Array.Find(ruleset.words, w => w.character == word);
            if (wordInfo == null)
            {
                Debug.LogError("Word not defined: " + word);
                yield return 0;
            }

            if (currentNode == null)
            {
                currentNode = pushedBranches.Last().CreateNode(radius);
                Debug.Log("Current Node " + count + ": " + currentNode.position);
            }

            switch (wordInfo.action)
            {
                case RuleAction.MoveForward:
                    currentNode.localPosition += Node.RotatedPosition(Vector3.up * wordInfo.value, currentNode.localRotation);
                    //currentNode.localPosition += Vector3.up * wordInfo.value;
                    pushedBranches.Last().ApplyNode(currentNode);

                    currentNode = null;
                    //currentNode = new(previousNode, true);
                    break;

                case RuleAction.RotateZPositive:
                    //xangle = Mathf.Round(UnityEngine.Random.Range(rotationAngle/2, -rotationAngle / 2));
                    currentNode.localRotation += Vector3.forward * wordInfo.value;
                    break;

                case RuleAction.RotateZNegative:
                    //xangle = Mathf.Round(UnityEngine.Random.Range(-rotationAngle / 2, rotationAngle / 2 ));
                    currentNode.localRotation += Vector3.back * wordInfo.value;
                    break;

                case RuleAction.RotateXPositive:
                    currentNode.localRotation += Vector3.right * wordInfo.value;
                    break;

                case RuleAction.RotateXNegative:
                    currentNode.localRotation += Vector3.left * wordInfo.value;
                    break;

                case RuleAction.PushBranch:
                    pushedBranches.Last().CreateMesh(sideCount, material);

                    Debug.Log("creanding new branch");
                    newBranch = CreateBranch(pushedBranches.Last().transform, currentNode, "Branch from open");
                    pushedBranches.Add(newBranch);

                    currentNode = null;
                    break;

                case RuleAction.PopBranch:
                    Debug.Log("acabanding new branch");
                    pushedBranches.Last().CreateMesh(sideCount, material);

                    //previousNode = pushedBranches.Last().rootNode;
                    pushedBranches.RemoveAt(pushedBranches.Count - 1);

                    // meshGeneratorScript.GenerateVertex(currentNode, true);

                    currentNode = null;

                    //newBranch = CreateBranch(currentNode, pushedBranches.Last().transform, "Branch from close");
                    //pushedBranches.Add(newBranch);
                    break;
            }

            count++;
            //Debug.Log("Progress: " + 100.0f * count / sentence.Length + "%");

        }
        pushedBranches.Last().CreateMesh(sideCount, material);
    }

    Branch CreateBranch(Transform parent, Node rootNode, string name = "Branch")
    {
        GameObject newBranch = new(name);

        newBranch.AddComponent<MeshFilter>();
        newBranch.AddComponent<MeshRenderer>();
        Branch newBranchScript = newBranch.AddComponent<Branch>();
        newBranchScript.Initialize(parent, rootNode);
        return newBranchScript;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            TurtleConversion();
            Debug.Log(sentence);
        }
    }
}
