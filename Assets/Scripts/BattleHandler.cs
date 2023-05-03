using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleHandler : MonoBehaviour
{
    [SerializeField]
    private BaseShip _corvettePrefab;
    [SerializeField]
    private BaseShip _frigatePrefab;
    [SerializeField]
    private BaseShip _destroyerPrefab;
    [SerializeField]
    private BaseShip _kruiserPrefab;
    
    [SerializeField]
    private BaseShip _baseShipPlayer;
    [SerializeField]
    private BaseShip _baseShipEnemy;
    [SerializeField]
    private LootHandler _lootHandler;

    private int lvl;
    private void Start()
    {
        _lootHandler.LootEnd += () => LootEndAsync().Forget();
        
        NextShipAsync().Forget();
    }
    
    private void Unsub()
    {
        _baseShipPlayer.EndBattleEvent -= OnPlayerEndBattleEvent;
        _baseShipEnemy.EndBattleEvent -= EnemyEndBattleEvent;
    }
    private void EnemyEndBattleEvent(ShipEndBattle end)
    {
        FinishBattle();
        switch (end)
        {
            case ShipEndBattle.Retreat:
                Debug.Log("Enemy ran away");
                break;
            case ShipEndBattle.Sinking:
                LootBegin();
                break;
            case ShipEndBattle.Surrender:
                LootBegin();
                break;
        }
    }
    private void OnPlayerEndBattleEvent(ShipEndBattle end)
    {
        FinishBattle();
        if (end == ShipEndBattle.Retreat)
        {
            Debug.Log("You ran away");
        }
        else
        {
            Debug.Log("game over");
        }
    }

    private void FinishBattle()
    {
        Unsub();
        Debug.Log("Finish Battle");
        _baseShipPlayer.FinishBattle();
        _baseShipEnemy.FinishBattle();
    }

    private void LootBegin()
    {
        _baseShipPlayer.ShipCrewHandler.RecoverInjuredCrew();


        AddRecoverability();
        AddCrew();
        AddShells();
        

        _lootHandler.ShowLoot(true);
    }

    private void AddRecoverability()
    {
        var possibleRecoverability = _baseShipEnemy.ShipSurvivability.SurvivabilityValue + Random.Range(1,6);
        _lootHandler.AddLootButton(
            $"Add recoverability {possibleRecoverability}",
            () =>
            {
                _baseShipPlayer.ShipSurvivability.AddRecoverability(possibleRecoverability);
            });
    }

    private void AddCrew()
    {
        var possibleCrewRecruit = _baseShipEnemy.ShipCrewHandler.OnDutyCrewValue / 2 + Random.Range(1,4);
        _lootHandler.AddLootButton(
            $"Add crew {possibleCrewRecruit}",
            () =>
            {
                _baseShipPlayer.ShipCrewHandler.AddNewCrew(possibleCrewRecruit);
            });
    }

    private void AddShells()
    {
        _lootHandler.AddLootButton(
            $"Add 3 of all shell type",
            () =>
            {
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.Shrapnel, 3);
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.ArmorPiercing, 3);
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.HighExplosive, 3);
            });
    }
    
    private async UniTask LootEndAsync()
    {
        Destroy(_baseShipEnemy.gameObject);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        NextShipAsync().Forget();
    }

    private async UniTask NextShipAsync()
    {
        if (lvl == 0)
        {
           await _baseShipPlayer.Setup();
        }
        
        lvl++;
        int shipNumber = lvl > 4 ? Random.Range(1, 5) : lvl;
        
        BaseShip enemyShip = shipNumber switch
        {
            1 => _corvettePrefab,
            2 => _frigatePrefab,
            3 => _destroyerPrefab,
            4 => _kruiserPrefab,
            _ => _corvettePrefab
        };

        _baseShipEnemy = Instantiate(enemyShip, new Vector3(-3,3,0), Quaternion.identity);
        _baseShipPlayer.EndBattleEvent += OnPlayerEndBattleEvent;
        _baseShipEnemy.EndBattleEvent += EnemyEndBattleEvent;
        await _baseShipEnemy.Setup();
        _baseShipEnemy.SetEnemy(_baseShipPlayer);
        _baseShipPlayer.SetEnemy(_baseShipEnemy);
    }
    
    
    
}