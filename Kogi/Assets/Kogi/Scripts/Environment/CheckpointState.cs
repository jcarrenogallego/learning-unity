using System.Collections.Generic;
using UnityEngine;

namespace Kogi.Scripts.Environment
{
    public static class CheckpointState
    {
        private static readonly Dictionary<string, Vector2> PositionsByScene = new Dictionary<string, Vector2>();
        private static readonly Dictionary<string, string> IdsByScene = new Dictionary<string, string>();

        public static void Activate(string sceneName, string checkpointId, Vector2 position)
        {
            PositionsByScene[sceneName] = position;
            IdsByScene[sceneName] = checkpointId;
        }

        public static Vector2 GetRespawnPosition(string sceneName, Vector2 fallback)
        {
            return PositionsByScene.TryGetValue(sceneName, out Vector2 position) ? position : fallback;
        }

        public static string GetActiveId(string sceneName)
        {
            return IdsByScene.TryGetValue(sceneName, out string checkpointId) ? checkpointId : string.Empty;
        }

        public static bool TryGet(string sceneName, out string checkpointId, out Vector2 position)
        {
            bool hasId = IdsByScene.TryGetValue(sceneName, out checkpointId);
            bool hasPosition = PositionsByScene.TryGetValue(sceneName, out position);
            return hasId && hasPosition;
        }

        public static void Clear()
        {
            PositionsByScene.Clear();
            IdsByScene.Clear();
        }
    }
}
