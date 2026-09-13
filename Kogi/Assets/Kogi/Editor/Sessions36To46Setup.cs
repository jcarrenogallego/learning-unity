using System;
using System.Linq;
using Kogi.Scripts.Enemies;
using Kogi.Scripts.Environment;
using Kogi.Scripts.Interaction;
using Kogi.Scripts.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kogi.EditorTools
{
    internal static class Sessions36To46Setup
    {
        private const string AppliedKey = "Kogi.Sessions36To46Setup.Applied.v1";
        private const string DesertPath = "Assets/Kogi/Scenes/NivelDesierto.unity";
        private const string SanctuaryPath = "Assets/Kogi/Scenes/SantuarioPrueba.unity";

        [MenuItem("Kogi/Setup/Apply sessions 36-46")]
        private static void Apply()
        {
            if (SessionState.GetBool(AppliedKey, false) || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            try
            {
                SetupDesert();
                Debug.Log("Session 39 verified: desert portal configured.");
                Debug.Log("Session 40 verified: desert checkpoint configured.");
                Debug.Log("Session 42 verified: desert collectibles configured.");
                Debug.Log("Session 44 verified: enemy variants configured.");

                SetupSanctuary();
                Debug.Log("Session 39 verified: SantuarioPrueba created and return portal configured.");
                Debug.Log("Session 43 verified: Kogi interaction, door, lever and NPC configured.");
                Debug.Log("Session 45 verified: Desert Warden configured.");

                ConfigureBuild();
                Validate();
                EditorSceneManager.OpenScene(DesertPath, OpenSceneMode.Single);
                SessionState.SetBool(AppliedKey, true);
                Debug.Log("Sessions 36-46 scene setup completed and verified.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static void SetupDesert()
        {
            OpenForSetup(DesertPath);
            Sprite square = FindSquareSprite();
            GameObject kogi = Require("Kogi");
            SetupInteractor(kogi);

            GameObject portalObject = GetOrCreateVisual("PortalAlSantuario", square, new Color(0.2f, 0.85f, 1f, 0.75f));
            portalObject.transform.position = new Vector3(14f, 0f, 0f);
            portalObject.transform.localScale = new Vector3(0.65f, 3f, 1f);
            BoxCollider2D portalCollider = GetOrAdd<BoxCollider2D>(portalObject);
            portalCollider.isTrigger = true;
            GetOrAdd<ScenePortal>(portalObject).Configure("SantuarioPrueba");

            GameObject checkpointObject = GetOrCreateVisual("CheckpointDesierto", square, new Color(0.2f, 0.8f, 1f, 0.8f));
            checkpointObject.transform.position = new Vector3(7.5f, 0f, 0f);
            checkpointObject.transform.localScale = new Vector3(0.35f, 1.5f, 1f);
            GetOrAdd<BoxCollider2D>(checkpointObject).isTrigger = true;
            Transform respawn = GetOrCreateChild(checkpointObject.transform, "RespawnPosition");
            respawn.localPosition = new Vector3(0f, 0.25f, 0f);
            GetOrAdd<Checkpoint>(checkpointObject).Configure("desierto-centro", respawn);

            CreateCollectible("ReliquiaDesierto01", "desert-relic-01", new Vector3(4f, 0.5f, 0f), square);
            CreateCollectible("ReliquiaDesierto02", "desert-relic-02", new Vector3(8f, 0.5f, 0f), square);

            ConfigureVariant("GuardiaIzquierda", EnemyVariant.Variant.Scout);
            ConfigureVariant("GuardiaBasico", EnemyVariant.Variant.Guardian);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(SanctuaryPath))
            {
                if (!AssetDatabase.CopyAsset(DesertPath, SanctuaryPath))
                {
                    throw new InvalidOperationException("Unity could not create SantuarioPrueba.");
                }

                AssetDatabase.ImportAsset(SanctuaryPath, ImportAssetOptions.ForceUpdate);
            }
        }

        private static void SetupSanctuary()
        {
            OpenForSetup(SanctuaryPath);
            Sprite square = FindSquareSprite();
            SetupInteractor(Require("Kogi"));

            DestroyIfPresent("CheckpointDesierto");
            DestroyIfPresent("ReliquiaDesierto01");
            DestroyIfPresent("ReliquiaDesierto02");

            GameObject portal = GameObject.Find("PortalAlDesierto");
            if (portal == null)
            {
                portal = Require("PortalAlSantuario");
            }
            portal.name = "PortalAlDesierto";
            portal.transform.position = new Vector3(-14f, 0f, 0f);
            portal.GetComponent<ScenePortal>().Configure("NivelDesierto");

            GameObject door = GetOrCreateVisual("PuertaSantuario", square, new Color(0.18f, 0.45f, 0.7f));
            door.transform.position = new Vector3(4f, 0f, 0f);
            door.transform.localScale = new Vector3(0.8f, 3f, 1f);
            door.layer = Mathf.Max(0, LayerMask.NameToLayer("Ground"));
            GetOrAdd<BoxCollider2D>(door).isTrigger = false;
            DoorController doorController = GetOrAdd<DoorController>(door);

            GameObject lever = GetOrCreateVisual("PalancaSantuario", square, new Color(0.95f, 0.75f, 0.2f));
            lever.transform.position = new Vector3(2f, -0.75f, 0f);
            lever.transform.localScale = new Vector3(0.25f, 0.75f, 1f);
            GetOrAdd<BoxCollider2D>(lever).isTrigger = true;
            GetOrAdd<Lever>(lever).Configure(doorController);

            GameObject npc = GetOrCreateVisual("ViajeroPrueba", square, new Color(0.3f, 0.9f, 0.65f));
            npc.transform.position = new Vector3(-3f, -0.5f, 0f);
            npc.transform.localScale = new Vector3(0.7f, 1.5f, 1f);
            GetOrAdd<CircleCollider2D>(npc).isTrigger = true;
            GetOrAdd<NpcDialogue>(npc);

            GameObject elite = GameObject.Find("GuardiaElite");
            if (elite == null)
            {
                elite = UnityEngine.Object.Instantiate(Require("GuardiaBasico"));
                elite.name = "GuardiaElite";
            }
            elite.transform.position = new Vector3(6.5f, 0f, 0f);
            GetOrAdd<EnemyVariant>(elite).Configure(EnemyVariant.Variant.Elite);

            GameObject boss = GameObject.Find("GuardianDelDesierto");
            if (boss == null)
            {
                boss = UnityEngine.Object.Instantiate(Require("GuardiaBasico"));
                boss.name = "GuardianDelDesierto";
            }
            boss.transform.position = new Vector3(10.5f, 0f, 0f);
            boss.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
            DisableIfPresent<EnemyBrain>(boss);
            DisableIfPresent<EnemyPatrol>(boss);
            // A disabled MonoBehaviour still runs Awake: remove the variant so it
            // cannot overwrite the boss health/cooldown after DesertWarden.Awake.
            EnemyVariant bossVariant = boss.GetComponent<EnemyVariant>();
            if (bossVariant != null)
            {
                UnityEngine.Object.DestroyImmediate(bossVariant);
            }
            GetOrAdd<DesertWarden>(boss);

            RenderSettings.ambientLight = new Color(0.08f, 0.12f, 0.2f);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
        }

        private static void SetupInteractor(GameObject kogi)
        {
            Transform origin = GetOrCreateChild(kogi.transform, "InteractionOrigin");
            origin.localPosition = new Vector3(0.7f, 0f, 0f);
            GetOrAdd<KogiInteractor>(kogi).Configure(origin, 1.2f);
        }

        private static void CreateCollectible(string objectName, string id, Vector3 position, Sprite sprite)
        {
            GameObject collectible = GetOrCreateVisual(objectName, sprite, new Color(0.3f, 0.95f, 1f));
            collectible.transform.position = position;
            collectible.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
            GetOrAdd<CircleCollider2D>(collectible).isTrigger = true;
            GetOrAdd<Collectible>(collectible).Configure(id);
        }

        private static void ConfigureVariant(string objectName, EnemyVariant.Variant variant)
        {
            GameObject enemy = Require(objectName);
            GetOrAdd<EnemyVariant>(enemy).Configure(variant);
        }

        private static void ConfigureBuild()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(DesertPath, true),
                new EditorBuildSettingsScene(SanctuaryPath, true)
            };
            PlayerSettings.productName = "Kogi";
            PlayerSettings.companyName = "Kogi Learning Project";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            AssetDatabase.SaveAssets();
        }

        private static void Validate()
        {
            string[] scenePaths = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (!scenePaths.SequenceEqual(new[] { DesertPath, SanctuaryPath }))
            {
                throw new InvalidOperationException("Build scene order is invalid.");
            }

            foreach (string path in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                if (!scene.IsValid() || GameObject.Find("Kogi") == null)
                {
                    throw new InvalidOperationException($"Scene validation failed: {path}");
                }
            }

            EditorSceneManager.OpenScene(SanctuaryPath, OpenSceneMode.Single);
            string[] required = { "PortalAlDesierto", "PuertaSantuario", "PalancaSantuario", "ViajeroPrueba", "GuardiaElite", "GuardianDelDesierto" };
            if (required.Any(name => GameObject.Find(name) == null))
            {
                throw new InvalidOperationException("SantuarioPrueba hierarchy is incomplete.");
            }
        }

        private static Sprite FindSquareSprite()
        {
            SpriteRenderer renderer = Require("Suelo").GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null)
            {
                throw new InvalidOperationException("Suelo does not provide the provisional square sprite.");
            }
            return renderer.sprite;
        }

        private static GameObject GetOrCreateVisual(string objectName, Sprite sprite, Color color)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                gameObject = new GameObject(objectName);
            }
            SpriteRenderer renderer = GetOrAdd<SpriteRenderer>(gameObject);
            renderer.sprite = sprite;
            renderer.color = color;
            return gameObject;
        }

        private static Transform GetOrCreateChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                return child;
            }
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(parent, false);
            return childObject.transform;
        }

        private static GameObject Require(string objectName)
        {
            GameObject found = GameObject.Find(objectName);
            if (found == null)
            {
                throw new InvalidOperationException($"Required GameObject not found: {objectName}");
            }
            return found;
        }

        internal static T GetOrAdd<T>(GameObject gameObject) where T : Component
        {
            // Unity's missing native objects can have a non-null managed wrapper.
            // Use Unity's equality operator, not the CLR-only null-coalescing operator.
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            if (component == null)
            {
                throw new InvalidOperationException($"Could not add {typeof(T).Name} to {gameObject.name}.");
            }
            return component;
        }

        private static void OpenForSetup(string path)
        {
            Scene active = SceneManager.GetActiveScene();
            if (active.path == path)
            {
                return;
            }
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                if (SceneManager.GetSceneAt(index).isDirty)
                {
                    throw new InvalidOperationException("Guarda las escenas abiertas antes de aplicar la configuración.");
                }
            }
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        private static void DisableIfPresent<T>(GameObject gameObject) where T : Behaviour
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                component.enabled = false;
            }
        }

        private static void DestroyIfPresent(string objectName)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
    }
}
