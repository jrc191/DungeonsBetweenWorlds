using UnityEngine;

public class DimensionalObject : MonoBehaviour
{
    [Header("Visibility Settings")]
    public bool visibleIn3D = true;
    public bool visibleIn2D = true;

    [Header("Physics Settings")]
    public bool physicsIn3D = true;
    public bool physicsIn2D = false;

    private Renderer[] renderers;
    private Collider[] colliders;

    private void Awake()
    {
        // Obtener todos los renderers y colliders del objeto y sus hijos
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        // Actualizar estado según la dimensión inicial
        UpdateDimensionalState(DimensionManager.Instance.is3DMode);
    }

    private void OnEnable()
    {
        // A IMPLEMENTAR
    }

    public void UpdateDimensionalState(bool is3D)
    {
        // Controlar visibilidad. 3d -> visible en 3d, no3d -> visible en 2d
        bool shouldBeVisible = is3D ? visibleIn3D : visibleIn2D;
        //hacemos visibles los renderers según su dimensión
        foreach (Renderer r in renderers)
        {
            r.enabled = shouldBeVisible;
        }

        // Físicas. Mismo principio uqe con los renders, pero con colliders
        bool shouldHavePhysics = is3D ? physicsIn3D : physicsIn2D;
        foreach (Collider c in colliders)
        {
            c.enabled = shouldHavePhysics;
        }
    }
}