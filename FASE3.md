# FASE 3 — Renderizado Híbrido y Mecánicas Dimensionales

**Ramas:**
- `feature/fase3-rendering` → Billboard + Outline Render Feature
- `feature/fase3-mechanics` → DimensionalObject + sala de prueba

**Merge destino:** `main`

---

## Objetivo

Implementar el sistema visual que diferencia ambas dimensiones y las mecánicas que permiten que los objetos del mundo existan exclusivamente en una de ellas.

---

## Scripts Creados

### `Assets/_Project/Scripts/Core/DimensionalBillboard.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Qué hace:**
- En **modo 3D**: el objeto rota en `LateUpdate` para siempre mirar a `Camera.main` (billboarding), usando `Quaternion.LookRotation`
- En **modo 2D**: aplica una rotación fija configurable (`rotation2D`, por defecto `90, 0, 0`)
- Opción `lockYAxis`: evita que el sprite se incline — mantiene la verticalidad del personaje

**Cuándo usarlo:** Añadir al GameObject del sprite del jugador (o cualquier sprite 3D) para que siempre sea visible independientemente del ángulo de cámara.

**Parámetros Inspector:**

| Campo | Por defecto | Descripción |
|---|---|---|
| `Lock Y Axis` | `true` | Mantiene el sprite vertical |
| `Rotation 2D` | `(90, 0, 0)` | Rotación fija en modo 2D (top-down) |

---

### `Assets/_Project/Scripts/Core/DimensionalOutlineFeature.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Qué hace:**
- `ScriptableRendererFeature` de URP
- Registra el `DimensionalOutlinePass` en el pipeline de renderizado
- Se configura desde el **URP Renderer Asset** en el Inspector

**Parámetros:**

| Campo | Por defecto | Descripción |
|---|---|---|
| `Outline Material` | null | Material URP Unlit semitransparente |
| `Outline Color` | Azul HDR (0.4, 0.8, 1, 0.35) | Color del contorno fantasmal |
| `Pass Event` | `AfterRenderingOpaques` | Momento del pipeline |

---

### `Assets/_Project/Scripts/Core/DimensionalOutlinePass.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Qué hace:**
- `ScriptableRenderPass` que dibuja los objetos de la **dimensión opuesta** con el material de outline
- En modo 2D → dibuja objetos en `LAYER_3D_ONLY` con contorno azul fantasmal
- En modo 3D → dibuja objetos en `LAYER_2D_ONLY` con contorno naranja fantasmal
- Usa `DrawRenderers` con `overrideMaterial` para no alterar los objetos originales

---

### `Assets/_Project/Scripts/Core/DimensionalObject.cs`
**Namespace:** `DungeonsBetweenWorlds.Core`

**Qué hace:**
- Marca cualquier GameObject como exclusivo de una dimensión
- En `Start()` y en cada cambio dimensional: activa/desactiva `Renderer` y `Collider`
- Gizmo visual en el Editor (azul = solo 2D, naranja = solo 3D)
- Opción `affectChildren`: aplica la visibilidad también a los GameObjects hijos

**Parámetros:**

| Campo | Por defecto | Descripción |
|---|---|---|
| `Visible In Dimension` | `TwoD` | Dimensión donde el objeto existe |
| `Affect Children` | `true` | Propaga a hijos |

---

## Cómo Configurar en Unity

### 1. Activar el Outline Render Feature

1. Ve a `Assets/Settings/` → abre el **URP Renderer Asset** (`PC_RPAsset`)
2. En el Inspector → sección **Renderer Features** → click `+ Add Renderer Feature`
3. Selecciona `DimensionalOutlineFeature`
4. Crea un nuevo Material: `Create → Material`, shader = `Universal Render Pipeline/Unlit`
   - Color: azul semitransparente con alpha ~0.3
   - Surface Type: Transparent
5. Asígnalo al campo `Outline Material` del feature

### 2. Añadir Billboard al jugador

1. Selecciona el GameObject del **sprite del jugador** (el hijo con `SpriteRenderer`)
2. Añade el componente `DimensionalBillboard`
3. Configura:
   - `Lock Y Axis`: ✅
   - `Rotation 2D`: `(90, 0, 0)` para top-down

### 3. Crear objetos dimensionales en la escena

Para un **muro que solo existe en 2D:**
1. Crea un GameObject con `SpriteRenderer` o `MeshRenderer` + `BoxCollider`
2. Asigna el **Layer** → `2D_Only`
3. Añade componente `DimensionalObject`
4. `Visible In Dimension` → `TwoD`

Para un **puente que solo existe en 3D:**
1. Ídem pero Layer `3D_Only` y `Visible In Dimension` → `ThreeD`

---

## Sala de Prueba (Playground)

### Estructura recomendada de escena `Playground.unity`

```
Scene
├── _GameManager          [DimensionalManager]
├── Main Camera           [DimensionalCameraController]
│
├── Player                [Rigidbody, BoxCollider, HybridPlayerController]
│   └── Sprite            [SpriteRenderer, DimensionalBillboard]
│
├── Environment
│   ├── Floor_Shared      [Layer: Shared] — suelo siempre visible
│   ├── Wall_2D_Only      [Layer: 2D_Only, DimensionalObject(TwoD)]
│   ├── Bridge_3D_Only    [Layer: 3D_Only, DimensionalObject(ThreeD)]
│   └── Chest_Shared      [Layer: Shared] — cofre siempre accesible
│
└── Puzzle_Zone
    ├── Door_2D            [DimensionalObject(TwoD)] — puerta solo en 2D
    └── Key_3D             [DimensionalObject(ThreeD)] — llave solo en 3D
```

### Puzzle básico de cambio dimensional:
- La **llave** (`Key_3D`) solo existe en modo 3D — el jugador debe cambiar a 3D para cogerla
- La **puerta** (`Door_2D`) solo se abre en modo 2D — el jugador debe volver a 2D con la llave

---

## Flujo de Renderizado Dimensional

```
Cámara renderiza frame
        │
        ├── Objetos Shared → siempre visibles
        ├── Capa activa (2D_Only ó 3D_Only) → visibles normalmente
        │
        └── DimensionalOutlinePass (AfterOpaques)
                └── Objetos de la capa OPUESTA → dibujados con material
                    outline semitransparente (efecto fantasma)
```

---

## Referencia: TopDown Engine

- **MMBillboard.cs** (`MoreMountains.Tools`): referencia de billboard usada para diseñar `DimensionalBillboard`
- **CharacterGridMovement.cs**: namespace `MoreMountains.TopDownEngine`, enum `DimensionModes.TwoD/ThreeD` — referencia para futuras integraciones del movimiento grid

---

## Próxima Fase

**Fase 4 — Backend Firebase**
Rama: `feature/fase4-firebase`
- Firebase SDK para Unity
- Autenticación anónima
- Firestore: guardado de stats, inventario, checkpoints
- UI de leaderboard top-20
