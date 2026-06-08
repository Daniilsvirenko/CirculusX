using UnityEngine;
using UnityEditor;

public class AutoAssignTextures : EditorWindow
{
    [MenuItem("Tools/Auto Assign Textures to Materials")]
    public static void AssignTextures()
    {
        string materialsDir = "Assets/Materiials";
        string texturesDir = "Assets/HotelHallway/tex";

        // Загрузим текстуры
        Texture2D carpetColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/carpet_color.png");
        Texture2D carpetNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Fabric_Carpet_1K_normal.jpg");

        Texture2D tableColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/table_1_color.jpg");
        Texture2D tableNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/table_1_normal.jpg");

        Texture2D wallColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Overlay_Abstract28_1K_overlay.jpg");
        Texture2D wallNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Wall_Stucco6A_2x2_1K_normal.jpg");

        Texture2D metalLightsColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Metal_AluminumBrushed_1K_albedo.jpg");
        Texture2D metalLightsNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Metal_AluminumBrushed_1K_normal.jpg");

        Texture2D doorWoodColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesDir}/TCom_Wood_MahoganyVeneer_1K_albedo_lite.jpg");

        // Загрузим материалы
        Material carpetMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/carpet.mat");
        Material tablesMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/tables.mat");
        Material wallsMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/walls.mat");
        Material metalLightsMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/metal_lights.mat");
        Material doorWoodMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/door_wood.mat");
        Material lightCoverMat = AssetDatabase.LoadAssetAtPath<Material>($"{materialsDir}/light_cover.mat");

        // Применяем текстуры к ковру
        if (carpetMat != null)
        {
            Undo.RecordObject(carpetMat, "Assign Carpet Textures");
            carpetMat.SetTexture("_BaseMap", carpetColor);
            carpetMat.SetTexture("_BumpMap", carpetNormal);
            carpetMat.SetFloat("_BumpScale", 1.0f);
            carpetMat.SetColor("_BaseColor", Color.white); // Сбрасываем зеленый тинт
            EditorUtility.SetDirty(carpetMat);
        }

        // Применяем текстуры к столам
        if (tablesMat != null)
        {
            Undo.RecordObject(tablesMat, "Assign Table Textures");
            tablesMat.SetTexture("_BaseMap", tableColor);
            tablesMat.SetTexture("_BumpMap", tableNormal);
            tablesMat.SetFloat("_BumpScale", 1.0f);
            tablesMat.SetColor("_BaseColor", Color.white); // Сбрасываем коричневый тинт
            EditorUtility.SetDirty(tablesMat);
        }

        // Применяем текстуры к стенам
        if (wallsMat != null)
        {
            Undo.RecordObject(wallsMat, "Assign Wall Textures");
            wallsMat.SetTexture("_BaseMap", wallColor);
            wallsMat.SetTexture("_BumpMap", wallNormal);
            wallsMat.SetFloat("_BumpScale", 1.0f);
            wallsMat.SetColor("_BaseColor", Color.white); // Сбрасываем бежевый тинт
            EditorUtility.SetDirty(wallsMat);
        }

        // Применяем текстуры к корпусам ламп
        if (metalLightsMat != null)
        {
            Undo.RecordObject(metalLightsMat, "Assign Metal Lights Textures");
            metalLightsMat.SetTexture("_BaseMap", metalLightsColor);
            metalLightsMat.SetTexture("_BumpMap", metalLightsNormal);
            metalLightsMat.SetFloat("_BumpScale", 1.0f);
            metalLightsMat.SetColor("_BaseColor", Color.white); // Сбрасываем голубой тинт
            EditorUtility.SetDirty(metalLightsMat);
        }

        // Применяем текстуру дерева к дверям
        if (doorWoodMat != null)
        {
            Undo.RecordObject(doorWoodMat, "Assign Door Wood Textures");
            doorWoodMat.SetTexture("_BaseMap", doorWoodColor);
            doorWoodMat.SetColor("_BaseColor", Color.white); // Сбрасываем оранжевый тинт
            EditorUtility.SetDirty(doorWoodMat);
        }

        // Настраиваем свечение плафонов
        if (lightCoverMat != null)
        {
            Undo.RecordObject(lightCoverMat, "Assign Light Cover Emission");
            lightCoverMat.EnableKeyword("_EMISSION");
            lightCoverMat.SetColor("_EmissionColor", new Color(1f, 0.95f, 0.8f) * 2.5f); // Теплый HDR свет
            EditorUtility.SetDirty(lightCoverMat);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Успех", "Все текстуры успешно привязаны к материалам, а цветные маски сброшены!", "ОК");
    }
}
