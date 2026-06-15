using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentFloor = 10;
    public bool isAnomalyPresentOnCurrentFloor = false;
    [Range(0f, 1f)] public float anomalyProbability = 0.5f;
    [Tooltip("How much the probability increases after a normal floor (e.g., 0.15 = +15%)")]
    [Range(0f, 1f)] public float probabilityIncreasePerNormalFloor = 0.15f;

    private float currentDynamicProbability;

    [Header("Player & References")]
    public Transform player;
    public Transform spawnPoint;
    public TextMeshProUGUI floorDisplayText;

    // List to hold all potential anomaly objects
    private List<GameObject> anomalyObjects = new List<GameObject>();
    private GameObject currentActiveAnomalyObject = null;
    private GameObject lastActiveAnomalyObject = null; // Запоминаем последнюю аномалию

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentDynamicProbability = anomalyProbability;
        UpdateFloorDisplay();
    }

    // Objects call this when the game starts to register themselves
    public void RegisterAnomalyObject(GameObject obj)
    {
        if (!anomalyObjects.Contains(obj))
        {
            anomalyObjects.Add(obj);
        }
    }

    public void MakeDecision(bool guessedAnomaly)
    {
        if (guessedAnomaly == isAnomalyPresentOnCurrentFloor)
        {
            currentFloor--;
            Debug.Log($"Correct! Moving down to floor {currentFloor}.");

            if (currentFloor <= 0)
            {
                Debug.Log("YOU ESCAPED! (Win Screen)");
                return;
            }
        }
        else
        {
            currentFloor = 10;
            currentDynamicProbability = anomalyProbability; // Reset probability on failure
            lastActiveAnomalyObject = null; // Сбрасываем историю при проигрыше
            Debug.Log("Wrong decision! Resetting back to Floor 10.");
        }

        UpdateFloorDisplay();
        GenerateFloorState();
        TeleportPlayerToStart();
    }

    // Logic to create an anomaly
    private void GenerateFloorState()
    {
        // Запоминаем последнюю фактическую аномалию (игнорируем нормальные этажи без аномалий)
        if (currentActiveAnomalyObject != null)
        {
            lastActiveAnomalyObject = currentActiveAnomalyObject;
        }

        // 1. RESET PHASE: Bring everything back to its natural "Normal Floor" state
        foreach (GameObject obj in anomalyObjects)
        {
            Anomaly anomalyScript = obj.GetComponent<Anomaly>();
            if (anomalyScript != null)
            {
                // If its anomaly state is to HIDE, its normal state is ACTIVE (true)
                // If its anomaly state is to SHOW, its normal state is INACTIVE (false)
                if (anomalyScript.behavior == Anomaly.AnomalyBehavior.HideWhenAnomalyPresent)
                {
                    obj.SetActive(true);
                }
                else if (anomalyScript.behavior == Anomaly.AnomalyBehavior.ShowWhenAnomalyPresent)
                {
                    obj.SetActive(false);
                }
            }
        }

        currentActiveAnomalyObject = null;

        // Decide if this floor has an anomaly (Never on floor 10)
        if (currentFloor == 10)
        {
            isAnomalyPresentOnCurrentFloor = false;
        }
        else
        {
            isAnomalyPresentOnCurrentFloor = (Random.value <= currentDynamicProbability);
        }

        Debug.Log($"\n--- GENERATING FLOOR {currentFloor} ---");

        // 2. ANOMALY PHASE: If an anomaly should exist, pick one and apply its specific rule
        if (isAnomalyPresentOnCurrentFloor && anomalyObjects.Count > 0)
        {
            // Filter anomalies based on the current floor's difficulty rule
            List<GameObject> validAnomalies = new List<GameObject>();
            foreach (GameObject obj in anomalyObjects)
            {
                // ИСКЛЮЧАЕМ аномалию, которая была на предыдущем этаже
                if (obj == lastActiveAnomalyObject)
                {
                    continue; // Пропускаем её
                }

                Anomaly anomalyScript = obj.GetComponent<Anomaly>();
                if (anomalyScript != null)
                {
                    if ((currentFloor >= 7 && currentFloor <= 9) && anomalyScript.type == Anomaly.AnomalyType.Simple)
                    {
                        validAnomalies.Add(obj);
                    }
                    else if ((currentFloor >= 4 && currentFloor <= 6) && anomalyScript.type == Anomaly.AnomalyType.Difficult)
                    {
                        validAnomalies.Add(obj);
                    }
                    else if ((currentFloor >= 1 && currentFloor <= 3) && anomalyScript.type == Anomaly.AnomalyType.Disturbing)
                    {
                        validAnomalies.Add(obj);
                    }
                }
            }

            if (validAnomalies.Count > 0)
            {
                // Anomaly spawned! Reset probability back to base
                currentDynamicProbability = anomalyProbability;

                int randomIndex = Random.Range(0, validAnomalies.Count);
                currentActiveAnomalyObject = validAnomalies[randomIndex];

                Anomaly anomalyScript = currentActiveAnomalyObject.GetComponent<Anomaly>();

                if (anomalyScript != null)
                {
                    if (anomalyScript.behavior == Anomaly.AnomalyBehavior.HideWhenAnomalyPresent)
                    {
                        // Example: A normal ceiling light disappears
                        currentActiveAnomalyObject.SetActive(false);
                        Debug.Log($"[FLOOR {currentFloor}] ANOMALY: Object '{currentActiveAnomalyObject.name}' disappeared (Difficulty: {anomalyScript.type})");
                    }
                    else if (anomalyScript.behavior == Anomaly.AnomalyBehavior.ShowWhenAnomalyPresent)
                    {
                        // Example: Footprints suddenly appear on the ground
                        currentActiveAnomalyObject.SetActive(true);
                        Debug.Log($"[FLOOR {currentFloor}] ANOMALY: Object '{currentActiveAnomalyObject.name}' appeared (Difficulty: {anomalyScript.type})");
                    }
                }
            }
            else
            {
                isAnomalyPresentOnCurrentFloor = false;
                currentDynamicProbability = Mathf.Clamp01(currentDynamicProbability + probabilityIncreasePerNormalFloor);
                Debug.Log($"[FLOOR {currentFloor}] NORMAL: No valid anomalies for this difficulty level. Next chance: {currentDynamicProbability * 100}%");
            }
        }
        else
        {
            if (currentFloor != 10)
            {
                currentDynamicProbability = Mathf.Clamp01(currentDynamicProbability + probabilityIncreasePerNormalFloor);
            }
            Debug.Log($"[FLOOR {currentFloor}] NORMAL: No anomaly generated (lucky). Next chance: {currentDynamicProbability * 100}%");
        }
    }

    private void TeleportPlayerToStart()
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // Reset all elevator doors so they act like the beginning of the game
        ElevatorDoorController[] allDoors = FindObjectsOfType<ElevatorDoorController>();
        foreach (ElevatorDoorController door in allDoors)
        {
            door.ResetDoors();
        }
    }

    // Updates the TMPro text on the elevator wall
    private void UpdateFloorDisplay()
    {
        if (floorDisplayText != null)
        {
            // Displays floor number using Roman numerals
            floorDisplayText.text = "FLOOR " + ConvertToRoman(currentFloor);
        }
    }

    // Helper method to convert floor numbers (1-10) to Roman numerals
    private string ConvertToRoman(int number)
    {
        switch (number)
        {
            case 10: return "X";
            case 9: return "IX";
            case 8: return "VIII";
            case 7: return "VII";
            case 6: return "VI";
            case 5: return "V";
            case 4: return "IV";
            case 3: return "III";
            case 2: return "II";
            case 1: return "I";
            default: return number.ToString();
        }
    }

    // Initialize the first floor state when the game kicks off
    private void Start()
    {
        // Find the AnomalyManager in the scene
        AnomalyManager anomalyManager = FindObjectOfType<AnomalyManager>();

        if (anomalyManager != null && anomalyManager.allAnomalies != null)
        {
            foreach (GameObject anomalyObj in anomalyManager.allAnomalies)
            {
                // Automatically register every anomaly from the manager's list
                RegisterAnomalyObject(anomalyObj);
            }
        }

        // Generate the floor state
        GenerateFloorState();
    }
}