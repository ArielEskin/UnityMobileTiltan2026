using UnityEngine;
using System.Collections.Generic;

public class SegmentPopulator : MonoBehaviour
{
    public TowerSegmentData segmentData;
    public Transform faceTransform; // Assign the parent object holding the 6 spawn points
    public string ladderTag = "Ladder"; 

    // This array tracks where we spawned top ladders so the NEXT segment can read it
    public bool[] topLadderSlots = new bool[3];
    private List<GameObject> activeLadders = new List<GameObject>();
    void Awake()
    {
        // This completely bypasses the Unity Inspector bug!
        // It automatically searches the prefab for the exact object named "Face_0"
        if (faceTransform == null)
        {
            faceTransform = transform.Find("Face_0");
            
            if (faceTransform == null)
            {
                Debug.LogError("Could not find Face_0! Check the spelling in the Hierarchy.");
            }
        }
    }

    public void GenerateLadders(bool[] previousTopSlots)
    {
        // 1. Apply the material from the Scriptable Object
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && segmentData != null && segmentData.segmentMaterial != null)
        {
            renderer.material = segmentData.segmentMaterial;
        }

        // 2. Clean up old ladders if this segment is being reused from the pool
        foreach (GameObject ladder in activeLadders)
        {
            TaggedObjectPooler.Instance.ReturnObject(ladder, ladderTag);
        }
        activeLadders.Clear();

        // 3. Generate BOTTOM ladders to match the previous segment perfectly
        // Assuming children 0, 1, 2 are the bottom row spawn points
        for (int i = 0; i < 3; i++)
        {
            if (previousTopSlots[i] == true) 
            {
                SpawnLadder(i); 
            }
        }

        // 4. Generate TOP ladders randomly based on the Scriptable Object rules
        int spawnedTopLadders = 0;
        for (int i = 0; i < 3; i++) topLadderSlots[i] = false; // Reset the array

        while (spawnedTopLadders < segmentData.minTopLadders)
        {
            int randomSlot = Random.Range(0, 3);
            if (!topLadderSlots[randomSlot])
            {
                topLadderSlots[randomSlot] = true;
                // Assuming children 3, 4, 5 are the top row spawn points
                SpawnLadder(randomSlot + 3); 
                spawnedTopLadders++;
            }
        }
    }

    private void SpawnLadder(int childIndex)
    {
        if (childIndex < faceTransform.childCount)
        {
            Transform spawnPoint = faceTransform.GetChild(childIndex);
            
            // Call your TaggedObjectPooler Singleton instead of Instantiate
            GameObject ladder = TaggedObjectPooler.Instance.GetPooledObject(ladderTag);
            
            if (ladder != null)
            {
                ladder.transform.position = spawnPoint.position;
                ladder.transform.rotation = spawnPoint.rotation;
                ladder.transform.SetParent(spawnPoint);
                activeLadders.Add(ladder);
            }
        }
    }
}