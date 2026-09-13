using System;
using System.Collections.Generic;

namespace Kogi.Scripts.Environment
{
    public static class CollectibleProgress
    {
        private static readonly HashSet<string> CollectedIds = new HashSet<string>();

        public static event Action<int> CountChanged;
        public static int Count => CollectedIds.Count;

        public static bool Collect(string collectibleId)
        {
            bool added = CollectedIds.Add(collectibleId);

            if (added)
            {
                CountChanged?.Invoke(Count);
            }

            return added;
        }

        public static bool Contains(string collectibleId)
        {
            return CollectedIds.Contains(collectibleId);
        }

        public static string[] GetIds()
        {
            string[] ids = new string[CollectedIds.Count];
            CollectedIds.CopyTo(ids);
            return ids;
        }

        public static void Restore(string[] ids)
        {
            CollectedIds.Clear();

            if (ids != null)
            {
                foreach (string collectibleId in ids)
                {
                    CollectedIds.Add(collectibleId);
                }
            }

            CountChanged?.Invoke(Count);
        }
    }
}
