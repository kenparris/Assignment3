using UnityEngine;

public class Reactive : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]
    // Row = time segment (0-10, 10-20, 20-30)
    // Col = sphere id
    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment

    GameObject[] cubes;
    public static int numObjects = 600;
    public GameObject mainCamera;
    public AudioSource song;
    public float skipTime = 0f;
    public Material translucentMaterial;
    int segmentCount;
    Vector3[,] startPositions;
    Vector3[,] endPositions;
    float[] timeFlags;
    float audioTime = 0f;
    float transition1Time = 0f;
    bool transtion1 = true;
    bool segment2 = true;
    bool segment3 = true;
    GameObject centerSphere;
    //GameObject[] centerCubes;
    GameObject[] backBars;


    void Start()

    {
        timeFlags = new float[] { 0f, 11f, 12f, 50f, 76f, 114f, 138f, 161f };

        segmentCount = timeFlags.Length - 1;

        cubes = new GameObject[numObjects];
        //centerCubes = new GameObject[10];
        backBars = new GameObject[300];

        startPositions = new Vector3[segmentCount, numObjects];

        endPositions = new Vector3[segmentCount, numObjects];

        song.time = skipTime;

        for (int i = 0; i < numObjects; i++)

        {
            float t = i * 2 * Mathf.PI / numObjects;
            float r = 3f;
            float twopi = 2f * Mathf.PI;
            float sin = Mathf.Sin(t);
            float cos = Mathf.Cos(t);

            // Segment 0 : 0s -> 12s

            startPositions[0, i] = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + 11f));

            endPositions[0, i] = new Vector3(-r * Mathf.Sin(t), -r * Mathf.Cos(t));


            // Segment 1 & 3: 12s -> 50s     1:15s -> 1:54s

            startPositions[1, i] = new Vector3(r * sin, r * cos, 5f);

            endPositions[1, i] = Vector3.one;


            // Segment 2 & 4: 50s -> 1:15s    1:54s -> 2:18s
            if (i < numObjects / 3)
            {
                startPositions[2, i] = new Vector3(3f * t * Mathf.Sin(3f * t), 3f * t * Mathf.Cos(3f * t), 5f * (twopi - 3f * t - 1));
            }
            else if (i < numObjects * 2 / 3)
            {
                startPositions[2, i] = new Vector3((3f * t - twopi) * Mathf.Sin(3f * t + (twopi / 3f)), (3f * t - twopi) * Mathf.Cos(3f * t + (twopi / 3f)), 5f * (2f * twopi - 3f * t - 1));
            }
            else
            {
                startPositions[2, i] = new Vector3((3f * t - 2f * twopi) * Mathf.Sin(3f * t + (2f * twopi / 3f)), (3f * t - 2f * twopi) * Mathf.Cos(3f * t + (2f * twopi / 3f)), 5f * (3f * twopi - 3f * t - 1));
            }


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
            //sphereRenderer.material = translucentMaterial;
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
        float currentTime = skipTime + Time.time;
        float scale = 20f;

        for (int i = 0; i < numObjects; i++)
        {
            float t = i * 2 * Mathf.PI / numObjects;
            float amp = Mathf.Max(Spectrum.samples[Spectrum.mappedMelBins[i / 6 + 25]] * scale * scale, 0.3f);

            if (currentTime < timeFlags[1])         //Segment 0
            {
                float r = 3f;
                cubes[i].transform.position = new Vector3(r * t - r * Mathf.PI, Mathf.Sin(r * t * Mathf.Cos(t) + currentTime));


                Renderer sphereRenderer = cubes[i].GetComponent<Renderer>();
                Color color = sphereRenderer.material.color;
                Color.RGBToHSV(color, out float h, out _, out float v);
                float sat = Mathf.Sin(t + currentTime) / 4f + 0.75f;
                sphereRenderer.material.color = Color.HSVToRGB(h, sat, v);

                //audioreactive sizing
                //amp = Mathf.Max(Spectrum.samples[Spectrum.mappedMelBins[i / 5]] * scale * scale, 0.3f);
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
            }

            //lerp transition
            else if (currentTime < timeFlags[2])
            {
                if (transtion1)
                {

                    transtion1 = false;
                }
                cubes[i].transform.position = Vector3.Lerp(startPositions[0, i], endPositions[0, i], transition1Time);
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
            }

            else if (currentTime < timeFlags[3] || (currentTime < timeFlags[5] && currentTime > timeFlags[4]))    //Segment 1 & 3
            {
                if (segment2)
                {
                    cubes[i].transform.rotation = Quaternion.Euler(0, 0, (float)i / numObjects * -360f);
                    //cubes[i].transform.Rotate(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));

                    // if (i < 10)
                    // {
                    //     centerCubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //     centerCubes[i].transform.position = new Vector3(0, 0, 5f);
                    //     centerCubes[i].transform.Rotate(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
                    // }

                    if (i == numObjects - 1)
                    {
                        centerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        centerSphere.transform.position = new Vector3(0, 0, 5f);
                        segment2 = false;
                        segment3 = true;
                    }
                }

                //amp = Mathf.Clamp(2 * Mathf.Log(amp, 5) + Random.Range(-0.2f,0.2f), 0f, 5f);
                amp = 3 * Mathf.Log(amp, 5) * Random.Range(0.9f, 1.1f);
                float r = 4 + amp * 0.5f;
                cubes[i].transform.position = new Vector3(r * Mathf.Sin(t), r * Mathf.Cos(t), 5f + Random.Range(-0.2f, 0.2f));
                //cubes[i].transform.position = new Vector3(r * Mathf.Sin(t + audioTime), r * Mathf.Cos(t + audioTime));
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);

                //Change color
                Vector3 low = new Vector3(Mathf.Sin(t), Mathf.Cos(t)) * 2f;
                Vector3 high = new Vector3(Mathf.Sin(t), Mathf.Cos(t)) * 8f;
                Color color = Color.HSVToRGB(InverseLerp(high, low, cubes[i].transform.position) * 0.15f, 1f, 1f);
                cubes[i].GetComponent<Renderer>().material.color = color;

                // if (i < 10)
                // {
                //     float bassAmp = 2 * Mathf.Log(Spectrum.bassAmp * scale * scale, 10) / 3;
                //     centerCubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                //     centerCubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.17f, Mathf.InverseLerp(1, 2.5f, amp), 1f);
                //     //centerCubes[i].transform.Rotate(Spectrum.bassAmp, 0f, Mathf.Sin(Spectrum.bassAmp) * Spectrum.bassAmp * scale * scale);
                // }
            }

            else if (currentTime < timeFlags[4] || currentTime < timeFlags[6])    //Segment 2 & 4
            {
                if (segment3)
                {
                    if (i < 300)
                    {
                        backBars[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        backBars[i].transform.position = new Vector3(scale * (t - Mathf.PI / 2f), -scale, 27f);
                    }

                    if (i == numObjects - 1)
                    {
                        //mainCamera.GetComponent<Camera>().backgroundColor = Color.white;
                        Destroy(centerSphere);
                        segment3 = false;
                        segment2 = true;
                    }
                }

                float twopi = 2f * Mathf.PI;
                if (i < numObjects / 3)
                {
                    t = i * 6 * Mathf.PI / numObjects;
                    cubes[i].transform.position = new Vector3(t * Mathf.Sin(t + audioTime), t * Mathf.Cos(t + audioTime), 5f * (twopi - t - 1));
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 1f);
                }
                else if (i < numObjects * 2 / 3)
                {
                    t = i * 6 * Mathf.PI / numObjects - twopi;
                    cubes[i].transform.position = new Vector3(t * Mathf.Sin(t + (twopi / 3f) + audioTime), t * Mathf.Cos(t + (twopi / 3f) + audioTime), 5f * (twopi - t - 1));
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 1f);
                }
                else
                {
                    t = i * 6 * Mathf.PI / numObjects - 2f * twopi;
                    cubes[i].transform.position = new Vector3(t * Mathf.Sin(t + (2f * twopi / 3f) + audioTime), t * Mathf.Cos(t + (2f * twopi / 3f) + audioTime), 5f * (twopi - t - 1));
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 1f);
                }
                t = i * 2 * Mathf.PI / numObjects;
                //cubes[i].transform.position = startPositions[2, i];
                amp = Mathf.Max(scale / 2f * Mathf.Log(amp, 2) * Random.Range(0.9f, 1.1f), 0);
                cubes[i].transform.localScale = Vector3.one * 0.3f;
                cubes[i].transform.rotation = Quaternion.Euler(0f, 0f, i * 3f / numObjects * -360f);

                if (i < 300)
                {
                    backBars[i].transform.position = new Vector3(84f / Mathf.PI * (t - Mathf.PI / 2f), -20f, 27f) + Vector3.up * (amp * 0.5f);
                    backBars[i].transform.localScale = new Vector3(0.3f, amp, 0.3f);
                    backBars[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.75f, Mathf.InverseLerp(40f, 0f, amp), 1f);
                }
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
        else if ((currentTime > timeFlags[2] && currentTime < timeFlags[3]) || (currentTime > timeFlags[4] && currentTime < timeFlags[5]))
        {
            float amp = 2 * Mathf.Log(Spectrum.bassAmp * scale * scale, 10) / 3;
            centerSphere.transform.localScale = Vector3.one * amp;
            centerSphere.GetComponent<Renderer>().material.color = Color.HSVToRGB(0.17f, Mathf.InverseLerp(1, 2.5f, amp), 1f);
        }

    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ab = b - a;
        Vector3 av = value - a;
        return Mathf.Clamp(Vector3.Dot(av, ab) / Vector3.Dot(ab, ab), 0, 1);
    }
}
