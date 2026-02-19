using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        Debug.Log("¡SetGridObject LLAMADO! Recibí datos: " + gridObject); // <--- AÑADE ESTO
    }

    private void Update()
    {
        // PROTECCIÓN CONTRA EL ERROR
        if (gridObject == null)
        {
            // Si esto sale en consola, es que SetGridObject nunca se llamó
            Debug.LogWarning("GridDebugObject: ¡Soy un clon pero no tengo datos! (gridObject es null)");
            return; // Salimos antes de que de error
        }

        textMeshPro.text = gridObject.ToString();
    }
}