using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject m_enemy;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks m_preSpawnFeedbacks;
    [SerializeField] private MMFeedbacks m_onSpawnFeedbacks;

    private void Awake()
    {
        m_enemy.SetActive(false);
    }

    private void Start()
    {
        m_preSpawnFeedbacks.PlayFeedbacks();
    }

    public void DoSpawn()
    {
        m_enemy.transform.parent = null;
        m_enemy.SetActive(true);

        m_onSpawnFeedbacks.PlayFeedbacks();
    }
}
