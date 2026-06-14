using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    [Header("List of All Anomalies in the Game")]
    public List<GameObject> allAnomalies;

    // Note: The actual spawning logic is handled by GameManager.GenerateFloorState().
    // This script now primarily serves as a centralized list (allAnomalies) 
    // that the GameManager reads on Start() to register anomalies.
}