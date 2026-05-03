using UnityEngine;
using System.Collections.Generic;

public class SimpleTowerGenerator : MonoBehaviour
{
    public Transform cameraTransform; 
    public float segmentHeight = 2.0f;
    public int drawDistance = 6;   
    public string segmentTag = "TowerSegment"; // Tag for the Object Pooler

    private List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnY;

    // Store the top ladder configuration of the very last segment spawned.
    // We start with all true so the very first segment spawns a full bottom row of ladders.
    private bool[] lastTopLadders = new bool[3] { true, true, true }; 

    void Start()
    {
        nextSpawnY = -(drawDistance * segmentHeight);
        for (int i = 0; i < drawDistance * 2; i++)
        {
            SpawnNewSegment();
        }
    }

    void Update()
    {
        if (cameraTransform.position.y + (drawDistance * segmentHeight) > nextSpawnY)
        {
            SpawnNewSegment();
        }
        
        float removalThreshold = cameraTransform.position.y - (drawDistance * segmentHeight);
        
        if (activeSegments.Count > 0 && activeSegments[0].transform.position.y < removalThreshold)
        {
            GameObject oldest = activeSegments[0];
            activeSegments.RemoveAt(0);
            
            // Return to the Singleton Tagged Pooler
            TaggedObjectPooler.Instance.ReturnObject(oldest, segmentTag); 
        }
    }

    void SpawnNewSegment()
    {
        // Get from the Singleton Tagged Pooler
        GameObject newSeg = TaggedObjectPooler.Instance.GetPooledObject(segmentTag);  
        
        if (newSeg != null)
        {
            newSeg.transform.position = new Vector3(transform.position.x, nextSpawnY, transform.position.z);
            activeSegments.Add(newSeg);
            nextSpawnY += segmentHeight;
            
            SegmentPopulator populator = newSeg.GetComponent<SegmentPopulator>();
            if (populator != null)            
            {
                // Pass the previous segment's ladders to ensure the path connects
                populator.GenerateLadders(lastTopLadders);  
                
                // Save this new segment's top ladders for the NEXT segment to read
                lastTopLadders = populator.topLadderSlots;
            }
        }
    }
}