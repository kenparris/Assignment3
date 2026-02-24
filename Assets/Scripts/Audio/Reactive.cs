using System.Collections;
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
    public Material gradient;
    int segmentCount;
    Vector3[,] startPositions;
    Vector3[,] endPositions;
    Vector3[] barStart;
    Vector3 pianoLeftStart, pianoLeftEnd, pianoRightStart, pianoRightEnd, pianoCenterStart, pianoCenterEnd;
    float[] timeFlags;
    float audioTime = 0f;
    float transitionTime = 0f;
    float pianoT1 = 0, pianoT2 = 0, pianoT3 = 0, pianoT4 = 0, pianoT5 = 0, pianoT6 = 0, pianoT7 = 0, pianoT8 = 0, pianoT9 = 0, pianoT10 = 0;
    bool transition1 = true;
    bool segment2 = true, segment3 = true, segment5 = true, p1 = true, p2 = true, p3 = true, p4 = true, p5 = true, p6 = true, p7 = true, p8 = true, p9 = true, p10 = true;
    GameObject centerSphere;
    //GameObject[] centerCubes;
    GameObject[] backBars;
    GameObject[] pianoTiles;


    void Start()

    {
        //                        0   t1   1    t2   2    t3     3      t4    4
        timeFlags = new float[] { 0f, 11.5f, 12.5f, 49.5f, 50.5f, 74.5f, 75.5f, 112f, 113f, 137f, 138f, 161.5f };

        segmentCount = timeFlags.Length - 1;

        cubes = new GameObject[numObjects];
        //centerCubes = new GameObject[10];
        backBars = new GameObject[300];

        startPositions = new Vector3[segmentCount, numObjects];

        endPositions = new Vector3[segmentCount, numObjects];
        barStart = new Vector3[300];

        pianoTiles = new GameObject[12];
        pianoLeftEnd = new Vector3(-9, -4, 0);
        pianoLeftStart = new Vector3(2f * 5 - 9f, 9f * 5 - 4f, 10f * 5);
        pianoRightEnd = new Vector3(9, -4, 0);
        pianoRightStart = new Vector3(-2f * 5 + 9f, 9f * 5 - 4f, 10f * 5);
        pianoCenterEnd = new Vector3(0, -4, 0);
        pianoCenterStart = new Vector3(0, 9f * 5 - 4f, 10f * 5);

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
            if (i < numObjects / 2)
            {
                startPositions[3, i] = new Vector3(2f * t - 12, 9f * t - 4f, 10f * t);
            }
            else
            {
                float t2 = t - Mathf.PI;
                startPositions[3, i] = new Vector3(-2f * t2 + 12, 9f * t2 - 4f, 10f * t2);
            }
            endPositions[3, i] = Vector3.one;

            // Segment 6 : 2:41s -> 2:55s

            startPositions[4, i] = Vector3.one;

            endPositions[4, i] = Vector3.one;

            if (i < 300)
            {
                barStart[i] = new Vector3(20f * (t - Mathf.PI / 2f), -20f, 27f);
            }

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
            Color color = Color.HSVToRGB(hue, (float)i / numObjects / 2f + 0.5f, 3f); // Full saturation and brightness
            sphereRenderer.material.color = color;

        }
        GetComponent<AudioSource>().Play();

    }
    //                                 1   2    3    4     5     6       7    8      9  
    //                                           X   Y     Z             X    Y      Z
    //                                 0   t1    1   t2    2     t3      3    t4     4          5  6
    //  timeFlags = new float[] { 0f, 11f, 12f, 49f, 50f, 74.5f, 75.5f, 112f, 113f, 137f, 138f, 161f };
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

            //lerp transition 1
            else if (currentTime < timeFlags[2])
            {
                if (transition1)
                {

                    transition1 = false;
                }
                cubes[i].transform.position = Vector3.Lerp(startPositions[0, i], endPositions[0, i], transitionTime);
                cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
            }

            else if (currentTime < timeFlags[3] || (currentTime < timeFlags[7] && currentTime > timeFlags[6]))    //Segment 1 & 3
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
                        transitionTime = 0;
                        centerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        centerSphere.transform.position = new Vector3(0, 0, 5f);
                        segment2 = false;
                        segment3 = true;
                    }

                    if (currentTime > timeFlags[4] && i < 300)
                    {
                        Destroy(backBars[i]);
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
                Color color = Color.HSVToRGB(InverseLerp(high, low, cubes[i].transform.position) * 0.15f, 1f, 3f);
                cubes[i].GetComponent<Renderer>().material.color = color;

                // if (i < 10)
                // {
                //     float bassAmp = 2 * Mathf.Log(Spectrum.bassAmp * scale * scale, 10) / 3;
                //     centerCubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                //     centerCubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.17f, Mathf.InverseLerp(1, 2.5f, amp), 1f);
                //     //centerCubes[i].transform.Rotate(Spectrum.bassAmp, 0f, Mathf.Sin(Spectrum.bassAmp) * Spectrum.bassAmp * scale * scale);
                // }
            }

            //lerp transition 2
            else if (currentTime < timeFlags[4] || (currentTime < timeFlags[8] && currentTime > timeFlags[7]))
            {
                cubes[i].transform.position = Vector3.Lerp(startPositions[1, i], startPositions[2, i], transitionTime);
                cubes[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cubes[i].transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            else if (currentTime < timeFlags[5] || (currentTime < timeFlags[9] && currentTime > timeFlags[8]))    //Segment 2 & 4
            {
                if (segment3)
                {
                    if (i < 300)
                    {
                        backBars[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        backBars[i].transform.position = barStart[i];
                    }

                    if (i == numObjects - 1)
                    {
                        transitionTime = 0;
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
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 3f);
                }
                else if (i < numObjects * 2 / 3)
                {
                    t = i * 6 * Mathf.PI / numObjects - twopi;
                    cubes[i].transform.position = new Vector3(t * Mathf.Sin(t + (twopi / 3f) + audioTime), t * Mathf.Cos(t + (twopi / 3f) + audioTime), 5f * (twopi - t - 1));
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 3f);
                }
                else
                {
                    t = i * 6 * Mathf.PI / numObjects - 2f * twopi;
                    cubes[i].transform.position = new Vector3(t * Mathf.Sin(t + (2f * twopi / 3f) + audioTime), t * Mathf.Cos(t + (2f * twopi / 3f) + audioTime), 5f * (twopi - t - 1));
                    cubes[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.3f, t / twopi, 3f);
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
                    backBars[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.75f, Mathf.InverseLerp(0f, 40f, amp) * 0.5f + 0.5f, 3f);
                }
            }

            else if (currentTime < timeFlags[6])
            {
                cubes[i].transform.position = Vector3.Lerp(startPositions[2, i], startPositions[1, i], transitionTime);
                cubes[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cubes[i].transform.rotation = Quaternion.Euler(Vector3.zero);

                if (i < 300)
                {
                    backBars[i].transform.position = Vector3.Lerp(barStart[i], barStart[i] + Vector3.down * 50f, transitionTime);
                    amp = Mathf.Max(scale / 2f * Mathf.Log(amp, 2) * Random.Range(0.9f, 1.1f), 0);
                    backBars[i].transform.localScale = new Vector3(0.3f, amp, 0.3f);
                    backBars[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.75f, Mathf.InverseLerp(0f, 40f, amp) * 0.5f + 0.5f, 3f);
                }
            }

            else if (currentTime < timeFlags[10])
            {
                cubes[i].transform.position = Vector3.Lerp(startPositions[2, i], startPositions[3, i], transitionTime);
                cubes[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cubes[i].transform.rotation = Quaternion.Euler(Vector3.zero);

                if (i < 300)
                {
                    backBars[i].transform.position = Vector3.Lerp(barStart[i], barStart[i] + Vector3.down * 50f, transitionTime);
                    amp = Mathf.Max(scale / 2f * Mathf.Log(amp, 2) * Random.Range(0.9f, 1.1f), 0);
                    backBars[i].transform.localScale = new Vector3(0.3f, amp, 0.3f);
                    backBars[i].GetComponent<Renderer>().material.color = Color.HSVToRGB(0.75f, Mathf.InverseLerp(0f, 40f, amp) * 0.5f + 0.5f, 3f);
                }
            }

            else    //Segment 5
            {
                if (segment5)
                {
                    for (i = 0; i < 12; i++)
                    {
                        pianoTiles[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        pianoTiles[i].transform.localScale = new Vector3(2f, 0.2f, 3.5f);
                        Material mat = new(translucentMaterial)
                        {
                            color = Color.HSVToRGB(0f, 0f, 3f)
                        };
                        pianoTiles[i].GetComponent<Renderer>().material = mat;

                        //L: 0, 4, 9
                        //C: 1, 2, 5, 8, 10, 11
                        //R: 3, 6, 7 

                        if (i == 0 || i == 4 || i == 9)
                        {
                            pianoTiles[i].transform.position = pianoLeftStart;
                            pianoTiles[i].transform.LookAt(pianoLeftEnd);
                        }
                        else if (i == 3 || i == 6 || i == 7)
                        {
                            pianoTiles[i].transform.position = pianoRightStart;
                            pianoTiles[i].transform.LookAt(pianoRightEnd);
                        }
                        else
                        {
                            pianoTiles[i].transform.position = pianoCenterStart;
                            pianoTiles[i].transform.LookAt(pianoCenterEnd);
                        }
                    }
                    transitionTime = 0;
                    segment5 = false;
                }

                amp = 3 * Mathf.Log(amp, 2) * Random.Range(0.9f, 1.1f);

                if (currentTime < timeFlags[11])
                {
                    if (i < 300)
                    {
                        Color color = Color.HSVToRGB(0.5f, Mathf.InverseLerp(4f, 0f, amp) * 0.5f + 0.5f, 3f);

                        cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                        cubes[i * 2].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                        cubes[i].GetComponent<Renderer>().material.color = color;
                        cubes[i * 2].GetComponent<Renderer>().material.color = color;
                        
                    }
                }
                else                                          //Segment 6
                {
                    if (i < 300)
                    {
                        Color color = Color.HSVToRGB(Mathf.InverseLerp((int)(300 * Mathf.InverseLerp(1f, 4f, Spectrum.audioAmp)), 0, i) * 0.5f, Mathf.InverseLerp(5f, 0f, amp) * 0.5f + 0.5f, 3f);
                        cubes[i].GetComponent<Renderer>().material.color = color;
                        cubes[i * 2].GetComponent<Renderer>().material.color = color;
                        cubes[i].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                        cubes[i * 2].transform.localScale = new Vector3(0.2f, amp, 0.2f);
                    }

                    if (i >= 300)
                    {
                        amp = Mathf.Max(scale / 2f * Mathf.Log(amp, 2) * Random.Range(0.9f, 1.1f), 0);
                        backBars[i - 300].transform.position = new Vector3(84f / Mathf.PI * (((i - 300) * 2f * Mathf.PI / numObjects) - Mathf.PI / 2f), -20f, 27f) + Vector3.up * (amp * 0.5f);
                        backBars[i - 300].transform.localScale = new Vector3(0.3f, amp, 0.3f);
                        backBars[i - 300].GetComponent<Renderer>().material.color = Color.HSVToRGB(Mathf.InverseLerp((int)(300 * Mathf.InverseLerp(1f, 4f, Spectrum.audioAmp)), 0, i - 300) * 0.5f, Mathf.InverseLerp(1f, 4f, Spectrum.audioAmp) * 0.4f + 0.6f, 3f);
                    }
                }


            }
        }

        //all transitions
        if (currentTime < timeFlags[2] && currentTime > timeFlags[1] ||
            (currentTime < timeFlags[4] && currentTime > timeFlags[3]) ||
            (currentTime < timeFlags[8] && currentTime > timeFlags[7]) ||
            (currentTime < timeFlags[6] && currentTime > timeFlags[5]) ||
            (currentTime < timeFlags[10] && currentTime > timeFlags[9]))
        {
            transitionTime += Time.deltaTime;
        }
        else if ((currentTime > timeFlags[2] && currentTime < timeFlags[3]) || (currentTime > timeFlags[6] && currentTime < timeFlags[7])) //segment 1 & 3
        {
            float amp = 2 * Mathf.Log(Spectrum.bassAmp * scale * scale, 10) / 3;
            centerSphere.transform.localScale = Vector3.one * amp;
            centerSphere.GetComponent<Renderer>().material.color = Color.HSVToRGB(0.17f, Mathf.InverseLerp(1, 2.5f, amp), 3f);
        }
        else if (currentTime < timeFlags[11])
        {
            if (currentTime > 138f && currentTime < 141.5f)
            {
                pianoTiles[0].transform.position = Vector3.Lerp(pianoLeftStart, pianoLeftEnd, pianoT1 / 3.5f);
                pianoT1 += Time.deltaTime;

            }

            if (currentTime > 141.5f && currentTime < 144.5f)
            {
                if (p1)
                {
                    StartCoroutine(FadeOut(pianoTiles[0]));
                    p1 = false;
                }

                pianoTiles[1].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT2 / 3f);
                pianoT2 += Time.deltaTime;
            }

            if (currentTime > 144.5f && currentTime < 147.75f)
            {
                if (p2)
                {
                    StartCoroutine(FadeOut(pianoTiles[1]));
                    p2 = false;
                }
                pianoTiles[2].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT3 / 3.25f);
                pianoT3 += Time.deltaTime;
            }

            if (currentTime > 147.75f && currentTime < 150.15f)
            {
                if (p3)
                {
                    StartCoroutine(FadeOut(pianoTiles[2]));
                    p3 = false;
                }
                pianoTiles[3].transform.position = Vector3.Lerp(pianoRightStart, pianoRightEnd, pianoT4 / 2.4f);
                pianoT4 += Time.deltaTime;
            }

            if (currentTime > 148f && currentTime < 151.1f)
            {
                pianoTiles[4].transform.position = Vector3.Lerp(pianoLeftStart, pianoLeftEnd, pianoT5 / 3.1f);
                pianoTiles[5].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT5 / 3.1f);
                pianoTiles[6].transform.position = Vector3.Lerp(pianoRightStart, pianoRightEnd, pianoT5 / 3.1f);

                pianoT5 += Time.deltaTime;
            }

            if (currentTime > 150.15f && currentTime < 151f)
            {
                if (p4)
                {
                    StartCoroutine(FadeOut(pianoTiles[3]));
                    p4 = false;
                }
            }

            if (currentTime > 151.1f && currentTime < 153.5f)
            {
                if (p5)
                {
                    StartCoroutine(FadeOut(pianoTiles[4]));
                    StartCoroutine(FadeOut(pianoTiles[5]));
                    StartCoroutine(FadeOut(pianoTiles[6]));
                    p5 = false;
                }

                pianoTiles[7].transform.position = Vector3.Lerp(pianoRightStart, pianoRightEnd, pianoT6 / 2.4f);
                pianoT6 += Time.deltaTime;
            }

            if (currentTime > 151.65f && currentTime < 153.75f)
            {

                pianoTiles[8].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT9 / 2.1f);
                pianoT9 += Time.deltaTime;
            }

            if (currentTime > 152f && currentTime < 154.1f)
            {
                pianoTiles[9].transform.position = Vector3.Lerp(pianoLeftStart, pianoLeftEnd, pianoT10 / 2.1f);
                pianoT10 += Time.deltaTime;
            }

            if (currentTime > 153.5f && currentTime < 154.15f)
            {
                if (p6)
                {
                    StartCoroutine(FadeOut(pianoTiles[7]));
                    p6 = false;
                }
                if (p9 && currentTime > 152.75f)
                {
                    StartCoroutine(FadeOut(pianoTiles[8]));
                    p9 = false;
                }
            }

            if (currentTime > 154.1f && currentTime < 157.3f)
            {
                if (p10)
                {
                    StartCoroutine(FadeOut(pianoTiles[9]));
                    p10 = false;
                }

                pianoTiles[10].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT7 / 3.2f);
                pianoT7 += Time.deltaTime;
            }

            if (currentTime > 157.3f && currentTime < 160.2f)
            {
                if (p7)
                {
                    StartCoroutine(FadeOut(pianoTiles[10]));
                    p7 = false;
                }

                pianoTiles[11].transform.position = Vector3.Lerp(pianoCenterStart, pianoCenterEnd, pianoT8 / 2.9f);
                pianoT8 += Time.deltaTime;
            }

            if (currentTime > 160.2f)
            {
                if (p8)
                {
                    StartCoroutine(FadeOut(pianoTiles[11]));
                    p8 = false;
                }
            }
        }

    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ab = b - a;
        Vector3 av = value - a;
        return Mathf.Clamp(Vector3.Dot(av, ab) / Vector3.Dot(ab, ab), 0, 1);
    }

    private IEnumerator FadeOut(GameObject pianoTile)
    {
        Renderer renderer = pianoTile.GetComponent<Renderer>();
        float delay = 0.5f;
        float startTime = Time.time;

        Color startColor = renderer.material.color;
        Color endColor = renderer.material.color;
        Vector3 startScale = pianoTile.transform.localScale;
        Vector3 endScale = startScale * 1.5f;
        startColor.a = 0.75f;
        endColor.a = 0f;

        while (Time.time < startTime + delay)
        {
            float t = (Time.time - startTime) / delay;
            renderer.material.color = Color.Lerp(startColor, endColor, t);
            pianoTile.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        renderer.material.color = endColor;
        Destroy(pianoTile);
    }
}
