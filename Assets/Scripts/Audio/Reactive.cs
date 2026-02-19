using UnityEngine;

public class Reactive : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]
    // Row = time segment (0-10, 10-20, 20-30)
    // Col = sphere id
    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment

    GameObject[] spheres;
    static int numSphere = 200;
    int segmentCount;
    Vector3[,] startPositions;

    Vector3[,] endPositions;
    float[] timeFlags;
    

    void Start()

    {

        timeFlags = new float[] { 0f, 11f, 50f, 75f, 114f, 138f, 161f };

        segmentCount = timeFlags.Length - 1;

        spheres = new GameObject[numSphere];

        startPositions = new Vector3[segmentCount, numSphere];

        endPositions = new Vector3[segmentCount, numSphere];


        for (int i = 0; i < numSphere; i++)

        {

            // Segment 0 : 0s -> 11s

            startPositions[0, i] = Vector3.one;

            endPositions[0, i] = Vector3.one;


            // Segment 1 & 3: 11s -> 50s     1:15s -> 1:54s

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


}

    
	  void Update()

    {
        float currentTime = Time.time;

        if (currentTime < timeFlags[1])

        {

				

        }

        else if (currentTime < timeFlags[2])

        {

        }

        else if (currentTime < timeFlags[3])

        {

        }

        else

        {

 }

  }
}
