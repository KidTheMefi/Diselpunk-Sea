using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using LootSettings;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleHandler : MonoBehaviour
{
    [SerializeField]
    private BaseShip _corvettePrefab;
    [SerializeField]
    private BaseShip _frigatePrefab;
    [SerializeField]
    private BaseShip _ironcladPrefab;
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
    
    private void Start()
    {
        StartAsync().Forget();
    }
    private async UniTask StartAsync()
    {
        await _baseShipPlayer.Setup();
        Patrol();
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
                Destroy(_baseShipEnemy.gameObject);
                Patrol();
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
        AddAllShells();
        
        Destroy(_baseShipEnemy.gameObject);
        _lootHandler.BeginSelecting(Patrol);
        //_lootHandler.ShowMenu(true);
    }

    private void AddRecoverability()
    {
        var possibleRecoverability = _baseShipEnemy.ShipSurvivability.SurvivabilityValue + Random.Range(1,6);
        _lootHandler.AddLootButton($"Add recoverability {possibleRecoverability}",
            () => { _baseShipPlayer.ShipSurvivability.AddRecoverability(possibleRecoverability);  });
    }

    private void AddCrew()
    {
        var possibleCrewRecruit = _baseShipEnemy.ShipCrewHandler.OnDutyCrewValue / 2 + Random.Range(1,4);
        _lootHandler.AddLootButton($"Add crew {possibleCrewRecruit}",
            () => { _baseShipPlayer.ShipCrewHandler.AddNewCrew(possibleCrewRecruit); });
    }

    private void AddAllShells()
    {
        RandomShellsLoot randomShellsLoot = new RandomShellsLoot(_baseShipPlayer);

        var action = randomShellsLoot.GetAction();
        _lootHandler.AddLootButton(randomShellsLoot.GetDescription(), action);
        
        /*
        _lootHandler.AddLootButton(
            $"Add 3 of all shell type",
            () =>
            {
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.Shrapnel, 3);
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.ArmorPiercing, 3);
                _baseShipPlayer.ShellsHandler.AddShell(ShellType.HighExplosive, 3);
            });*/
    }
    
    private void Patrol()
    {
        var firstShip = GetRandomShip();
        _lootHandler.AddLootButton($"Approach {firstShip.gameObject.name}",
            () =>ShipBattle(firstShip).Forget(), false);
        
        var secondShip = GetRandomShip();
        _lootHandler.AddLootButton($"Approach {secondShip.gameObject.name}",
            () =>ShipBattle(secondShip).Forget(), false);

        _lootHandler.BeginSelecting(null);
    }

    private BaseShip GetRandomShip()
    {
        int shipNumber =  Random.Range(1, 6);
        BaseShip enemyShip = shipNumber switch
        {
            1 => _corvettePrefab,
            2 => _frigatePrefab,
            3 => _destroyerPrefab,
            4 => _kruiserPrefab,
            5 => _ironcladPrefab,
            _ => _corvettePrefab
        };
        return enemyShip;
    }

    private async UniTask ShipBattle(BaseShip enemyShip)
    {
        _baseShipEnemy = Instantiate(enemyShip, new Vector3(-3,3,0), Quaternion.identity);
        _baseShipPlayer.EndBattleEvent += OnPlayerEndBattleEvent;
        _baseShipEnemy.EndBattleEvent += EnemyEndBattleEvent;
        await _baseShipEnemy.Setup();
        _baseShipEnemy.SetEnemy(_baseShipPlayer);
        _baseShipPlayer.SetEnemy(_baseShipEnemy);
    }
}