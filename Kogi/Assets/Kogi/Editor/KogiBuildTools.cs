using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kogi.EditorTools
{
    public static class KogiBuildTools
    {
        private const string DesertScene = "Assets/Kogi/Scenes/NivelDesierto.unity";
        private const string SanctuaryScene = "Assets/Kogi/Scenes/SantuarioPrueba.unity";

        [MenuItem("Kogi/Validate/Project")]
        public static void ValidateProject()
        {
            string[] enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (!enabledScenes.Contains(DesertScene) || !enabledScenes.Contains(SanctuaryScene))
            {
                throw new InvalidOperationException("NivelDesierto y SantuarioPrueba deben estar habilitadas en la lista del build.");
            }

            if (AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/Kogi/Scripts" }).Length == 0)
            {
                throw new InvalidOperationException("No se encontraron scripts de Kogi.");
            }

            Debug.Log("Kogi project validation passed.");
        }

        [MenuItem("Kogi/Build/Windows 64-bit")]
        public static void BuildWindows()
        {
            BuildWindowsPlayer(false);
        }

        [MenuItem("Kogi/Build/Windows verification player")]
        public static void BuildWindowsVerification()
        {
            BuildWindowsPlayer(true);
        }

        private static void BuildWindowsPlayer(bool verification)
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Sal de Play antes de construir.");
            ValidateProject();
            string outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath,
                verification ? "../../Builds/VerificationPlayer" : "../../Builds/Windows"));
            Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(Path.Combine(outputDirectory, "build-status.txt"), "Building");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = Path.Combine(outputDirectory, "Kogi.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = verification ? BuildOptions.Development : BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            string[] messages = report.steps.SelectMany(step => step.messages)
                .Where(message => message.type == LogType.Error || message.type == LogType.Warning)
                .Select(message => message.type + ": " + message.content).ToArray();
            File.WriteAllLines(Path.Combine(outputDirectory, "build-messages.txt"), messages);
            File.WriteAllText(Path.Combine(outputDirectory, "build-status.txt"),
                $"{report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nBytes: {report.summary.totalSize}\nDuration: {report.summary.totalTime}");

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"La build falló: {report.summary.result}");
            }

            Debug.Log($"Kogi Windows build completed: {report.summary.outputPath}");
        }
    }
}
