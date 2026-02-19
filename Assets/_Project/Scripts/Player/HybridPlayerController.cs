using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Player
{
    /// <summary>
    /// Controlador de jugador híbrido 2D/3D.
    /// En modo 2D: movimiento en plano XY (grid-style).
    /// En modo 3D: movimiento en plano XZ (top-down libre).
    /// Reacciona al evento OnDimensionChanged del DimensionalManager.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HybridPlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private KeyCode switchDimensionKey = KeyCode.Q;

        [Header("Velocidades")]
        [SerializeField] private float speed2D = 5f;
        [SerializeField] private float speed3D = 7f;

        private Rigidbody rb;
        private Dimension currentDimension = Dimension.TwoD;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        private void OnEnable()  => DimensionalManager.OnDimensionChanged += OnDimensionChanged;
        private void OnDisable() => DimensionalManager.OnDimensionChanged -= OnDimensionChanged;

        private void Update()
        {
            if (Input.GetKeyDown(switchDimensionKey))
                DimensionalManager.Instance?.SwitchDimension();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 dir;
            float   speed;

            if (currentDimension == Dimension.TwoD)
            {
                dir   = new Vector3(h, v, 0f).normalized; // Plano XY
                speed = speed2D;
            }
            else
            {
                dir   = new Vector3(h, 0f, v).normalized; // Plano XZ
                speed = speed3D;
            }

            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        }

        private void OnDimensionChanged(Dimension dimension)
        {
            currentDimension = dimension;

            // Ajustar constraints de físicas según la dimensión
            rb.constraints = dimension == Dimension.TwoD
                ? RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation
                : RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }
}
