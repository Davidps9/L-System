using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
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
     * FF+[+F-F-F]-[-F+F+F]
     * F+F−F−F+F
     * angulo 25
     * */
    [Header("L-System Parameters")]
    [SerializeField] private string axiom;
    [SerializeField] private Rule[] rules;
    [SerializeField] private float rotationAngle;
    private MeshGenerator meshGeneratorScript;

    [Serializable]
    class Rule
    {
        public char originalCase;
        public string conversion;
    }

    private string sentence;

    void Start()
    {
        sentence = axiom;
        //TurtleConversion();
        meshGeneratorScript = GetComponent<MeshGenerator>();
    }

    private void TurtleConversion()
    {
        string newSentence = "";
        foreach (char word in sentence)
        {

            bool found = false;

            foreach (Rule rule in rules)
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
        List<Node> pushedBranches = new List<Node>();
        Node previousBranch = new();
        Node currentBranch = new(previousBranch);

        meshGeneratorScript.GenerateVertex(currentBranch, true);

        float xangle = 0;
        int count = 0;
        foreach (char word in sentence)
        {
            switch (word)
            {
                case 'F':
                    currentBranch.localPosition += Vector3.up * 2;
                    meshGeneratorScript.GenerateVertex(currentBranch, false);

                    previousBranch = currentBranch;
                    currentBranch = new(previousBranch, true);
                    break;

                case '+':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(rotationAngle/2, -rotationAngle / 2));
                    currentBranch.localRotation += new Vector3(xangle, 0, rotationAngle);
                    break;

                case '-':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(-rotationAngle / 2, rotationAngle / 2 ));
                    currentBranch.localRotation += new Vector3(xangle, 0, -rotationAngle);
                    break;

                case '[':
                    pushedBranches.Add(currentBranch.parent);
                    previousBranch = currentBranch;

                    currentBranch = new(previousBranch, true);
                    break;

                case ']':
                    previousBranch = pushedBranches[pushedBranches.Count - 1];
                    pushedBranches.Remove(previousBranch);

                    meshGeneratorScript.GenerateVertex(currentBranch, true);

                    currentBranch = new(previousBranch, true);
                    break;
            }

            count++;
            Debug.Log("Progress: " + 100.0f * count / sentence.Length + "%");
            yield return new WaitForEndOfFrame();
        }
        meshGeneratorScript.UpdateMesh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            meshGeneratorScript.ResetMesh();
            TurtleConversion();
            Debug.Log(sentence);
        }
    }
}
