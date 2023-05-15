using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using PrefabsStatic;
using Signals;
using SomeMenu;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BattleHandler : MonoBehaviour
{
    [SerializeField]
    private BaseShip _baseShipPlayer;
    [SerializeField]
    private BaseShip _baseShipEnemy;
    [SerializeField]
    private MenuButtons _menuButtons;

    private Shipyard _shipyard;
    private LootAfterBattle _lootAfterBattle;

    [SerializeField]
    private bool shipyardAlways;

    private UnityEvent _afterLootEvent = new UnityEvent();
    private PlayerShipSelect _shipSelect;
    
    private void Start()
    {
        MenuSignal.Instance.BattleWithShip += OnBattleWithShip;
        MenuSignal.Instance.BackToPatrol += OnBackToPatrol;
        
        _shipSelect = new PlayerShipSelect(_menuButtons);
        _shipSelect.ShipSelected += ShipSelected;
        _shipSelect.ShowShipsVariants();
    }


    private void ShipSelected(BaseShip ship)
    {
        _shipSelect.ShipSelected -= ShipSelected;
        _baseShipPlayer = Instantiate(ship, new Vector3(-3, -2.5f), quaternion.identity);
        _lootAfterBattle = new LootAfterBattle(_baseShipPlayer, _menuButtons);
        _lootAfterBattle.LeaveLoot += OnLeaveLoot;
        StartAsync().Forget();
    }
    private async UniTask StartAsync()
    {
        await _baseShipPlayer.Setup();
        Patrol();
    }

    private void OnLeaveLoot()
    {
        if (_afterLootEvent.GetPersistentEventCount() == 0)
        {
            _afterLootEvent.AddListener(() =>
            {
                var shipyardOption = new SosSignalEvent(_baseShipPlayer, _menuButtons, () => Patrol(0));
                _menuButtons.AddButton($"Listen radio waves", shipyardOption.ShowEventsOptions);
                _menuButtons.AddButton($"Go patrol", () => Patrol(30));
                string menuInfo = "After battle you receive S.O.S. signal nearby";
                _menuButtons.ShowMenu(menuInfo);
            });
        }
        _afterLootEvent.Invoke();
        _afterLootEvent.RemoveAllListeners();
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
                Patrol(50);
                break;
            case ShipEndBattle.Sinking:
                LootBegin();
                break;
            case ShipEndBattle.Surrender:
                LootBegin();
                break;
        }
    }

    private void LootBegin()
    {
        _lootAfterBattle.SetShipToLoot(_baseShipEnemy);
        _lootAfterBattle.LootBegin();
    }

    private void OnPlayerEndBattleEvent(ShipEndBattle end)
    {
        FinishBattle();
        if (end == ShipEndBattle.Retreat)
        {
            Destroy(_baseShipEnemy.gameObject);
            Patrol(40);
        }
        else
        {
            Debug.Log("GameOver");
        }
    }

    private void FinishBattle()
    {
        Unsub();
        Debug.Log("Finish Battle");
        _baseShipPlayer.FinishBattle();
        _baseShipEnemy.FinishBattle();
    }

    private BaseShip GetRandomShip()
    {
        BaseShip enemyShip = EnemyShipPrefabs.GetRandomShipPrefab();
        return enemyShip;
    }

    private void OnBackToPatrol()
    {
        _menuButtons.RemoveButtons();
        _menuButtons.HideMenu();
        Patrol(33);
    }
    
    private void Patrol(int shipyardChance = 0)
    {
        var firstShip = GetRandomShip();
        _menuButtons.AddButton($"Approach {firstShip.gameObject.name}", () => OnBattleWithShip(firstShip));

        var secondShip = GetRandomShip();
        _menuButtons.AddButton($"Approach unidentified ship", () => OnBattleWithShip(secondShip));

        // FOR TEST
        shipyardChance = shipyardAlways? 100 : shipyardChance;
        bool shipyard = Random.Range(0, 100) < shipyardChance;

        if (shipyard)
        {
            var shipyardOption = new Shipyard(_baseShipPlayer, _menuButtons, () => Patrol(0));
            _menuButtons.AddButton($"Back to shipyard", shipyardOption.ShowShipOptionsShipyard);
        }

        if (shipyard)
        {
            var shipyardOption = new SosSignalEvent(_baseShipPlayer, _menuButtons, () => Patrol(0));
            _menuButtons.AddButton($"Listen radio waves", shipyardOption.ShowEventsOptions);
        }

        string menuInfo = "The ship went out to sea on patrol. You see enemy ships sailing in your direction. \n";
        menuInfo += shipyard ? "You can retreat to ally shipyard" : "";
        _menuButtons.ShowMenu(menuInfo);
    }

    private void OnBattleWithShip(BaseShip ship)
    {
        _menuButtons.RemoveButtons();
        _menuButtons.HideMenu();
        ShipBattleInitASync(ship).Forget();
    }

    private async UniTask ShipBattleInitASync(BaseShip enemyShip)
    {
        _baseShipEnemy = Instantiate(enemyShip, new Vector3(-3, 3, 0), Quaternion.identity);
        _baseShipPlayer.EndBattleEvent += OnPlayerEndBattleEvent;
        _baseShipEnemy.EndBattleEvent += EnemyEndBattleEvent;
        await _baseShipEnemy.Setup();
        _baseShipEnemy.SetEnemy(_baseShipPlayer);
        _baseShipPlayer.SetEnemy(_baseShipEnemy);
    }
    private void OnDestroy()
    {
        _shipSelect.ShipSelected -= ShipSelected;
        _lootAfterBattle.LeaveLoot -= OnLeaveLoot;
        Unsub();
    }
}