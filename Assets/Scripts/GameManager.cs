using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Wave> waves;
    public float waveBreak = 5f;

    public event EventHandler<bool> OnWaveTriggered;
    public int spawnedEnemies;

    private int currentWaveIndex = 0;

    public bool loopWaves = false;

    public GUI gui;

    private void Start()
    {
        instance = this;
        StartCoroutine(WaveLogic());
    }

    IEnumerator WaveLogic()
    {
        while (currentWaveIndex < waves.Count)
        {
            // Update GUI
            gui.TriggerWaveText(currentWaveIndex + 1);
            gui.UpdateWaveText(currentWaveIndex + 1);

            // Trigger wave start
            OnWaveTriggered?.Invoke(this, true);

            // Set the number of enemies to spawn
            spawnedEnemies = waves[currentWaveIndex].enemiesToSpawn;

            // Start spawning
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));

            // Trigger wave end
            OnWaveTriggered?.Invoke(this, false);
            currentWaveIndex++;

            // Wave break before next wave
            Debug.Log("Wave break!");
            yield return new WaitForSeconds(waveBreak);
        }

        if(loopWaves)
        {
            currentWaveIndex = 0;
            StartCoroutine(WaveLogic());
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        int spawnedEnemiess = 0;
        while (spawnedEnemiess < wave.enemiesToSpawn)
        {
            // Spawn an enemy
            TriggerSpawn(wave);
            spawnedEnemiess++; // Increment counter
            yield return new WaitForSeconds(wave.spawnSpeed);
        }

        // Wait until all enemies are killed
        while (spawnedEnemies > 0)
        {
            yield return null;
        }
    }

    private void TriggerSpawn(Wave wave)
    {
        // Select a random spawn point
        Transform positionSpawn = wave.spawnPoints[UnityEngine.Random.Range(0, wave.spawnPoints.Count)];

        // Select a random enemy
        GameObject chosenEnemy = wave.enemies[UnityEngine.Random.Range(0, wave.enemies.Count)];

        // Spawn the enemy
        GameObject enemy = Instantiate(chosenEnemy, positionSpawn.position, positionSpawn.rotation);

        // Assign GameManager to the spawned enemy
        enemy.GetComponent<ParentEnemyAI>().manager = this;
    }
}

[Serializable]
public class Wave
{
    public List<GameObject> enemies = new();
    public List<Transform> spawnPoints = new();

    public int enemiesToSpawn;
    public float spawnSpeed;
}