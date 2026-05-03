using UnityEngine;

[CreateAssetMenu(fileName = "NewSegmentData", menuName = "Tower/Segment Data")]
public class TowerSegmentData : ScriptableObject
{
    public Material segmentMaterial;
    public int minTopLadders = 1;
}