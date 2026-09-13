using Kogi.Scripts.Environment;
using NUnit.Framework;
using UnityEngine;

namespace Kogi.Tests.Editor
{
    public sealed class FoundationProgressTests
    {
        [SetUp]
        public void SetUp()
        {
            CheckpointState.Clear();
            CollectibleProgress.Restore(new string[0]);
        }

        [Test]
        public void CheckpointFallsBackUntilOneIsActivated()
        {
            Vector2 fallback = new Vector2(1f, 2f);
            Assert.That(CheckpointState.GetRespawnPosition("TestScene", fallback), Is.EqualTo(fallback));
        }

        [Test]
        public void ActivatedCheckpointReplacesFallback()
        {
            Vector2 expected = new Vector2(7.5f, 0.25f);
            CheckpointState.Activate("TestScene", "center", expected);
            Assert.That(CheckpointState.GetRespawnPosition("TestScene", Vector2.zero), Is.EqualTo(expected));
        }

        [Test]
        public void CollectibleCannotBeCountedTwice()
        {
            Assert.That(CollectibleProgress.Collect("relic-01"), Is.True);
            Assert.That(CollectibleProgress.Collect("relic-01"), Is.False);
            Assert.That(CollectibleProgress.Count, Is.EqualTo(1));
        }
    }
}
