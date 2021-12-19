using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSession : MonoBehaviour
{
    [SerializeField] private GameObject m_canvas;
    public GameObject Canvas => m_canvas;

    [SerializeField] private GameObject m_enemySpawnerPrefab;

    [SerializeField] private BoxCollider2D m_spawnArea;

    private float m_elapsedTimeSeconds = 0;
    public float ElapsedTimeSeconds => m_elapsedTimeSeconds;

    private int m_levelStage = 0;
    public int LevelStage => m_levelStage;

    private bool m_elapseTime = false;
    bool m_didSpawn = false;

    private int m_livingEnemies = 0;
    public int LivingEnemies => m_livingEnemies;

    private int m_kills = 0;
    public int Kills => m_kills;

    private enum State { PRE_GAME, FIGHT, BETWEEN_FIGHT, END_GAME };
    private State m_state = State.PRE_GAME;

    public void StartGame()
    {
        m_state = State.BETWEEN_FIGHT;
        m_elapseTime = true;
    }

    public void EndGame()
    {
        m_state = State.END_GAME;
        m_elapseTime = false;
    }

    public void NotifyEnemySpawned()
    {
        m_livingEnemies++;
    }

    public void NotifyEnemyKilled()
    {
        m_livingEnemies--;
        m_kills++;
    }

    private Character m_playerCharacter;
    public Character PlayerCharacter {
        get
        {
            if (m_playerCharacter == null)
                m_playerCharacter = FindObjectOfType<PlayerBrain>().Character;

            return m_playerCharacter;
        }
    }

    private void Awake()
    {
        m_canvas.SetActive(false);
    }

    private void Update()
    {
        if (m_elapseTime)
            m_elapsedTimeSeconds += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (m_state == State.BETWEEN_FIGHT)
        {
            if (!m_didSpawn)
            {
                SpawnNewWave();
                m_didSpawn = true;
            }

            if (LivingEnemies > 0)
            {
                m_state = State.FIGHT;
            }
        }
        else if (m_state == State.FIGHT)
        {
            if (LivingEnemies == 0)
            {
                m_state = State.BETWEEN_FIGHT;
                m_didSpawn = false;
            }
        }
    }

    public void SpawnNewWave()
    {
        m_levelStage++;

        for (int i = 0; i < m_levelStage; ++i)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var prefab = Instantiate(m_enemySpawnerPrefab);
        prefab.transform.parent = null;
        prefab.transform.position = GetEnemyPosition();
    }

    private Vector2 GetEnemyPosition()
    {
        float x = Random.Range(m_spawnArea.bounds.min.x, m_spawnArea.bounds.max.x);
        float y = Random.Range(m_spawnArea.bounds.min.y, m_spawnArea.bounds.max.y);

        return new Vector2(x, y) + (Vector2)m_spawnArea.bounds.center;
    }
}
