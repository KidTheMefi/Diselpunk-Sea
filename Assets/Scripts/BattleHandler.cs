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
    private MenuButtons _menuButtons;

    private Shipyard _shipyard;
    private LootAfterBattle _lootAfterBattle;

    [SerializeField]
    private bool shipyardAlways;
    
    private void Start()
    {
        _lootAfterBattle = new LootAfterBattle(_baseShipPlayer, _menuButtons);
        _lootAfterBattle.LeaveLoot += OnLeaveLoot;
        StartAsync().Forget();
    }

    private void OnLeaveLoot()
    {
        Patrol(35);
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

    private void Patrol(int shipyardChance = 0)
    {
        Debug.Log("Patrol");
        var firstShip = GetRandomShip();
        _menuButtons.AddButton($"Approach {firstShip.gameObject.name}", () =>ShipBattle(firstShip).Forget());
        
        var secondShip = GetRandomShip();
        _menuButtons.AddButton($"Approach {secondShip.gameObject.name}", () =>ShipBattle(secondShip).Forget());

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
    
    private async UniTask ShipBattle(BaseShip enemyShip)
    {
        _baseShipEnemy = Instantiate(enemyShip, new Vector3(-3,3,0), Quaternion.identity);
        _baseShipPlayer.EndBattleEvent += OnPlayerEndBattleEvent;
        _baseShipEnemy.EndBattleEvent += EnemyEndBattleEvent;
        await _baseShipEnemy.Setup();
        _baseShipEnemy.SetEnemy(_baseShipPlayer);
        _baseShipPlayer.SetEnemy(_baseShipEnemy);
    }
    private void OnDestroy()
    {
        //_shipyard.LeaveShipYard -= OnLeaveShipYard;
        _lootAfterBattle.LeaveLoot -= OnLeaveLoot;
        Unsub();
    }
}