
#if UNITY_EDITOR  
using UnityEngine;  
using UnityEditor;  

[CustomEditor(typeof(LevelPath))]  
public class LevelPathEditor : Editor  
{  
    public override void OnInspectorGUI()  
    {  
        serializedObject.Update();
        DrawDefaultInspector();  

        LevelPath path = (LevelPath)target;  
        
        // --- НОВОЕ: Выбор LevelPath ---
        // Используем ObjectField, чтобы выбрать конкретный объект уровня в 
сцене
        EditorGUILayout.ObjectField("Target LevelPath", target, 
typeof(LevelPath), true);

        if (GUILayout.Button("Update Path from Scene"))  
        {  
            if (path == null)
            {
                EditorUtility.DisplayDialog("Ошибка", "Выберите LevelPath для 
обновления!", "OK");
                return;
            }
            
            // 1. Находим Waypoints, находящиеся внутри выбранного LevelPath
            GameObject waypointsGO = path.gameObject.Find("Waypoints"); 
            
            if (waypointsGO == null)  
            {
                EditorUtility.DisplayDialog("Ошибка", "Не найден Waypoints в 
объекте уровня!", "OK");
                return;
            }

            // 2. Извлекаем точки
            Transform[] transforms = 
waypointsGO.GetComponentsInChildren<Transform>();  
            path.points = new Vector3[transforms.Length];  
            for (int i = 0; i < transforms.Length; i++)  
            {  
                path.points[i] = transforms[i].position;  
            }

            EditorUtility.SetDirty(path);  
            EditorUtility.DisplayDialog("Готово", $"Обновлено 
{path.points.Length} точек для уровня", "OK");  
        }
        
        serializedObject.ApplyModifiedProperties();
    }  
}  
#endif