using Kogi.EditorTools;
using NUnit.Framework;
using UnityEngine;

namespace Kogi.Tests.Editor
{
    public sealed class SceneSetupComponentTests
    {
        private GameObject temporaryObject;

        [SetUp]
        public void CreateObject()
        {
            temporaryObject = new GameObject("PortalComponentRegressionTest");
        }

        [TearDown]
        public void RemoveObject()
        {
            Object.DestroyImmediate(temporaryObject);
        }

        [Test]
        public void MissingSpriteRendererIsAddedAndCanBeUsed()
        {
            var renderer = Sessions36To46Setup.GetOrAdd<SpriteRenderer>(temporaryObject);
            Assert.That(renderer == null, Is.False);
            Assert.DoesNotThrow(() => renderer.color = Color.cyan);
            Assert.That(temporaryObject.GetComponents<SpriteRenderer>().Length, Is.EqualTo(1));
        }

        [Test]
        public void ExistingComponentIsReusedWithoutDuplicates()
        {
            var existing = temporaryObject.AddComponent<BoxCollider2D>();
            existing.isTrigger = true;
            var result = Sessions36To46Setup.GetOrAdd<BoxCollider2D>(temporaryObject);
            Assert.That(result, Is.SameAs(existing));
            Assert.That(result.isTrigger, Is.True);
            Assert.That(temporaryObject.GetComponents<BoxCollider2D>().Length, Is.EqualTo(1));
        }

        [Test]
        public void RemovedNativeComponentIsRecreated()
        {
            var original = temporaryObject.AddComponent<SpriteRenderer>();
            Object.DestroyImmediate(original);
            var replacement = Sessions36To46Setup.GetOrAdd<SpriteRenderer>(temporaryObject);
            Assert.That(replacement == null, Is.False);
            Assert.DoesNotThrow(() => replacement.color = Color.white);
        }
    }
}
