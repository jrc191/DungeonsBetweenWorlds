using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    //Velocidades y tamaños. Iguales que movimiento 3d
    [Header("Grid Movement")]
    public float gridSize = 1f;
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        // Inicializar posición objetivo
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Solo mover si estamos en modo 2D
        if (DimensionManager.Instance.is3DMode)
            return;

        if (!isMoving)
        {
            HandleInput();
        }

        MoveToTarget();
    }

    private void HandleInput()
    {
        //Movimiento X,0,Z. Moveremos solo en dos ejes
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector3(0, 0, gridSize); // Arriba (Z+)
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector3(0, 0, -gridSize); // Abajo (Z-)
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector3(-gridSize, 0, 0); // Izquierda (X-)
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = new Vector3(gridSize, 0, 0); // Derecha (X+)
        }

        if (moveDirection != Vector3.zero)
        {
            targetPosition = transform.position + moveDirection;
            isMoving = true;
        }
    }

    private void MoveToTarget()
    {
        if (isMoving)
        {
            // Mover  hacia la posición objetivo. REQUIERE DE LA POSICION INICIAL, POSICIÓN FINAL Y MOVIMIENTO (VELOCIDAD DE MOVIMIENTO * TIEMPO EN MS)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Verificar si llegamos a la posición objetivo.
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}