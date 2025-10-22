using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    //VELOCIDADES DE MOVIMIENTO
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    //VELOCIDADES DE GRAVEDAD
    [Header("Gravity")]
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;

    //CONTROLAREMOS AL CARACTER MEDIANTE UN VECTOR DE MOVIMIENTO
    private CharacterController controller;
    private Vector3 moveDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Solo mover si estamos en modo 3D
        if (!DimensionManager.Instance.is3DMode)
            return;

        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        // Obtener input
        float horizontal = Input.GetAxis("Horizontal"); // A/D o flechas <- ->
        float vertical = Input.GetAxis("Vertical");     // W/S o flechas ^ v

        // Calcular dirección de movimiento (relativa a la cámara)
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Eliminar componente Y (para no volar)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Dirección de movimiento. forward * vertical (adelante w, atrás s), right + horizontal (izquierda a, derecha d). 
        //SIEMPRE NORMALIZAMOS LOS VECTORES, PARA QUE SUS LONGITUDES VECTORIALES SEAN DE 1 UNIDAD. Para trabajar con direcciones.
        moveDirection = (forward * vertical + right * horizontal).normalized;

        // Mover el personaje
        if (moveDirection.magnitude >= 0.1f)
        {
            //vector direccion (magnitud 1) * velocidad de movimiento * tiempo en ms
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Rotar hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        // Aplicar gravedad.
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // Pequeño valor para mantener grounded
        }
        else
        {
            //La velocidad vertical será la que lleve tras el salto + gravedad * tiempo en ms
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Aplicar velocidad vertical.
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}