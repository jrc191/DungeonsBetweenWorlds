using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    // Singleton simple para acceder fácil (opcional, pero útil en prototipos)
    public static LevelGrid Instance { get; private set; }

    [Header("Configuración del Grid")]
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f;

    private GridSystem gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Hay más de un LevelGrid en la escena " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicializamos el sistema pasando el prefab de debug y 'this.transform' como padre
        gridSystem = new GridSystem(width, height, cellSize, gridDebugObjectPrefab, this.transform);
    }

    // Exponemos funciones del sistema para que otros scripts no accedan a GridSystem directamente
    public GridObject GetGridObject(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellSize() => cellSize;

    // --- VISUALIZACIÓN EXTRA (GIZMOS) ---
    // Esto dibuja líneas en la vista de escena sin necesidad de Play Mode
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Dibujamos la base de cada celda
                Vector3 startPos = new Vector3(x, 0, z) * cellSize;

                // Línea horizontal (abajo)
                Gizmos.DrawLine(startPos, startPos + Vector3.right * cellSize);
                // Línea vertical (izquierda)
                Gizmos.DrawLine(startPos, startPos + Vector3.forward * cellSize);
            }
        }
        // Cerrar el cuadro exterior
        Gizmos.DrawLine(new Vector3(0, 0, height * cellSize), new Vector3(width * cellSize, 0, height * cellSize));
        Gizmos.DrawLine(new Vector3(width * cellSize, 0, 0), new Vector3(width * cellSize, 0, height * cellSize));
    }
}