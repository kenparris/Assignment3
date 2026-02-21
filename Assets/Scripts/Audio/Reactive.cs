using System;
using UnityEngine;

public class Reactive : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]
    // Row = time segment (0-10, 10-20, 20-30)
    // Col = sphere id
    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment

    GameObject[] spheres;
    static int numSphere = 300;
    int segmentCount;
    Vector3[,] startPositions;

    Vector3[,] endPositions;
    float[] timeFlags;


    void Start()

    {

        timeFlags = new float[] { 0f, 12f, 50f, 75f, 114f, 138f, 161f };

        segmentCount = timeFlags.Length - 1;

        spheres = new GameObject[numSphere];

        startPositions = new Vector3[segmentCount, numSphere];

        endPositions = new Vector3[segmentCount, numSphere];


        for (int i = 0; i < numSphere; i++)

        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 3f;

            // Segment 0 : 0s -> 12s

            startPositions[0, i] = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + Time.time));

            endPositions[0, i] = Vector3.one;


            // Segment 1 & 3: 12s -> 50s     1:15s -> 1:54s

            startPositions[1, i] = Vector3.one;

            endPositions[1, i] = Vector3.one;


            // Segment 2 & 4: 50s -> 1:15s    1:54s -> 2:18s

            startPositions[2, i] = Vector3.one;

            endPositions[2, i] = Vector3.one;

            // Segment 5 : 2:18s -> 2:41s

            startPositions[3, i] = Vector3.one;

            endPositions[3, i] = Vector3.one;

            // Segment 6 : 2:41s -> 2:55s

            startPositions[4, i] = Vector3.one;

            endPositions[4, i] = Vector3.one;

        }

        for (int i = 0; i < numSphere; i++)
        {
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Position
            spheres[i].transform.position = startPositions[0, i];
            spheres[i].transform.localScale = Vector3.one * 0.3f;
            // Color
            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }


    }


    void Update()

    {
        float currentTime = Time.time;

        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;

            if (currentTime < timeFlags[1])         //Segment 0
            {
                float r = 3f;
                spheres[i].transform.position = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + Time.time));
                spheres[i].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
            }
            //Add lerp transition

            else if (currentTime < timeFlags[2] || currentTime < timeFlags[4])    //Segment 1 & 3
            {

            }

            else if (currentTime < timeFlags[3] || currentTime < timeFlags[5])    //Segment 2
            {

            }

            else if (currentTime < timeFlags[6])    //Segment 5
            {

            }

            else                                    //Segment 6
            {

            }
        }

    }
}
