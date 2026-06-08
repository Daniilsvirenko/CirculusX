using UnityEngine;
using UnityEditor;

public class AutoPlaceLights : EditorWindow
{
    [MenuItem("Tools/Place Lights on Fixtures")]
    public static void PlaceLights()
    {
        GameObject parentObj = Selection.activeGameObject;
        if (parentObj == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, выделите родительский объект 'lights' в Иерархии (Hierarchy) перед запуском!", "ОК");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(parentObj, "Place Lights on Fixtures");

        int count = 0;
        foreach (Transform child in parentObj.transform)
        {
            // Проверяем, нет ли уже источника света в дочерних объектах
            Light existingLight = child.GetComponentInChildren<Light>();
            if (existingLight == null)
            {
                GameObject lightGo = new GameObject("Point Light");
                lightGo.transform.SetParent(child);
                lightGo.transform.localPosition = new Vector3(0f, 0.033f, 0.106f); // Позиционируем по заданным координатам пользователя
                lightGo.transform.localRotation = Quaternion.identity;
                lightGo.transform.localScale = Vector3.one;

                Light light = lightGo.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(1f, 0.93f, 0.75f); // Красивый теплый свет
                light.intensity = 1.2f;
                light.range = 8f;
                
                // Включаем мягкие тени
                light.shadows = LightShadows.Soft;

                count++;
            }
        }

        EditorUtility.DisplayDialog("Успех", $"Успешно добавлено {count} источников света Point Light!", "ОК");
    }
}
