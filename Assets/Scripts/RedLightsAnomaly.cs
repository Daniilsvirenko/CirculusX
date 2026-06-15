using UnityEngine;
using System.Collections.Generic;

public class RedLightsAnomaly : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag the lights.001 object here. If left empty, the script will try to find it by name.")]
    [SerializeField] private Transform lightsParent;
    [SerializeField] private Color anomalyColor = Color.red;

    // Dictionary to store the original colors of each light source
    private Dictionary<Light, Color> originalColors = new Dictionary<Light, Color>();
    private bool isInitialized = false;
    private bool hasTriggered = false; // Флаг, чтобы свет не менялся дважды

    private void InitializeLights()
    {
        if (isInitialized) return;

        // Attempt to find the lights.001 object automatically if it is not assigned in the inspector
        if (lightsParent == null)
        {
            GameObject foundLights = GameObject.Find("lights.001");
            if (foundLights != null)
            {
                lightsParent = foundLights.transform;
            }
            else
            {
                Debug.LogWarning("[RedLightsAnomaly] Could not find the 'lights.001' object in the scene! Assign it manually.");
                return;
            }
        }

        // Collect all Light components inside lights.001 (including inactive ones)
        Light[] allLights = lightsParent.GetComponentsInChildren<Light>(true);
        foreach (Light lightComponent in allLights)
        {
            // We only need Point Lights
            if (lightComponent.type == LightType.Point)
            {
                originalColors[lightComponent] = lightComponent.color;
            }
        }

        isInitialized = true;
    }

    // Called when the GameManager selects this anomaly and enables it
    private void OnEnable()
    {
        InitializeLights();
        hasTriggered = false; // Сбрасываем триггер
        // Свет больше не меняется мгновенно! Ждем OnTriggerEnter
    }

    // Вызывается, когда объект с коллайдером-триггером пересекается с другим объектом
    private void OnTriggerEnter(Collider other)
    {
        // Если уже сработало, игнорируем
        if (hasTriggered) return;

        // Проверяем, что в триггер вошел именно Игрок (проверь, чтобы у игрока был тег "Player")
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Меняем цвета всех ламп на красный
            foreach (var kvp in originalColors)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.color = anomalyColor;
                }
            }

            Debug.Log("[RedLightsAnomaly] Player entered trigger! Light changed to blood red!");
        }
    }

    // Called when the player passes the floor and GameManager disables the anomaly (Reset Phase)
    private void OnDisable()
    {
        // Return all lights to their original color
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.color = kvp.Value;
            }
        }
        
        hasTriggered = false;
    }
}

