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
            Transform existingLightTransform = child.Find("Point Light");
            Light light;

            if (existingLightTransform == null)
            {
                GameObject lightGo = new GameObject("Point Light");
                lightGo.transform.SetParent(child);
                existingLightTransform = lightGo.transform;
            }

            existingLightTransform.localPosition = new Vector3(0f, 0.033f, 0.106f);
            existingLightTransform.localRotation = Quaternion.identity;
            existingLightTransform.localScale = Vector3.one;

            light = existingLightTransform.GetComponent<Light>();
            if (light == null)
            {
                light = existingLightTransform.gameObject.AddComponent<Light>();
            }

            light.type = LightType.Point;
            light.lightmapBakeType = LightmapBakeType.Realtime;
            
            ColorUtility.TryParseHtmlString("#FFEEDB", out Color lightColor);
            light.color = lightColor;
            
            light.intensity = 1.2f;
            light.bounceIntensity = 1f;
            light.range = 1f;

            count++;
        }

        EditorUtility.DisplayDialog("Успех", $"Успешно добавлено/обновлено {count} источников света Point Light!", "ОК");
    }
}
