using UnityEngine;
using UnityEditor; // This line is essential and only works in an "Editor" folder
using UnityEngine.Tilemaps;

public class MultiTilemapEraser : EditorWindow
{
    private GameObject tilemapParent;
    private Grid grid;
    
    private Vector3Int eraseAreaOrigin;
    private Vector3Int eraseAreaSize = Vector3Int.one;

    private enum SelectionState { Idle, WaitingForFirstCorner, WaitingForSecondCorner }
    private SelectionState currentState = SelectionState.Idle;
    private Vector3Int firstCorner;

    [MenuItem("Tools/Multi-Tilemap Eraser")]
    public static void ShowWindow()
    {
        GetWindow<MultiTilemapEraser>("Multi-Tilemap Eraser");
    }

    // OnEnable is called when the window is opened.
    private void OnEnable()
    {
        // This is the critical line. We are telling the SceneView
        // to call our "UpdateSceneView" method every time it redraws.
        // This is how an EditorWindow can interact with the Scene View.
        SceneView.duringSceneGui += UpdateSceneView;
    }

    // OnDisable is called when the window is closed.
    private void OnDisable()
    {
        // It's crucial to "unsubscribe" from the event to prevent errors.
        SceneView.duringSceneGui -= UpdateSceneView;
    }

    // This method draws the GUI for the pop-up window itself.
    void OnGUI()
    {
        // ... (The OnGUI code is identical to the previous answer)
        GUILayout.Label("Multi-Layer Tilemap Eraser", EditorStyles.boldLabel);
        
        tilemapParent = (GameObject)EditorGUILayout.ObjectField("Tilemap Parent (Grid)", tilemapParent, typeof(GameObject), true);

        if (tilemapParent != null) { grid = tilemapParent.GetComponent<Grid>(); }

        EditorGUILayout.Space();

        if (currentState == SelectionState.Idle)
        {
            if (GUILayout.Button("Select Erase Area with Mouse"))
            {
                if (grid == null) { Debug.LogError("Please assign a Tilemap Parent with a Grid component first."); }
                else
                {
                    currentState = SelectionState.WaitingForFirstCorner;
                    Debug.Log("Selection started. Click in the Scene View to set the first corner.");
                }
            }
        }
        else
        {
            if (GUILayout.Button("Cancel Selection")) { currentState = SelectionState.Idle; }
            EditorGUILayout.HelpBox("Waiting for selection in the Scene View...\n1. Click to set the first corner.\n2. Click again to set the second corner.", MessageType.Info);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Define Erase Area (in Tile Coordinates)", EditorStyles.boldLabel);

        eraseAreaOrigin = EditorGUILayout.Vector3IntField("Area Origin (X, Y)", eraseAreaOrigin);
        eraseAreaSize = EditorGUILayout.Vector3IntField("Area Size (Width, Height)", eraseAreaSize);

        EditorGUILayout.Space();

        if (tilemapParent == null) { EditorGUILayout.HelpBox("Please assign a Tilemap Parent Object.", MessageType.Warning); return; }

        if (GUILayout.Button("Erase Tiles in Defined Area")) { EraseTiles(); }
    }

    // This method is called by the SceneView delegate. I've renamed it for clarity.
    void UpdateSceneView(SceneView sceneView)
    {
        if (currentState == SelectionState.Idle || grid == null) { return; }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Vector2 mousePos = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        Vector3Int currentCell = grid.WorldToCell(ray.origin);
        currentCell.z = 0;

        Handles.Label(grid.CellToWorld(currentCell) + new Vector3(0, 1), $"Current Cell: {currentCell}\nClick to set corner.");

        if (currentState == SelectionState.WaitingForSecondCorner)
        {
            DrawSelectionPreview(firstCorner, currentCell);
        }
        
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Event.current.Use();

            if (currentState == SelectionState.WaitingForFirstCorner)
            {
                firstCorner = currentCell;
                currentState = SelectionState.WaitingForSecondCorner;
                Debug.Log($"First corner set at: {firstCorner}");
            }
            else if (currentState == SelectionState.WaitingForSecondCorner)
            {
                CalculateBounds(firstCorner, currentCell);
                currentState = SelectionState.Idle;
                Debug.Log($"Second corner set. Area defined from {eraseAreaOrigin} with size {eraseAreaSize}.");
                Repaint();
            }
        }
        
        sceneView.Repaint();
    }
    
    // --- The rest of the helper methods are unchanged ---
    void DrawSelectionPreview(Vector3Int cornerA, Vector3Int cornerB)
    {
        Vector3Int min = Vector3Int.Min(cornerA, cornerB);
        Vector3Int max = Vector3Int.Max(cornerA, cornerB) + Vector3Int.one;
        BoundsInt previewBounds = new BoundsInt(min, max - min);
        
        Vector3 worldMin = grid.CellToWorld(previewBounds.min);
        Vector3 worldMax = grid.CellToWorld(previewBounds.max);

        Handles.DrawSolidRectangleWithOutline(
            new Rect(worldMin.x, worldMin.y, worldMax.x - worldMin.x, worldMax.y - worldMin.y),
            new Color(0, 0.8f, 1f, 0.25f),
            new Color(0, 0.8f, 1f, 1f)
        );
    }
    
    void CalculateBounds(Vector3Int cornerA, Vector3Int cornerB)
    {
        eraseAreaOrigin = Vector3Int.Min(cornerA, cornerB);
        eraseAreaSize = Vector3Int.Max(cornerA, cornerB) - eraseAreaOrigin + Vector3Int.one;
        eraseAreaSize.z = 1;
    }

    private void EraseTiles()
    {
        Tilemap[] tilemaps = tilemapParent.GetComponentsInChildren<Tilemap>();
        if (tilemaps.Length == 0)
        {
            Debug.LogWarning("No Tilemap components found in the children of the assigned object.");
            return;
        }

        BoundsInt eraseBounds = new BoundsInt(eraseAreaOrigin, eraseAreaSize);
        int tilesErasedCount = 0;

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Vector3Int position in eraseBounds.allPositionsWithin)
            {
                if (tilemap.GetTile(position) != null)
                {
                    tilemap.SetTile(position, null);
                    tilesErasedCount++;
                }
            }
        }
        Debug.Log($"Erased {tilesErasedCount} tiles across {tilemaps.Length} tilemap layers.");
    }
}