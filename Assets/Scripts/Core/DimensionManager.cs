using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance { get; private set; }

    [Header("Cameras")]
    public Camera camera3D;
    public Camera camera2D;

    [Header("Current Dimension")]
    public bool is3DMode = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iniciar en modo 3D
        SetDimension(true);
    }

    private void Update()
    {
        // Cambiar dimensión con la tecla TAB
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleDimension();
        }
    }

    public void ToggleDimension()
    {
        is3DMode = !is3DMode;
        SetDimension(is3DMode);
    }

    private void SetDimension(bool is3D)
    {
        if (is3D)
        {
            // Activar modo 3D
            camera3D.enabled = true;
            camera2D.enabled = false;
        }
        else
        {
            // Activar modo 2D
            camera3D.enabled = false;
            camera2D.enabled = true;
        }

        Debug.Log("Dimensión cambiada a: " + (is3D ? "3D" : "2D"));
    }
}