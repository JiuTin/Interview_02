using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool.Singleton;
using NB_FGT.Combat;
using NB_FGT.Movement;
public class EnemyManager : Singleton<EnemyManager>
{
    private Transform _mainPlayer;
    [SerializeField] private List<GameObject> _allEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _activeEnemies = new List<GameObject>();
    private WaitForSeconds _waitTime;

    private bool _closeAttackCommandCoroutine;          //关闭攻击指令协程
    protected override void Awake()
    {
        base.Awake();
        _mainPlayer = GameObject.FindWithTag("Player").transform;
        _waitTime = new WaitForSeconds(6f);
    }
    private void Start()
    {
        InitActiveEnemt();
        StartCoroutine(EnableEnemyUnitAttackCommand());
        if (_activeEnemies.Count > 0)
            _closeAttackCommandCoroutine = false;
    }
    private void OnDestroy()
    {
        CloseAttackCommandCoroutine();
    }
    public void AddEnemyUnit(GameObject enemy)
    {
     
        if (!_allEnemies.Contains(enemy))
        {
            
            _allEnemies.Add(enemy);
        }
    }
    public void RemoveEnemyUnit(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            EnemyMovementController enemyMovementController;
            if (enemy.TryGetComponent(out enemyMovementController))
            {
                enemyMovementController.EnableCharacterController(false);
                
            }
            _activeEnemies.Remove(enemy);
        }
    }

    public Transform GetMainPlayer() =>_mainPlayer;

    IEnumerator EnableEnemyUnitAttackCommand()
    {
        if (_activeEnemies == null) yield break;  //直接关闭协程
        if (_activeEnemies.Count == 0) yield break;//直接关闭协程
        while (_activeEnemies.Count > 0)
        {
            if (_closeAttackCommandCoroutine)
                yield break;
            var index = Random.Range(0, _activeEnemies.Count);
            if (index < _activeEnemies.Count)
            {
                GameObject temp = _activeEnemies[index];
                if (temp.TryGetComponent(out EnemyCombatControl enemyCombatControl))
                {
                    enemyCombatControl.SetAttackCommand(true);
                }
            }
            yield return _waitTime;
        }
        yield break;//直接关闭协程
    }
    public void StopAllActiveUnit()
    {
        //让当前所有激活的敌人停止攻击
        foreach (var e in _activeEnemies)
        {
            EnemyCombatControl enemyControl;
            if (e.TryGetComponent(out enemyControl))
            { 
                enemyControl.StopAllAction();
            }
        }
    }
    private void InitActiveEnemt()
    {
        foreach (var e in _allEnemies)
        {
            EnemyMovementController enemyMovementController;
            if (e.TryGetComponent(out enemyMovementController))
            {
                enemyMovementController.EnableCharacterController(true);
            }
            if (e.activeSelf)
            {
                _activeEnemies.Add(e);
            }
        }
    }

    private void CloseAttackCommandCoroutine()
    {
        _closeAttackCommandCoroutine = true;
        StopCoroutine(EnableEnemyUnitAttackCommand());
    }
}
