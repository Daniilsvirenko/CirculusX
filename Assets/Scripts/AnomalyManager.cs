using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    // Assets
    public List<GameObject> allAnomalies;

    public void SpawnAnomalyForFloor(int currentFloor)
    {
        List<GameObject> validAnomalies = new List<GameObject>();

        foreach (GameObject anomalyObj in allAnomalies)
        {
            Anomaly anomalyScript = anomalyObj.GetComponent<Anomaly>();

            if (anomalyScript != null)
            {
                if ((currentFloor <= 9 && currentFloor >= 7) && anomalyScript.type == Anomaly.AnomalyType.Simple)
                {
                    validAnomalies.Add(anomalyObj);
                }
                else if ((currentFloor <= 6 && currentFloor >= 4) && anomalyScript.type == Anomaly.AnomalyType.Difficult)
                {
                    validAnomalies.Add(anomalyObj);
                }
                else if ((currentFloor <= 3 && currentFloor >= 1) && anomalyScript.type == Anomaly.AnomalyType.Disturbing)
                {
                    validAnomalies.Add(anomalyObj);
                }
            }
        }

        // Select by chance
        if (validAnomalies.Count > 0)
        {
            int randomIndex = Random.Range(0, validAnomalies.Count);
            validAnomalies[randomIndex].SetActive(true);
            Debug.Log("Spawned Anomaly: " + validAnomalies[randomIndex].name);
        }
    }
}