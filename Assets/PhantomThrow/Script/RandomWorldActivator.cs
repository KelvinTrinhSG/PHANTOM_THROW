using UnityEngine;

public class RandomWorldActivator : MonoBehaviour
    {
    public GameObject[] worldPrefabs; // Gán trong Inspector: tất cả các world như Summer, Jungle...

    void Start()
        {
        // Bảo vệ: nếu chưa gán gì
        if (worldPrefabs == null || worldPrefabs.Length == 0)
            {
            Debug.LogError("❌ No world prefabs assigned to RandomWorldActivator!");
            return;
            }

        // 1. Tắt toàn bộ trước
        foreach (GameObject world in worldPrefabs)
            {
            if (world != null)
                world.SetActive(false);
            }

        // 2. Random chọn 1 world
        int index = Random.Range(0, worldPrefabs.Length);
        if (worldPrefabs[index] != null)
            {
            worldPrefabs[index].SetActive(true);
            Debug.Log($"🌍 Activated: {worldPrefabs[index].name}");
            }
        }
    }
