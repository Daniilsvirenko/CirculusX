using UnityEngine;
using System.Collections.Generic;

public class RedLightsAnomaly : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Перетащите сюда объект lights.001. Если оставить пустым, скрипт попытается найти его по имени.")]
    [SerializeField] private Transform lightsParent;
    [SerializeField] private Color anomalyColor = Color.red;

    // Словарь для хранения оригинальных цветов каждого источника света
    private Dictionary<Light, Color> originalColors = new Dictionary<Light, Color>();
    private bool isInitialized = false;

    private void InitializeLights()
    {
        if (isInitialized) return;

        // Попытка найти объект lights.001 автоматически, если он не назначен в инспекторе
        if (lightsParent == null)
        {
            GameObject foundLights = GameObject.Find("lights.001");
            if (foundLights != null)
            {
                lightsParent = foundLights.transform;
            }
            else
            {
                Debug.LogWarning("[RedLightsAnomaly] Не удалось найти объект 'lights.001' на сцене! Назначьте его вручную.");
                return;
            }
        }

        // Собираем все компоненты Light внутри lights.001 (включая неактивные)
        Light[] allLights = lightsParent.GetComponentsInChildren<Light>(true);
        foreach (Light lightComponent in allLights)
        {
            // Нам нужны только Point Light
            if (lightComponent.type == LightType.Point)
            {
                originalColors[lightComponent] = lightComponent.color;
            }
        }

        isInitialized = true;
    }

    // Вызывается, когда GameManager выбирает эту аномалию и включает её
    private void OnEnable()
    {
        InitializeLights();

        // Меняем цвет всех сохранённых ламп на красный
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.color = anomalyColor;
            }
        }

        Debug.Log("[RedLightsAnomaly] Свет изменён на кроваво-красный!");
    }

    // Вызывается, когда игрок проходит этаж и GameManager выключает аномалию (Reset Phase)
    private void OnDisable()
    {
        // Возвращаем всем лампам их оригинальный цвет
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.color = kvp.Value;
            }
        }
    }
}
