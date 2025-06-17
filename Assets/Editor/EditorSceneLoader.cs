using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// This attribute makes the class's static constructor run when the editor is loaded.
[InitializeOnLoad]
public class EditorSceneLoader
{
    // Static constructor is called automatically by the editor.
    static EditorSceneLoader()
    {
        // Subscribe to the event that is fired when the play mode state changes.
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // This method is called when you press Play, Pause, or Stop in the editor.
        // We are only interested in the moment you press Play.
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // --- SCENE SETUP ---
            // Set the name of your manager scene here.
            string managerSceneName = "SceneManager";

            // If the active scene is already the manager scene, do nothing.
            if (EditorSceneManager.GetActiveScene().name == managerSceneName)
            {
                return;
            }

            // Make sure the user has saved any changes to the current scene.
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Store the path of the scene we were in, so we can load it back.
            string currentScenePath = EditorSceneManager.GetActiveScene().path;

            // Load the manager scene first.
            EditorSceneManager.OpenScene(GetScenePath(managerSceneName));

            // If the scene we were in was a valid, saved scene, load it additively.
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Additive);
            }
        }
    }

    // A helper function to find the full path of a scene by its name.
    private static string GetScenePath(string sceneName)
    {
        // Find all scenes in the project
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName)
            {
                return path;
            }
        }
        Debug.LogError("Scene '" + sceneName + "' not found in project. Make sure it is named correctly.");
        return null;
    }
}