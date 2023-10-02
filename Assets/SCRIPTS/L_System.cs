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
    [SerializeField] private string axiom;
    [SerializeField] private Rule[] rules;
    [SerializeField] private float rotationAngle;
    [SerializeField] private GameObject cylinderPrefab;
    [SerializeField] private string pivotBranchTag;

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

        if (gameObject.transform.childCount > 0)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }
        StartCoroutine(CreateTree());
    }

    private IEnumerator CreateTree()
    {
        List<GameObject> pushedBranches = new List<GameObject>();
        GameObject currentBranch = new("0");
        GameObject previousBranch = gameObject;
        currentBranch.transform.SetParent(previousBranch.transform, false);

        float xangle = 0;

        //float zangle = 0;
        int count = 1;
        foreach (char word in sentence)
        {
            yield return new WaitForEndOfFrame();

            switch (word)
            {
                case 'F':

                    GameObject cyl = Instantiate(cylinderPrefab, currentBranch.transform, false);
                    if (!previousBranch.CompareTag(pivotBranchTag))
                    {
                        currentBranch.transform.localPosition += Vector3.up * 2;
                    }

                    previousBranch = currentBranch;
                    currentBranch = new(count.ToString());
                    currentBranch.transform.SetParent(previousBranch.transform, false);
                    count++;

                    //Debug.Log(currentBranch.transform.position);
                    //CreateBranch(false, 0, 0, counter, transform);
                    break;
                case '+':
                    xangle = Mathf.Round(UnityEngine.Random.Range(rotationAngle/2, -rotationAngle / 2));
                    currentBranch.transform.localEulerAngles += new Vector3(xangle, 0, rotationAngle);

                    //CreateBranch(true, 25, 1, counter, transform);

                    break;

                case '-':
                    xangle = Mathf.Round(UnityEngine.Random.Range(-rotationAngle / 2, rotationAngle / 2 ));
                    currentBranch.transform.localEulerAngles = new Vector3(xangle, 0, -rotationAngle);

                    //CreateBranch(true, 25, -1, counter, transform);

                    break;
                case '[':
                    currentBranch.tag = "PivotBranch";
                    currentBranch.name = "Push";
                    if (!previousBranch.CompareTag(pivotBranchTag))
                    {
                        currentBranch.transform.localPosition += Vector3.up * 2;
                    }
                    pushedBranches.Add(currentBranch.transform.parent.gameObject);
                    previousBranch = currentBranch;

                    currentBranch = new(count.ToString());
                    currentBranch.transform.SetParent(previousBranch.transform, false);
                    count++;
                    break;

                case ']':
                    Destroy(currentBranch);

                    previousBranch = pushedBranches[pushedBranches.Count - 1];
                    pushedBranches.Remove(previousBranch);

                    currentBranch = new(count.ToString());
                    currentBranch.transform.SetParent(previousBranch.transform, false);

                    count++;
                    break;


            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TurtleConversion();
            Debug.Log(sentence);

        }
    }
}
