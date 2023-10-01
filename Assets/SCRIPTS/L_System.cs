using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
    [SerializeField] private GameObject cylinderPrefab;

    

    class Rule
    {
        public char originalCase;
        public string conversion;
    }

    private List<Rule> rules = new List<Rule>();
    private string axiom = "F";
    private string sentence = "";
    public void GenerateRules()
    {
        sentence += axiom;
        Rule rule1 = new Rule();
        rule1.originalCase = 'F';
        rule1.conversion = "FF-[-F+F+F]+[+F-F-F]";
        rules.Add(rule1);

    }
    private void TurtleConversion()
    {
        string newSentence = "";
        foreach (char word  in sentence)
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
        GameObject currentBranch = new("0");
        GameObject currentParent = gameObject;
        float zangle = 0;
        int count = 1;
        foreach (char word in sentence)
        {
            yield return new WaitForEndOfFrame();

            switch (word)
            {           
                case 'F':

                    GameObject cyl = Instantiate(cylinderPrefab, currentBranch.transform, false);
                    currentBranch.transform.SetParent(currentParent.transform, false);
                    currentBranch.transform.position += Vector3.up * 2;

                    GameObject previousBranch = currentBranch;
                    currentBranch = new(count.ToString());
                    count++;
                    currentBranch.transform.position = previousBranch.transform.position;
                    currentBranch.transform.rotation = previousBranch.transform.rotation;

                    Debug.Log(currentBranch.transform.position);
                    //CreateBranch(false, 0, 0, counter, transform);
                    break;
                case'+':
                    zangle += 25;
                    currentBranch.transform.localEulerAngles = new Vector3(0, 0, zangle);

                    //CreateBranch(true, 25, 1, counter, transform);

                    break;

                case '-':
                    zangle -= 25;
                    currentBranch.transform.localEulerAngles = new Vector3(0, 0, zangle);

                    //CreateBranch(true, 25, -1, counter, transform);

                    break;
                case '[':
                    currentBranch.transform.SetParent(currentParent.transform);
                    currentParent = currentBranch;
                    currentBranch = new(count.ToString());
                    count++;
                    break;

                case ']':
                    currentParent = currentParent.transform.parent.gameObject;
                    currentBranch = new(count.ToString());
                    count++;
                    zangle = 0;
                    break;
                


                //default:
                //    counter += 2;
                //    CreateBranch(false, 0, 0, counter, transform);

                //    break;


            }
            
        }
    }

    private void CreateBranch(bool hasToRotate, float angle, float angleDirection, float ObjectCounter, Transform parent)
    {
        float counter = ObjectCounter;
        GameObject branch;

        branch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        branch.transform.parent = parent;

        if (hasToRotate)
        {
            branch.transform.localEulerAngles = new Vector3(angleDirection * angle, 0, -angleDirection * angle);
            branch.transform.position = new Vector3(angleDirection * branch.transform.localScale.x / 2, counter, angleDirection * branch.transform.localScale.x / 2);

        }
        else
        {
            branch.transform.position = new Vector3(0, counter, 0);

        }
    }
    void Start()
    {
        GenerateRules();    
        //TurtleConversion();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            TurtleConversion();
            Debug.Log(sentence);
            
        }
    }
}
