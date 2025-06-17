using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Use a static instance to be able to access this from anywhere
    public static SceneLoader Instance { get; private set; }

    public string currentLoadedLevel;

    // This is called when the script instance is being loaded.
    private void Awake()
    {
        // --- Singleton Pattern ---
        // If an instance of this already exists and it's not this one, destroy this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Set the instance to this script.
        Instance = this;

        // Make this GameObject persistent across scene loads.
        DontDestroyOnLoad(gameObject);
       
    }

    // Call this method to load a new level
    public void LoadLevel(string sceneName)
    {
        // If a level is already loaded, unload it first.
        if (!string.IsNullOrEmpty(currentLoadedLevel))
        {
            SceneManager.UnloadSceneAsync(currentLoadedLevel);
        }

        // Load the new scene additively (so it adds to the current scene)
        // and handle what happens when it's fully loaded.
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += (asyncOperation) =>
        {
            // Set the newly loaded scene as the active scene.
            // This is important for lighting and other scene-specific settings.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            currentLoadedLevel = sceneName;
        };
    }
}