using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyPrefabs;

    [SerializeField] float minSpawnDistance = 60f;
    [SerializeField] float maxSpawnDistance = 100f;
    [SerializeField] float spawnDelay = 0.2f;
    [SerializeField] int spawnCount = 10;

    Transform target;
    void Start()
    {
        target = FindFirstObjectByType<PlayerHealth>().transform;
        StartCoroutine(SpawnEnemyAtRandomPos(spawnCount));

        Vector3 OuterCircle = new Vector3(maxSpawnDistance + target.position.x, 0, maxSpawnDistance + target.position.z);
        Vector3 innerCircle = new Vector3(minSpawnDistance + target.position.x, 0, minSpawnDistance + target.position.z);

        transform.position =  Random.insideUnitCircle * minSpawnDistance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemyAtRandomPos(int spawnAmount) {

        for (int i = 0; i < spawnAmount; i++) {
            Vector3 pos = target.position;

            Vector2 randomDriection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 randomPos = new Vector3(pos.x + randomDriection.x * randomDistance,
                1, pos.z + randomDriection.y * randomDistance);

            Instantiate(EnemyPrefabs[Random.Range(0,EnemyPrefabs.Length)], randomPos, Quaternion.identity);
            Debug.Log(randomPos);
            yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Spawn Sequence Done!");
        yield return new WaitForSeconds(1f);
    }

     Vector2 RandomOutsideUnitCircle(float minRadius, Vector3 directionParent) {
        // minRadius: dairenin dışına ne kadar mesafeden başlasın
        Vector2 dir = directionParent.normalized;
        float dist = Random.Range(minRadius, minRadius + 1f);
        return dir * dist;
    }

}
