using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

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
    // Start is called before the first frame update

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
       
    }

    void Start()
    {
       
        GenerateRules();    
        TurtleConversion();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            TurtleConversion();
            
        }
    }
}
