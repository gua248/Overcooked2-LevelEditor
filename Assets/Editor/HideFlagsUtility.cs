using UnityEngine;
using UnityEditor;
using System.Linq;


public static class HideFlagsUtility
{
    [MenuItem("Tools/List HideAndDontSave Objects")]
    public static void ListHideAndDontSaveObjects()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        var hiddenObjects = allObjects.Where(go => (go.hideFlags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave).ToList();

        Debug.Log("Objects with HideAndDontSave flag: " + hiddenObjects.Count);
        foreach (GameObject go in hiddenObjects)
        {
            Debug.Log(go.name, go); // Log with the object reference to easily find it in the editor
        }
    }
}