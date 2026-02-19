using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;   // 씬의 Spawner 참조

    private void Start()
    {
        // 1일차: 게임 시작하자마자 웨이브 1개만 실행
        spawner.StartWave();
    }
}
