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
     * FF+[+F-F-F]-[-F+F+F]
     * F+F−F−F+F
     * angulo 25
     * */
    [Header("L-System Parameters")]
    [SerializeField] private string axiom;
    [SerializeField] private Rule[] rules;
    [SerializeField] private float rotationAngle;
    [SerializeField] private GameObject cylinderPrefab;
    [SerializeField] private string pivotBranchTag;
    //  [SerializeField]private Mesh mesh;
    private MeshGenerator meshGeneratorscript;

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
        meshGeneratorscript = GetComponent<MeshGenerator>();
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

    Branch currentBranch;
    private IEnumerator CreateTree()
    {
        List<Branch> pushedBranches = new List<Branch>();
        currentBranch = new();
        Branch previousBranch = new();

        meshGeneratorscript.GenerateVertex(currentBranch.position, Vector3.positiveInfinity, Vector3.zero);

        float xangle = 0;
        int count = 0;
        foreach (char word in sentence)
        {
            yield return new WaitForEndOfFrame();
            count++;

            Debug.Log("Progress: " + 100.0f * count / sentence.Length + "%");
            switch (word)
            {
                case 'F':

                    //GameObject cyl = Instantiate(cylinderPrefab, currentBranch.transform, false);



                    currentBranch.localPosition += Vector3.up * 2;

                    Vector3 angle = previousBranch.rotation + ((currentBranch.rotation - previousBranch.rotation) / 2);

                    meshGeneratorscript.GenerateVertex(currentBranch.position, previousBranch.position, angle);

                    //CombineInstance combineInstance = new CombineInstance();
                    //combineInstance.mesh = mesh;
                    //combineInstance.transform = currentBranch.transform.localToWorldMatrix;
                    //combineInstances.Add(combineInstance);

                    previousBranch = currentBranch;

                    currentBranch = new(previousBranch, true);

                    //Debug.Log(currentBranch.transform.position);
                    //CreateBranch(false, 0, 0, counter, transform);
                    break;
                case '+':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(rotationAngle/2, -rotationAngle / 2));
                    currentBranch.localRotation += new Vector3(xangle, 0, rotationAngle);

                    //CreateBranch(true, 25, 1, counter, transform);

                    break;

                case '-':
                    //xangle = Mathf.Round(UnityEngine.Random.Range(-rotationAngle / 2, rotationAngle / 2 ));
                    currentBranch.localRotation += new Vector3(xangle, 0, -rotationAngle);

                    //CreateBranch(true, 25, -1, counter, transform);

                    break;
                case '[':
                    //if (!previousBranch.CompareTag(pivotBranchTag))
                    //{
                    //    currentBranch.transform.localPosition += Vector3.up * 2;
                    //}
                    pushedBranches.Add(currentBranch.parent);
                    previousBranch = currentBranch;

                    currentBranch = new(previousBranch, true);
                    break;

                case ']':
                    //Destroy(currentBranch);

                    previousBranch = pushedBranches[pushedBranches.Count - 1];
                    pushedBranches.Remove(previousBranch);

                    currentBranch = new(previousBranch, true);
                    break;


            }

        }
        //MergeMeshes(combineInstances);
        meshGeneratorscript.UpdateMesh();
        AssignDefaultShader();
    }
    public void AssignDefaultShader()
    {
        //assign it a white Diffuse shader, it's better than the default magenta
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
        meshRenderer.sharedMaterial.color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            meshGeneratorscript.ResetMesh();
            TurtleConversion();
            Debug.Log(sentence);

        }
    }

    //private void OnDrawGizmos()
    //{
    //    Handles.Label(currentBranch.position, currentBranch.position.ToString());
    //}
}
