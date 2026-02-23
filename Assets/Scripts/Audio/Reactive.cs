using UnityEngine;

public class Reactive : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]
    // Row = time segment (0-10, 10-20, 20-30)
    // Col = sphere id
    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment

    GameObject[] cubes;
    public static int numObjects = 500;
    public static GameObject mainCamera;
    int segmentCount;
    Vector3[,] startPositions;

    Vector3[,] endPositions;
    float[] timeFlags;
    float audioTime = 0f;
    float transition1Time = 0f;
    bool transtion1 = true;
    bool segment2 = true;


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
        audioTime += Time.deltaTime * Spectrum.audioAmp;
        float currentTime = Time.time;
        float scale = 20f;

        for (int i = 0; i < numObjects; i++)
        {
            float t = i * 2 * Mathf.PI / numObjects;
            float amp = Mathf.Max(Spectrum.samples[Spectrum.mappedMelBins[i / 5]] * scale * scale, 0.3f);
            //amp = Mathf.Min(7f, amp);

            if (currentTime < timeFlags[1])         //Segment 0
            {
                float r = 3f;
                cubes[i].transform.position = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + currentTime));
                
                
                Renderer sphereRenderer = cubes[i].GetComponent<Renderer>();
                Color color = sphereRenderer.material.color;
                Color.RGBToHSV(color, out float h, out _, out float v);
                float s = Mathf.Sin(t + currentTime) / 4f + 0.75f;
                color = Color.HSVToRGB(h, s, v);
                sphereRenderer.material.color = color;

                //audioreactive sizing
                //amp = Mathf.Max(Spectrum.samples[Spectrum.mappedMelBins[i / 5]] * scale * scale, 0.3f);
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f); 
            }

            //lerp transition
            else if (currentTime < timeFlags[2])
            {
                if(transtion1)
                {
                    
                    transtion1 = false;
                }
                cubes[i].transform.position = Vector3.Lerp(startPositions[0,i], endPositions[0,i], transition1Time);
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f); 
            }

            else if (currentTime < timeFlags[3] || currentTime < timeFlags[5])    //Segment 1 & 3
            {
                if (segment2)
                {
                    cubes[i].transform.rotation = Quaternion.Euler(0, 0, (float)i / numObjects * -360f);
                    //cubes[i].transform.Rotate(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
                    if (i == numObjects - 1) segment2 = false;
                }
                //cubes[i].transform.position = new Vector3(r * Mathf.Sin(t + audioTime), r * Mathf.Cos(t + audioTime));
                //amp = Mathf.Clamp(2 * Mathf.Log(amp, 5) + Random.Range(-0.2f,0.2f), 0f, 5f);
                amp = 2 * Mathf.Log(amp, 5) + Random.Range(-0.2f,0.2f);
                float r = 3 + amp * 0.5f;
                cubes[i].transform.position = new Vector3(r * Mathf.Sin(t), r * Mathf.Cos(t), Random.Range(-0.2f,0.2f));
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);

                //Change color
                Vector3 low = new Vector3(Mathf.Sin(t), Mathf.Cos(t)) * 3f;
                Vector3 high = new Vector3(Mathf.Sin(t), Mathf.Cos(t)) * 6f;
                Color color = Color.HSVToRGB(InverseLerp(high, low, cubes[i].transform.position) * 0.17f, 1f, 1f);
                cubes[i].GetComponent<Renderer>().material.color = color;
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
            transition1Time += Time.deltaTime;
        }

    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ab = b - a;
        Vector3 av = value - a;
        return Mathf.Clamp(Vector3.Dot(av, ab) / Vector3.Dot(ab, ab), 0, 1);
    }
}
