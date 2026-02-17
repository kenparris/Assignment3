// UMD IMDM290 
// Instructor: Myungin Lee

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomly : MonoBehaviour
{
    GameObject[] blocks;
    public GameObject unitBlock;
    static int numSphere = 100;
    Vector3[] initPos, initRotation;
    Color[] initColor;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new GameObject[numSphere];
        initPos = new Vector3[numSphere];
        initRotation = new Vector3[numSphere];
        initColor = new Color[numSphere];

        // Let there be blocks..
        for (int i = 0; i < numSphere; i++)
        {
            float r = 10f; // radius
                           // Draw primitive elements:
            blocks[i] = GameObject.Instantiate(unitBlock);

            // Initial Random Position
            initPos[i] = new Vector3(r * Random.Range(-10f, 10f), r * Random.Range(-10f, 10f), 100f);
            blocks[i].transform.position = initPos[i];
            // Random rotation
            initRotation[i] = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            blocks[i].transform.eulerAngles = initRotation[i];

            // hsv color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            initColor[i] = Color.HSVToRGB((float)i / numSphere, 1f, 1f);

            // Get the renderer of the blocks and assign colors.
            Renderer blocksRenderer = blocks[i].GetComponentInChildren<Renderer>();
            blocksRenderer.material.color = initColor[i];
        }
    }
}
