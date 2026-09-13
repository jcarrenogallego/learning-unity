using System.Collections;
using Kogi.Scripts.Combat;
using Kogi.Scripts.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Kogi.Tests.Editor
{
    public sealed class DesertWardenDeathTests
    {
        private GameObject boss;

        [UnityTest]
        public IEnumerator LethalDamageImmediatelyRemovesCollisionAndThenDestroysBoss()
        {
            // EnterPlayMode exercises real Awake/OnEnable, physics and delayed Destroy.
            yield return new EnterPlayMode();
            Time.timeScale = 1f;

            boss = new GameObject("BossDeathRegressionTest");
            boss.SetActive(false);
            boss.transform.position = new Vector3(10000f, 10000f, 0f);
            var body = boss.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            var collider = boss.AddComponent<CapsuleCollider2D>();
            var contactDamage = boss.AddComponent<EnemyContactDamage>();
            boss.AddComponent<DesertWarden>();

            var child = new GameObject("AdditionalCollider");
            child.transform.SetParent(boss.transform, false);
            var childCollider = child.AddComponent<BoxCollider2D>();
            boss.SetActive(true);

            var health = boss.GetComponent<EnemyHealth>();
            int deathEvents = 0;
            health.Died += () => deathEvents++;
            Assert.That(health.CurrentHealth, Is.EqualTo(12));
            health.TakeDamage(health.CurrentHealth - 1);
            Assert.That(collider.enabled, Is.True, "A living boss must still collide.");

            body.linearVelocity = Vector2.right;
            body.angularVelocity = 10f;
            health.TakeDamage(1);
            health.TakeDamage(1);

            Assert.That(health.CurrentHealth, Is.Zero);
            Assert.That(deathEvents, Is.EqualTo(1));
            Assert.That(collider.enabled, Is.False);
            Assert.That(childCollider.enabled, Is.False);
            Assert.That(body.simulated, Is.False);
            Assert.That(body.linearVelocity, Is.EqualTo(Vector2.zero));
            Assert.That(body.angularVelocity, Is.Zero);
            Assert.That(contactDamage.enabled, Is.False);
            Assert.That(boss.GetComponent<EnemyShooter>().enabled, Is.False);
            Assert.That(boss.GetComponent<EnemyVision>().enabled, Is.False);
            Assert.That(boss.GetComponent<SpriteRenderer>().color, Is.EqualTo(Color.gray));
            Assert.That(boss != null, Is.True, "Keep the short death feedback before removal.");

            // Yield frames (not WaitForSeconds) for compatibility with the EditMode runner.
            float deadline = Time.realtimeSinceStartup + 1f;
            while (boss != null && Time.realtimeSinceStartup < deadline)
            {
                Time.timeScale = 1f;
                yield return null;
            }

            Assert.That(boss == null, Is.True, "The defeated boss must leave the hierarchy.");
        }

        [UnityTearDown]
        public IEnumerator ReturnToEditMode()
        {
            if (boss != null)
            {
                Object.DestroyImmediate(boss);
            }

            if (EditorApplication.isPlaying)
            {
                yield return new ExitPlayMode();
            }
        }
    }
}
