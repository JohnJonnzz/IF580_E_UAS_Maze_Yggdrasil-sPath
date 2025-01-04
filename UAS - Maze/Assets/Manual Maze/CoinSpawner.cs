using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spawn Area Settings")]
    public Vector3 spawnAreaMin = new Vector3(-50, 1, -50); // Default titik minimum area spawn
    public Vector3 spawnAreaMax = new Vector3(50, 3, 50);   // Default titik maksimum area spawn

    [Header("Coin Settings")]
    public GameObject CoinPrefab; // Prefab koin yang akan di-spawn
    public int maxCoins = 25;      // Default jumlah maksimum koin
    public float spawnInterval = 1f; // Default waktu antar spawn (detik)

    private List<GameObject> spawnedCoins = new List<GameObject>(); // Daftar koin yang telah di-spawn

    void Start()
    {
        // Mulai coroutine untuk spawn koin secara berkala
        StartCoroutine(SpawnCoinsRoutine());
    }

    IEnumerator SpawnCoinsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnedCoins.Count < maxCoins)
            {
                SpawnCoin();
            }
        }
    }

    void SpawnCoin()
    {
        Vector3 randomPosition;

        // Cari posisi spawn yang valid
        int attempts = 0; // Batas percobaan untuk mencegah infinite loop
        do
        {
            randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            attempts++;
            if (attempts > 100) // Jika terlalu banyak percobaan, keluar dari loop
            {
                Debug.LogWarning("Unable to find valid spawn location for coin.");
                return;
            }
        } 
        while (!IsValidSpawnPosition(randomPosition));

        // Spawn koin di posisi yang valid
        GameObject newCoin = Instantiate(CoinPrefab, randomPosition, Quaternion.identity);

        // Tambahkan koin ke daftar
        spawnedCoins.Add(newCoin);

        // Pastikan untuk menghapus koin dari daftar jika dihancurkan
        newCoin.GetComponent<OnDestroyCallback>().onDestroy += () =>
        {
            spawnedCoins.Remove(newCoin);
        };
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        float checkRadius = 1f; // Radius pemeriksaan untuk koin (sesuaikan dengan ukuran koin)

        // Periksa apakah ada tabrakan dengan objek lain di lokasi ini
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

        foreach (var collider in colliders)
        {
            // Jika lokasi bertabrakan dengan dinding (tag "Wall"), posisi tidak valid
            if (collider.CompareTag("Walls"))
            {
                return false;
            }
        }

        return true; // Tidak ada tabrakan, posisi valid
    }
}
