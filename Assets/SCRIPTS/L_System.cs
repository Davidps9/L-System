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
        Node previousNode = new();
        Node currentNode = new(previousNode);

        Branch newBranch = CreateBranch(currentNode, transform, "Axiom");
        pushedBranches.Add(newBranch);
        newBranch.GenerateVertex(currentNode, true);

        // float xangle = 0;
        int count = 0;
        foreach (char word in sentence)
        {
            switch (word)
            {
                case 'F':
                    currentNode.localPosition += Vector3.up * 2;
                    pushedBranches.Last().GenerateVertex(currentNode, false);

                    previousNode = currentNode;
                    currentNode = new(previousNode, true);
                    break;

                case 'Z':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(rotationAngle/2, -rotationAngle / 2));
                    currentNode.localRotation += Vector3.forward * ruleset.rotationAngle;
                    break;

                case 'z':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(-rotationAngle / 2, rotationAngle / 2 ));
                    currentNode.localRotation += Vector3.back * ruleset.rotationAngle;
                    break;

                case 'X':
                    currentNode.localRotation += Vector3.right * ruleset.rotationAngle;
                    break;

                case 'x':
                    currentNode.localRotation += Vector3.left * ruleset.rotationAngle;
                    break;

                case '[':
                    pushedBranches.Last().CreateMesh();

                    Debug.Log("creanding new branch");
                    newBranch = CreateBranch(currentNode, pushedBranches.Last().transform, "Branch from open");
                    pushedBranches.Add(newBranch);

                    previousNode = currentNode;

                    currentNode = new(previousNode, true);
                    break;

                case ']':
                    Debug.Log("acabanding new branch");
                    pushedBranches.Last().CreateMesh();
                    previousNode = pushedBranches.Last().rootNode;
                    pushedBranches.RemoveAt(pushedBranches.Count - 1);


                    // meshGeneratorScript.GenerateVertex(currentNode, true);

                    currentNode = new(previousNode, true);

                    newBranch = CreateBranch(currentNode, pushedBranches.Last().transform, "Branch from close");
                    pushedBranches.Add(newBranch);
                    break;
            }
            Debug.Log("Current Node " + count + ": " + currentNode.position);
            count++;
            //Debug.Log("Progress: " + 100.0f * count / sentence.Length + "%");
            yield return new WaitForSeconds(1);
        }
        pushedBranches.Last().CreateMesh();
    }

    Branch CreateBranch(Node rootNode, Transform parent, string name = "Branch")
    {
        GameObject newBranch = new(name);
        newBranch.transform.SetParent(parent);

        newBranch.AddComponent<MeshFilter>();
        newBranch.AddComponent<MeshRenderer>();
        Branch newBranchScript = newBranch.AddComponent<Branch>();
        newBranchScript.SetRootNode(rootNode);
        newBranchScript.SetRenderParameters(sideCount, radius, material);
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
