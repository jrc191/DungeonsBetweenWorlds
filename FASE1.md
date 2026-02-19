# FASE 1 — Estructura Base del Proyecto

**Rama:** `feature/fase1-base-proyecto`  
**Merge destino:** `main`

---

## Objetivo

Establecer la base sólida del proyecto Unity: estructura de carpetas definitiva, configuración del motor, control de versiones limpio, y preparar el terreno para importar el asset visual principal.

---

## Cambios Realizados

### 1. Limpieza de Ramas Git
- **Eliminadas:** `develop` (local + remote) y `feature/grid-system` (local)
- **Estado actual del repo:**
  ```
  main                          ← única rama base (producción)
  └── feature/fase1-base-proyecto  ← rama activa (esta fase)
  ```

### 2. Corrección del `.gitignore`
Los siguientes patrones estaban rotos (les faltaban caracteres) y se corrigieron:

| Antes (roto) | Después (correcto) |
|---|---|
| `/emp/` | `/[Tt]emp/` |
| `/uild/` | `/[Bb]uild/` |
| `/uilds/` | `/[Bb]uilds/` |
| `/[Uu]serettings/` | `/[Uu]serSettings/` |
| `/ecordings/` | `/[Rr]ecordings/` |

### 3. Estructura de Carpetas `Assets/_Project/`

```
Assets/_Project/
├── Animations/          ← Clips y AnimatorControllers
├── Audio/               ← SFX y música
├── Data/                ← ScriptableObjects y datos de juego
├── Editor/              ← Herramientas y ventanas custom del Editor
├── Materials/           ← Materiales URP
├── Models/              ← Meshes 3D (.fbx, .obj)
├── Prefabs/             ← Prefabs del juego
├── Scenes/              ← Escenas Unity (.unity)
├── Scripts/
│   ├── Core/            ← ★ NUEVO — DimensionalManager, DimensionalObject
│   ├── Grid/            ← Sistema de grid (base existente)
│   ├── Managers/        ← GameManager, LevelManager wrappers
│   ├── Player/          ← ★ NUEVO — HybridPlayerController
│   ├── Systems/         ← ★ NUEVO — Firebase, Leaderboard
│   └── Unit/            ← Lógica de unidades/enemigos
├── Shaders/             ← ShaderGraphs (Billboard, DimensionalOutline)
├── Sprites/
│   └── PixelArtTopDown/ ← ★ NUEVO — Asset "Pixel Art Top Down - Basic"
└── Textures/            ← Texturas URP/Lit
```

### 4. Configuración Unity Verificada

| Setting | Valor | Estado |
|---|---|---|
| Color Space | **Linear** | ✅ Correcto (`m_ActiveColorSpace: 1`) |
| URP Render Graph | Activo en Settings/ | ✅ Verificado |
| Git LFS | Configurado desde commit inicial | ✅ Activo |

---

## ⭐ Importación del Asset: Pixel Art Top Down - Basic

> **Cuándo:** Ahora, antes de continuar con Fase 2.  
> **Requiere:** Tener el asset en tu cuenta de Unity Asset Store.  
> URL: https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605

### Pasos para importar:
1. Abre Unity con este proyecto
2. Ve a **Window → Package Manager**
3. Cambia el filtro a **"My Assets"**
4. Busca **"Pixel Art Top Down - Basic"** → click **Download** → **Import**
5. En el diálogo de importación, importa TODO el contenido

### Pasos para organizar (después de importar):
1. Mueve la carpeta del asset de `Assets/` a `Assets/_Project/Sprites/PixelArtTopDown/`
2. Selecciona **todos los sprites** del pack → en el Inspector configura:
   - **Texture Type:** `Sprite (2D and UI)`
   - **Pixels Per Unit:** `16`  ← crítico para pixel art correcto
   - **Filter Mode:** `Point (No Filter)` ← evita el blur en pixel art
   - **Compression:** `None`
3. Click **Apply** en todos

### Estructura esperada tras la importación:
```
Assets/_Project/Sprites/PixelArtTopDown/
├── Characters/    ← sprites del personaje (idle, walk, etc.)
├── Tiles/         ← tiles de suelo, paredes, bordes
├── Objects/       ← objetos interactivos, cofres, trampas
└── UI/            ← iconos para interfaz (si los hay)
```

### ¿Para qué lo usaremos?
- **Modo 2D:** tiles del suelo y paredes mediante Tilemap + Tile Palette
- **Modo 3D:** sprites billboarded del personaje (siempre miran a la cámara)
- El pack mantiene coherencia visual entre ambas dimensiones del juego

---

## Árbol de Ramas Completo (Planificado)

```
main
├── feature/fase1-base-proyecto      ← ESTA FASE
├── feature/fase2-dimensional-manager
├── feature/fase2-camera-system
├── feature/fase2-player-controller
├── feature/fase3-rendering
├── feature/fase3-mechanics
└── feature/fase4-firebase
```

---

## Próxima Fase

**Fase 2 — Sistema Dimensional**  
Rama: `feature/fase2-dimensional-manager`  
Archivos a crear: `DimensionalManager.cs` (control de capas, culling, física por dimensión)
