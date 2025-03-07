using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Wave> waves;
    public float waveBreak = 5f;

    public event EventHandler<bool> OnWaveTriggered;
    public event Action OnAfterFinalWave;
    public int spawnedEnemies;

    internal int currentWaveIndex = 0;

    public GUI gui;
    public Animator endingAnim;

    private void Start()
    {
        instance = this;
        StartCoroutine(WaveLogic());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SkipCurrentWave();
        }
    }

    private void SkipCurrentWave()
    {
        if (currentWaveIndex >= waves.Count) return;
        
        StopAllCoroutines();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies = 0;
        currentWaveIndex++;
        StartCoroutine(WaveLogic());
    }

    IEnumerator WaveLogic()
    {
        while (currentWaveIndex < waves.Count)
        {
            // Update GUI with increasing wave number
            gui.UpdateWaveText((currentWaveIndex + 1).ToString());
            gui.TriggerWaveText((currentWaveIndex + 1).ToString());

            // Trigger wave start
            OnWaveTriggered?.Invoke(this, true);

            // Set the number of enemies to spawn
            spawnedEnemies = waves[currentWaveIndex].enemiesToSpawn;

            // Start spawning
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));

            // Trigger wave end
            OnWaveTriggered?.Invoke(this, false);
            
            if (currentWaveIndex == waves.Count - 1)
            {
                // Final wave completed - keep the last wave number showing
                int finalWaveNumber = waves.Count;
                gui.UpdateWaveText(finalWaveNumber.ToString());
                gui.TriggerWaveText(finalWaveNumber.ToString());
                yield return new WaitForSeconds(waveBreak);

                endingAnim.Play("Ending");
                
                yield break;
            }

            currentWaveIndex++;

            // Wave break before next wave, still showing the completed wave number
            gui.UpdateWaveText(currentWaveIndex.ToString());
            gui.TriggerWaveText(currentWaveIndex.ToString());
            Debug.Log("Wave break!");

            yield return new WaitForSeconds(waveBreak);
        }
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