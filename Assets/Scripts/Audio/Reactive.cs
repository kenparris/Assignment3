using System;
using UnityEngine;

public class Reactive : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]
    // Row = time segment (0-10, 10-20, 20-30)
    // Col = sphere id
    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment

    GameObject[] cubes;
    static int numObjects = 300;
    int segmentCount;
    Vector3[,] startPositions;

    Vector3[,] endPositions;
    float[] timeFlags;
    float transitionTime = 0;


    void Start()

    {

        timeFlags = new float[] { 0f, 11f, 12f, 50f, 75f, 114f, 138f, 161f };

        segmentCount = timeFlags.Length - 1;

        cubes = new GameObject[numObjects];

        startPositions = new Vector3[segmentCount, numObjects];

        endPositions = new Vector3[segmentCount, numObjects];


        for (int i = 0; i < numObjects; i++)

        {
            float t = i * 2 * Mathf.PI / numObjects;
            float r = 3f;

            // Segment 0 : 0s -> 12s

            startPositions[0, i] = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + 11f));

            endPositions[0, i] = new Vector3(-r * Mathf.Sin(t), -r * Mathf.Cos(t));


            // Segment 1 & 3: 12s -> 50s     1:15s -> 1:54s

            startPositions[1, i] = new Vector3(r * Mathf.Sin(t), r * Mathf.Cos(t));

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

        for (int i = 0; i < numObjects; i++)
        {
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Position
            cubes[i].transform.position = startPositions[0, i];
            cubes[i].transform.localScale = Vector3.one * 0.3f;
            // Color
            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = cubes[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            //float hue = (float)i / numObjects; // Hue cycles through 0 to 1
            float hue = 0.6f;
            Color color = Color.HSVToRGB(hue, (float)i / numObjects / 2f + 0.5f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }


    }


    void Update()

    {
        float currentTime = Time.time;

        for (int i = 0; i < numObjects; i++)
        {
            float t = i * 2 * Mathf.PI / numObjects;

            if (currentTime < timeFlags[1])         //Segment 0
            {
                float r = 3f;
                cubes[i].transform.position = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + Time.time));
                
                
                Renderer sphereRenderer = cubes[i].GetComponent<Renderer>();
                Color color = sphereRenderer.material.color;
                Color.RGBToHSV(color, out float h, out _, out float v);
                float s = Mathf.Sin(t + Time.time) / 4f + 0.75f;
                color = Color.HSVToRGB(h, s, v);
                sphereRenderer.material.color = color;

                //audioreactive sizing
                cubes[i].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
            }

            //lerp transition
            else if (currentTime < timeFlags[2])
            {
                cubes[i].transform.position = Vector3.Lerp(startPositions[0,i], endPositions[0,i], transitionTime);
            }

            else if (currentTime < timeFlags[3] || currentTime < timeFlags[5])    //Segment 1 & 3
            {
                float r = 3f;
                cubes[i].transform.position = new Vector3(r * Mathf.Sin(t + Time.time), r * Mathf.Cos(t + Time.time));

            }

            else if (currentTime < timeFlags[4] || currentTime < timeFlags[6])    //Segment 2
            {

            }

            else if (currentTime < timeFlags[7])    //Segment 5
            {

            }

            else                                    //Segment 6
            {

            }
        }

        if (currentTime < timeFlags[2] && currentTime > timeFlags[1])
        {
            transitionTime += Time.deltaTime;
        }

    }
}
