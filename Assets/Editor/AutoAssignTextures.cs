using UnityEngine;
using UnityEditor;

public class AutoAssignTextures : EditorWindow
{
    [MenuItem("Tools/Auto Assign Textures to Materials")]
    public static void AssignTextures()
    {
        string texDir = "Assets/HotelHallway/tex";

        // Загрузим текстуры
        Texture2D carpetColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/carpet_color.png");
        Texture2D carpetNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Fabric_Carpet_1K_normal.jpg");

        Texture2D tableColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/table_1_color.jpg");
        Texture2D tableNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/table_1_normal.jpg");

        Texture2D wallColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Overlay_Abstract28_1K_overlay.jpg");
        Texture2D wallNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Wall_Stucco6A_2x2_1K_normal.jpg");

        Texture2D metalColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Metal_AluminumBrushed_1K_albedo.jpg");
        Texture2D metalNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Metal_AluminumBrushed_1K_normal.jpg");

        Texture2D doorWoodColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texDir}/TCom_Wood_MahoganyVeneer_1K_albedo_lite.jpg");

        // Ищем все материалы в папке tex
        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { texDir });
        
        int count = 0;

        foreach (string guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            string matName = mat.name.ToLower();
            bool changed = false;

            Undo.RecordObject(mat, "Auto Assign Textures");

            if (matName.Contains("carpet"))
            {
                if (carpetColor) mat.SetTexture("_BaseMap", carpetColor);
                if (carpetNormal) mat.SetTexture("_BumpMap", carpetNormal);
                mat.SetFloat("_BumpScale", 1.0f);
                mat.SetColor("_BaseColor", Color.white);
                changed = true;
            }
            else if (matName.Contains("table"))
            {
                if (tableColor) mat.SetTexture("_BaseMap", tableColor);
                if (tableNormal) mat.SetTexture("_BumpMap", tableNormal);
                mat.SetFloat("_BumpScale", 1.0f);
                mat.SetColor("_BaseColor", Color.white);
                changed = true;
            }
            else if (matName.Contains("wall"))
            {
                if (wallColor) mat.SetTexture("_BaseMap", wallColor);
                if (wallNormal) mat.SetTexture("_BumpMap", wallNormal);
                mat.SetFloat("_BumpScale", 1.0f);
                mat.SetColor("_BaseColor", Color.white);
                changed = true;
            }
            else if (matName.Contains("metal") || matName.Contains("door_frame") || matName.Contains("lamp_base"))
            {
                if (metalColor) mat.SetTexture("_BaseMap", metalColor);
                if (metalNormal) mat.SetTexture("_BumpMap", metalNormal);
                mat.SetFloat("_BumpScale", 1.0f);
                mat.SetColor("_BaseColor", Color.white);
                changed = true;
            }
            else if (matName.Contains("door_wood"))
            {
                if (doorWoodColor) mat.SetTexture("_BaseMap", doorWoodColor);
                mat.SetColor("_BaseColor", Color.white);
                changed = true;
            }
            else if (matName.Contains("light_cover") || matName.Contains("lamp_light"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Color(1f, 0.95f, 0.8f) * 2.5f);
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(mat);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Успех", $"Все текстуры успешно обновлены!\nЗатронуто материалов: {count}", "ОК");
    }
}
