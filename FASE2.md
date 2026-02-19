# FASE 2 — Sistema Dimensional

**Ramas:**
- `feature/fase2-dimensional-manager`
- `feature/fase2-camera-system`
- `feature/fase2-player-controller`

**Merge destino:** `main`

---

## Objetivo

Implementar el núcleo del juego: el **sistema de cambio dimensional** que permite al jugador alternar entre Modo 2D (cámara ortográfica, movimiento en plano XY) y Modo 3D (cámara perspectiva top-down, movimiento libre en plano XZ).

---

## Arquitectura del Sistema

```
DimensionalManager (Singleton)
    │
    ├── event OnDimensionChanged ──────┬──────────────────────┐
    │                                  │                       │
    ▼                                  ▼                       ▼
Physics layers              DimensionalCameraController   HybridPlayerController
Camera culling mask         (transición ortho↔perspectiva)  (movimiento 2D/3D)
```

---

## Scripts Creados

### `Assets/_Project/Scripts/Core/DimensionalManager.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Responsabilidades:**
- Singleton persistente (`DontDestroyOnLoad`)
- Gestiona las capas de Unity para el sistema dimensional:

| Layer | Índice | Uso |
|---|---|---|
| `2D_Only` | 6 | Objetos que solo existen en modo 2D |
| `3D_Only` | 7 | Objetos que solo existen en modo 3D |
| `Shared` | 8 | Objetos presentes en ambas dimensiones |

- Aplica `Physics.IgnoreLayerCollision` por dimensión activa
- Actualiza `Camera.main.cullingMask` para ocultar la capa exclusiva de la dimensión contraria
- Dispara el evento estático `OnDimensionChanged` para que otros sistemas reaccionen

**API pública:**
```csharp
DimensionalManager.Instance.SwitchDimension();        // Alterna 2D ↔ 3D
DimensionalManager.Instance.SetDimension(Dimension.ThreeD); // Fuerza una dimensión
DimensionalManager.Instance.CurrentDimension;         // Propiedad de solo lectura
DimensionalManager.OnDimensionChanged += callback;    // Evento estático suscribible
```

---

### `Assets/_Project/Scripts/Core/DimensionalCameraController.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Responsabilidades:**
- Se suscribe a `DimensionalManager.OnDimensionChanged`
- Transición suave entre cámara ortográfica (2D) y perspectiva (3D)
- Usa `Mathf.SmoothStep` para una interpolación sin acelerón inicial ni frenada brusca

**Parámetros configurables (Inspector):**

| Campo | Valor por defecto | Descripción |
|---|---|---|
| `Ortho Size` | `5` | Tamaño ortográfico en modo 2D |
| `Field Of View` | `60` | FOV en modo 3D |
| `Transition Duration` | `0.8s` | Duración de la transición |

**Comportamiento de la transición:**
- **2D → 3D:** Activa cámara perspectiva y abre el FOV desde 0.1° hasta el valor configurado
- **3D → 2D:** Cierra el FOV hasta 0.1°, luego activa modo ortográfico con el tamaño configurado

---

### `Assets/_Project/Scripts/Player/HybridPlayerController.cs`
**Namespace:** `DungeonsBetweenWorlds.Player`

**Responsabilidades:**
- Controlador de jugador que cambia su lógica de movimiento según la dimensión activa
- Usa `Rigidbody.MovePosition` (física suave, sin teletransporte)
- Detecta input de teclado para disparar el cambio dimensional

**Modos de movimiento:**

| Dimensión | Plano | Teclas | Velocidad |
|---|---|---|---|
| 2D | XY (plano frontal) | WASD / Flechas | `speed2D` (5 u/s) |
| 3D | XZ (plano suelo) | WASD / Flechas | `speed3D` (7 u/s) |

**Constraints de Rigidbody automáticos:**

| Dimensión | Constraints |
|---|---|
| 2D | `FreezePositionZ + FreezeRotation` |
| 3D | `FreezePositionY + FreezeRotation` |

**Input de cambio dimensional:** Tecla `Q` (configurable en Inspector)

---

## Cómo Configurar en Unity

### 1. Configurar las Layers
En Unity → **Edit → Project Settings → Tags and Layers**:
- Layer 6 → `2D_Only`
- Layer 7 → `3D_Only`
- Layer 8 → `Shared`

### 2. Crear el Game Manager GameObject
1. En la escena, crea un GameObject vacío: `_GameManager`
2. Añade el componente `DimensionalManager`
3. En Inspector → `Start Dimension`: `TwoD`

### 3. Configurar la Cámara
1. Selecciona la cámara principal (`Main Camera`)
2. Añade el componente `DimensionalCameraController`
3. Ajusta `Ortho Size`, `Field Of View` y `Transition Duration` al gusto

### 4. Configurar el Jugador
1. Crea un GameObject para el jugador con `Rigidbody` (Is Kinematic: OFF)
2. Añade el componente `HybridPlayerController`
3. Asigna el layer `Shared` al jugador (puede existir en ambas dimensiones)
4. Tecla de cambio dimensional: `Q` (editable en Inspector)

---

## Flujo de Ejecución

```
[Jugador pulsa Q]
        │
        ▼
HybridPlayerController.Update()
        │
        ▼
DimensionalManager.SwitchDimension()
        │
        ├── ApplyDimension() → Physics.IgnoreLayerCollision + Camera.cullingMask
        │
        └── OnDimensionChanged?.Invoke(newDimension)
                │
                ├── DimensionalCameraController.HandleDimensionChange()
                │       └── StartCoroutine(TransitionCamera)  [SmoothStep]
                │
                └── HybridPlayerController.OnDimensionChanged()
                        └── Cambia plano de movimiento + Rigidbody constraints
```

---

## Próxima Fase

**Fase 3 — Renderizado Híbrido y Mecánicas**
Ramas:
- `feature/fase3-rendering` → BillboardSprite.shadergraph, DimensionalOutline, Render Feature
- `feature/fase3-mechanics` → DimensionalObject.cs, sala de prueba con puzzle
