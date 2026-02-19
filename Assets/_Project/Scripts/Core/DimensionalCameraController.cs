using System.Collections;
using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Controla la transición de cámara entre modo ortográfico (2D) y perspectiva (3D).
    /// Usa SmoothStep para una interpolación suave. Se suscribe al evento OnDimensionChanged.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class DimensionalCameraController : MonoBehaviour
    {
        [Header("Configuración 2D (Ortográfica)")]
        [SerializeField] private float orthoSize = 5f;

        [Header("Configuración 3D (Perspectiva)")]
        [SerializeField] private float fieldOfView = 60f;

        [Header("Transición")]
        [SerializeField] private float transitionDuration = 0.8f;

        private Camera cam;
        private Coroutine transitionCoroutine;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            // Estado inicial: modo 2D
            cam.orthographic     = true;
            cam.orthographicSize = orthoSize;
        }

        private void OnEnable()  => DimensionalManager.OnDimensionChanged += HandleDimensionChange;
        private void OnDisable() => DimensionalManager.OnDimensionChanged -= HandleDimensionChange;

        private void HandleDimensionChange(Dimension dimension)
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionCamera(dimension));
        }

        private IEnumerator TransitionCamera(Dimension dimension)
        {
            float elapsed  = 0f;
            bool  goingTo2D = dimension == Dimension.TwoD;

            // Snapshot del estado inicial de la transición
            float fromOrtho = cam.orthographicSize;
            float fromFOV   = cam.orthographic ? 0.1f : cam.fieldOfView;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

                if (goingTo2D)
                {
                    // Perspectiva → Ortográfica: reducir FOV hasta cortar y activar ortho
                    if (!cam.orthographic)
                    {
                        cam.fieldOfView = Mathf.Lerp(fromFOV, 0.1f, t);
                        if (t >= 0.98f)
                        {
                            cam.orthographic     = true;
                            cam.orthographicSize = orthoSize;
                        }
                    }
                }
                else
                {
                    // Ortográfica → Perspectiva: activar perspectiva y abrir FOV
                    if (cam.orthographic)
                    {
                        cam.orthographic = false;
                        cam.fieldOfView  = 0.1f;
                    }
                    cam.fieldOfView = Mathf.Lerp(0.1f, fieldOfView, t);
                }

                yield return null;
            }

            // Estado final limpio
            cam.orthographic     = goingTo2D;
            cam.orthographicSize = goingTo2D ? orthoSize   : cam.orthographicSize;
            cam.fieldOfView      = goingTo2D ? cam.fieldOfView : fieldOfView;
        }
    }
}
