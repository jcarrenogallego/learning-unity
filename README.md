# Learning Unity 🎮

Proyecto didáctico para aprender Unity creando un videojuego paso a paso, con C#, buenas prácticas, arquitectura sencilla y pruebas.

## 🕹️ El juego

**Kogi: El lazo del destino** será un juego de acción y plataformas 2D para un jugador.

- Kogi puede avanzar, retroceder, agacharse y saltar.
- Utiliza espada, dagas y un lazo para atacar, balancearse y alcanzar muros.
- Tiene tres vidas para superar obstáculos y enemigos.
- El estilo será 2D ilustrado, colorido y con animaciones expresivas.
- Nivel 1: un reino desértico.
- Nivel 2: islas y barcos en el mar.
- Nivel 3: el inframundo y el enfrentamiento con el jefe final.
- El objetivo es derrotar al jefe y rescatar a la princesa.

Los personajes pertenecerán a un mundo fantástico inspirado visualmente en Oriente Medio, sin representar una cultura o religión real como enemiga.

## 🧰 Herramientas

- **Unity Hub:** instala y administra Unity Editor y los proyectos.
- **Unity Editor:** permite construir y ejecutar el videojuego.
- **Visual Studio Code:** permite escribir y depurar el código C#.
- **Git y GitHub:** guardan el historial y permiten colaborar.
- **Node.js:** permite utilizar las capacidades locales de Codex mediante `npx skills`.

```mermaid
flowchart LR
    A[Unity Hub] --> B[Unity Editor]
    B --> C[Visual Studio Code]
    C --> B
    B --> D[Videojuego]
```

## 🚀 Preparar Windows

Abre PowerShell y ejecuta los pasos en orden.

### 1. Comprobar `winget`

```powershell
winget --version
```

### 2. Instalar Git

```powershell
winget install --id Git.Git --exact --source winget
git --version
```

### 3. Instalar Node.js LTS

```powershell
winget install --id OpenJS.NodeJS.LTS --exact --source winget
node --version
npx --version
```

### 4. Instalar Visual Studio Code y la extensión de Unity

```powershell
winget install --id Microsoft.VisualStudioCode --exact --source winget
code --install-extension visualstudiotoolsforunity.vstuc
code --version
```

### 5. Clonar el repositorio

```powershell
git clone https://github.com/jcarrenogallego/learning-unity.git
cd learning-unity
code .
```

Si ya estás dentro del repositorio, no vuelvas a clonarlo.

### 6. Comprobar las capacidades locales de Codex

```powershell
npx skills list --json
```

Deben aparecer con `"scope": "project"`:

- `new-unity-project`
- `unity-cli`
- `unity-package-management`

### 7. Instalar Unity Hub

```powershell
winget install --id Unity.UnityHub --exact --source winget
winget list --id Unity.UnityHub --exact
```

## 📚 Sesiones

1. [Conocer Unity y crear la primera escena](docs/sesiones/01-primera-escena.md)
2. [Mover a Kogi horizontalmente con C#](docs/sesiones/02-movimiento-horizontal.md)

Después:

1. Abre Unity Hub.
2. Inicia sesión o crea una cuenta.
3. Utiliza Unity Personal; no necesitas comprar Unity Pro para este aprendizaje.

### 8. Instalar Unity Editor

Desde Unity Hub:

1. Entra en **Installs**.
2. Pulsa **Install Editor**.
3. Elige la versión estable más reciente de Unity 6.
4. Añade **Windows Build Support (IL2CPP)**.
5. Pulsa **Install** y espera a que termine.

### 9. Crear el proyecto Kogi

Desde Unity Hub:

1. Entra en **Projects** y pulsa **New project**.
2. Selecciona la plantilla **Universal 2D**.
3. Escribe `Kogi` como nombre.
4. Elige la carpeta de este repositorio como ubicación.
5. Marca **Use Unity CLI**.
6. Deja desmarcado **Use AI Assistant**.
7. En proveedor, elige **Do not select any**: ya utilizamos Git y GitHub.
8. Pulsa **Create project**.
9. Espera a que Unity importe y compile todo sin errores rojos en **Console**.

El proyecto quedará dentro de la carpeta `Kogi`.

## ✅ Comprobación final

```powershell
git --version
node --version
npx --version
code --version
npx skills list --json
winget list --id Unity.UnityHub --exact
```
