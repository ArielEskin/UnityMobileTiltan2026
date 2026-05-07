using UnityEngine;
using System.Collections.Generic;

public class SegmentPopulator : MonoBehaviour
{
    public TowerSegmentData segmentData;
    public Transform faceTransform; 
    public string ladderTag = "Ladder"; 
    public bool[] topLadderSlots = new bool[3];
    private List<GameObject> activeLadders = new List<GameObject>();
    
    void Awake()
    {
        if (faceTransform == null) { faceTransform = transform.Find("Face_0"); }
    }

    public void GenerateLadders(bool[] previousTopSlots)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && segmentData != null && segmentData.segmentMaterial != null)
        {
            renderer.material = segmentData.segmentMaterial;
        }
        foreach (GameObject ladder in activeLadders)
        {
            TaggedObjectPooler.Instance.ReturnObject(ladder, ladderTag);
        }
        activeLadders.Clear();
        for (int i = 0; i < 3; i++)
        {
            if (previousTopSlots[i] == true) 
            {
                SpawnLadder(i); 
            }
        }

        int spawnedTopLadders = 0;
        for (int i = 0; i < 3; i++) topLadderSlots[i] = false; 

        while (spawnedTopLadders < segmentData.minTopLadders)
        {
            int randomSlot = Random.Range(0, 3);
            if (!topLadderSlots[randomSlot])
            {
                topLadderSlots[randomSlot] = true;
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