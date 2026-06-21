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

    [Header("Level 0 - Delusional Corridor (Ending)")]
    [Tooltip("Parent object containing the normal repeating hallway (hallway_hotel3 etc.). Gets disabled on Floor 0.")]
    public GameObject mainHallway;
    [Tooltip("Parent object containing the Delusional Corridor ending area. Stays disabled until Floor 0.")]
    public GameObject level0Corridor;
    [Tooltip("Spawn point used only inside the Delusional Corridor.")]
    public Transform level0SpawnPoint;
    [Tooltip("Controller that handles the fade-to-white and 'THE END' text.")]
    public EndingUIController endingUI;

    private bool isInLevel0 = false;

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
        // Inside the Delusional Corridor the only elevator interaction is "give up"
        // and give up always means: hard reset back to Floor 10.
        if (isInLevel0)
        {
            GiveUpFromLevel0();
            return;
        }

        if (guessedAnomaly == isAnomalyPresentOnCurrentFloor)
        {
            currentFloor--;
            Debug.Log($"Correct! Moving down to floor {currentFloor}.");

            if (currentFloor <= 0)
            {
                EnterLevel0();
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
        ResetAllElevatorDoors();
    }

    // Called once the player correctly escapes Floor 1. Swaps the normal hallway
    // for the Delusional Corridor ending area instead of generating another floor.
    private void EnterLevel0()
    {
        isInLevel0 = true;
        isAnomalyPresentOnCurrentFloor = false;

        Debug.Log("Entering Floor 0 - Delusional Corridor.");

        if (floorDisplayText != null)
        {
            floorDisplayText.text = "FLOOR 0";
        }

        if (mainHallway != null) mainHallway.SetActive(false);
        if (level0Corridor != null) level0Corridor.SetActive(true);

        TeleportPlayerTo(level0SpawnPoint);

        // Reset elevator doors so the Level 0 elevator behaves like a fresh start
        ElevatorDoorController[] allDoors = FindObjectsOfType<ElevatorDoorController>();
        foreach (ElevatorDoorController door in allDoors)
        {
            door.ResetDoors();
        }
    }

    // Player chose the exit door inside the Delusional Corridor -> real win ending
    public void TriggerWinEnding()
    {
        if (!isInLevel0) return;

        Debug.Log("Player escaped the loop. Triggering win ending.");

        if (endingUI != null)
        {
            endingUI.PlayEnding();
        }
    }

    // Player chose to go back to the start elevator inside the Delusional Corridor.
    // This is a full reset, identical in spirit to a wrong decision.
    private void GiveUpFromLevel0()
    {
        Debug.Log("Player gave up. Resetting back to Floor 10.");

        isInLevel0 = false;
        currentFloor = 10;
        currentDynamicProbability = anomalyProbability;
        lastActiveAnomalyObject = null;

        if (level0Corridor != null) level0Corridor.SetActive(false);
        if (mainHallway != null) mainHallway.SetActive(true);

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
        TeleportPlayerTo(spawnPoint);
    }

    private void TeleportPlayerTo(Transform target)
    {
        if (target == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = target.position;
        player.rotation = target.rotation;

        if (cc != null) cc.enabled = true;
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
        if (level0Corridor != null) level0Corridor.SetActive(false);

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

    private void ResetAllElevatorDoors()
    {
        ElevatorDoorController[] allDoors = FindObjectsOfType<ElevatorDoorController>();
        foreach (ElevatorDoorController door in allDoors)
        {
            door.ResetDoors();
        }
    }

    // Fully resets the game back to its initial state (Floor 10, normal hallway).
    // Call this when returning to the main menu after winning, so a fresh
    // "Start" press begins a clean run instead of continuing from Floor 0.
    public void ResetGameState()
    {
        isInLevel0 = false;
        currentFloor = 10;
        currentDynamicProbability = anomalyProbability;
        lastActiveAnomalyObject = null;
        currentActiveAnomalyObject = null;

        if (level0Corridor != null) level0Corridor.SetActive(false);
        if (mainHallway != null) mainHallway.SetActive(true);

        UpdateFloorDisplay();
        GenerateFloorState();
        TeleportPlayerToStart();
        ResetAllElevatorDoors();

        // Re-enable movement, since the ending sequence locks it
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.SetInputLocked(false);
        }
    }
}
