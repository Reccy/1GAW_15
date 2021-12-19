using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSession : MonoBehaviour
{
    [SerializeField] private GameObject m_canvas;
    public GameObject Canvas => m_canvas;

    [SerializeField] private GameObject m_enemySpawnerPrefab;

    private float m_elapsedTimeSeconds = 0;
    public float ElapsedTimeSeconds => m_elapsedTimeSeconds;

    private int m_levelStage = 0;
    public int LevelStage => m_levelStage;

    private bool m_elapseTime = false;

    public void BeginTimer()
    {
        m_elapseTime = true;
    }

    public void StopTimer()
    {
        m_elapseTime = false;
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

        if (m_elapsedTimeSeconds > 5 && !didspawn)
        {
            didspawn = true;
            SpawnNewWave();
        }
    }

    bool didspawn = false;

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
        return Vector2.zero;
    }
}
