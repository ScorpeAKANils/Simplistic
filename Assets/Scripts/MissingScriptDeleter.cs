using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingScriptDeleter : MonoBehaviour
{
    [MenuItem("Tools/Find missing scripts/DeleteAll")]
    static void FindAndDeleteMissingScripts()
    {
        foreach (GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
        {
            foreach (Component component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                    break;
                }
            }
        }
    }

    [MenuItem("Tools/Remove Missing Scripts from Prefabs")]
    public static void RemoveMissingScriptsFromPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int count = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                int before = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                if (before > 0)
                {
                    count += before;
                    EditorUtility.SetDirty(prefab);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Removed {before} missing scripts from {path}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Finished! Removed {count} missing scripts from prefabs.");
    }
}
