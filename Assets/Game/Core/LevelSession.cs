using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

public class LevelSession : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_rp;
    private const string BTN_RESET_GAME = "ResetGame";

    [SerializeField] private GameObject m_canvas;
    public GameObject Canvas => m_canvas;

    [SerializeField] private GameObject m_endGameScreen;

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

    private PlayerBrain m_playerBrain;
    public PlayerBrain PlayerBrain
    {
        get
        {
            if (m_playerBrain == null)
                m_playerBrain = FindObjectOfType<PlayerBrain>();

            return m_playerBrain;
        }
    }

    public Character PlayerCharacter => PlayerBrain.Character;

    private void Awake()
    {
        m_canvas.SetActive(false);

        m_rp = ReInput.players.GetPlayer(PLAYER_ID);
    }

    private void Update()
    {
        if (m_elapseTime)
            m_elapsedTimeSeconds += Time.deltaTime;

        // Reload Scene
        if (m_rp.GetButtonDown(BTN_RESET_GAME) && m_state == State.END_GAME)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FixedUpdate()
    {
        if (m_state == State.BETWEEN_FIGHT)
        {
            if (!m_didSpawn)
            {
                HealPlayer();
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

        if (m_state != State.END_GAME && m_state != State.PRE_GAME)
        {
            if (!PlayerCharacter.IsAlive)
            {
                m_state = State.END_GAME;
            }
        }

        if (m_state == State.END_GAME)
        {
            m_endGameScreen.SetActive(true);
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

    private void HealPlayer()
    {
        PlayerBrain.GainHP();
    }
}
