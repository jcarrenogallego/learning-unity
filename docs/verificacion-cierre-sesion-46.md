# Cierre técnico del prototipo — sesión 46

Fecha: 13 de septiembre de 2026. Unity: **6000.5.9f1**. Plataforma: **Windows x64 / Mono**.

## Resultado

Se ha generado `Builds/Windows/Kogi.exe`, junto con sus dependencias. La build normal terminó correctamente, con **0 errores**, aproximadamente **137 MiB** de contenido y un aviso de Pipeline: su conexión de automatización no está habilitada dentro del Player. No impide jugar y no es necesario activarla para distribuir el prototipo.

El ejecutable normal se abrió en una ventana de 1280 × 720, cargó el desierto, ejecutó la partida hasta la derrota y se cerró de forma normal. No aparecieron excepciones ni errores en `Builds/Windows-player.log` durante esta prueba de arranque.

## Pruebas realizadas

- **7 pruebas de Test Runner** aprobadas: 3 de progreso, 3 de creación de componentes y 1 de muerte del jefe con entrada temporal en Play.
- **38 comprobaciones dirigidas** aprobadas en el Editor.
- **38 comprobaciones dirigidas** aprobadas en un ejecutable Windows de desarrollo, también con la ventana visible y renderizado comprobado mediante captura.
- Pantalla de victoria revisada visualmente en Editor y Windows, con «Jugar de nuevo» y, en Windows, «Salir».

Las comprobaciones dirigidas cubren entrada de movimiento, salto, agacharse/levantarse, ataque, daga, componentes visuales, audio ambiental, pausa/reanudación, reliquias, checkpoint, daño e invulnerabilidad, reaparición, guardado/carga aislados, recuperación desde derrota, portal al santuario, persistencia del controlador, palanca y puerta, apertura/cierre del diálogo, fases y muerte del jefe, victoria y nueva partida.

Se utilizan teclas virtuales para las acciones de entrada, reposicionamientos para ejercitar triggers y daño directo para comprobar estados. Son **pruebas de integración**, no una partida humana de principio a fin. No certifican dificultad equilibrada, todos los saltos posibles, calidad artística ni todas las combinaciones de estados. Las pruebas previas del usuario complementan esta revisión técnica.

## Rendimiento observado

Muestra dirigida de 30 segundos con enemigos, partículas, desplazamiento de cámara y generación de proyectiles. El jugador se mantiene fuera del contacto físico para evitar que una derrota corte la medición.

| Medición | Editor | Windows visible, 1280 × 720 |
|---|---:|---:|
| FPS medios | 183,4 | 59,94 |
| Percentil 95 del tiempo de frame | 7,41 ms | 16,74 ms |
| Memoria inicial del Player | — | 267,05 MiB |
| Memoria final del Player | — | 267,16 MiB |

Equipo de la prueba: renderizador **AMD Radeon (TM) Graphics**, Direct3D 11. La muestra visible se mantuvo alrededor de 60 FPS y la memoria aumentó unos 109 KiB. No se detectó aquí un problema que justificara una optimización adicional. Esta muestra corta no demuestra ausencia de fugas a largo plazo ni garantiza el mismo resultado en otros equipos.

La primera ejecución de Windows se realizó oculta. Sus comprobaciones funcionales pasaron, pero su captura salió negra y su cifra de FPS **se descartó como medición gráfica**. Se repitió la prueba con ventana visible antes de registrar los valores de esta tabla.

## Correcciones incorporadas al cierre

- Sesiones 37–38: Escape no puede reactivar la simulación detrás de una derrota o victoria; se coordina la pausa con el estado final y se bloquea la entrada de juego.
- Sesión 38: se ignoran pérdidas adicionales de vida cuando la partida ya terminó.
- Sesión 41: cargar limpia derrota/victoria y pausa; guardar durante un estado final no sobrescribe el archivo anterior. Las pruebas usan su propio archivo.
- Sesión 43: se documenta **mantener E** y se ignora la notificación de soltar para no accionar dos veces la palanca.
- Sesión 45: muerte completa del jefe, 12 vidas sin la configuración de una variante heredada, pantalla **¡SANTUARIO COMPLETADO!** y nueva partida sin borrar el archivo guardado.
- Sesión 46: build normal y build de verificación separadas, comprobaciones integradas y registro de resultados.

Unity actualizó automáticamente datos de renderizado y filtrado de shaders de URP al construir. Se conservaron esos datos generados por el Editor. El ajuste de conexión Unity Connect se devolvió al valor desactivado que tenía antes de la build.

## Dónde consultar o repetir

- Ejecutable normal: `Builds/Windows/Kogi.exe`.
- Resultado de la build: `Builds/Windows/build-status.txt` y `build-messages.txt`.
- Integración en Editor: `Builds/VerificationEditor/verification.json`.
- Integración y rendimiento Windows visible: `Builds/VerificationWindowsVisible/verification.json`.
- Captura de victoria Windows: `Builds/VerificationWindowsVisible/victory.png`.
- Procedimiento reproducible: [sesión 46](sesiones/46-pruebas-y-build.md).

`Builds` no se versiona: contiene ejecutables, registros y partidas de prueba reproducibles. Este informe sí queda dentro del repositorio. Para compartir el juego, entrega **toda la carpeta Windows**, no solamente el `.exe`.

## Alcance del cierre

El bloque de fundamentos queda cerrado como **prototipo de aprendizaje con final jugable**. No significa que esté producido el juego completo: quedan para otra etapa el arte definitivo, el pulido de animaciones, el lazo, los tres niveles de la historia y su desenlace.
