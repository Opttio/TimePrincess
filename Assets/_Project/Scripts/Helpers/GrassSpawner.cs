using UnityEngine;

namespace _Project.Scripts.Helpers
{
    public class GrassSpawner : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject _prefabA;
        [SerializeField] private GameObject _prefabB;

        [Header("Spawn Settings")] [SerializeField, Min(0)]
        private int countPerPrefab = 300;

        [SerializeField] private Transform parentContainer;

        [Header("Spawn Range")] [SerializeField]
        private float minX = -41f;

        [SerializeField] private float maxX = 11f;
        [SerializeField] private float yPos = -1f;
        [SerializeField] private float minZ = -59f;
        [SerializeField] private float maxZ = 59f;

        [Header("Scale Range")] [SerializeField]
        private float minScale = 1f;

        [SerializeField] private float maxScale = 3f;

        [Header("Rotation Settings")] [SerializeField]
        private bool randomYOnly = true;

#if UNITY_EDITOR
        [ContextMenu("Spawn Grass")]
#endif
        public void SpawnGrass()
        {
            if (parentContainer == null)
            {
                Debug.LogError("❌ Parent Container не заданий!");
                return;
            }

            SpawnPrefab(_prefabA);
            SpawnPrefab(_prefabB);

            Debug.Log($"✅ Спавн завершено: {countPerPrefab * 2} об'єктів трави.");
        }

        private void SpawnPrefab(GameObject prefab)
        {
            if (prefab == null) return;

            for (int i = 0; i < countPerPrefab; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(minX, maxX),
                    yPos,
                    Random.Range(minZ, maxZ)
                );

                float scale = Random.Range(minScale, maxScale);

                Quaternion rot;
                if (randomYOnly)
                {
                    // Random rotation only around the Y axis (natural for grass)
                    rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }
                else
                {
                    // Full random rotation (if needed for props)
                    rot = Random.rotation;
                }

                GameObject go = Instantiate(prefab, pos, rot, parentContainer);
                go.transform.localScale = Vector3.one * scale;
            }
        }
    }
}