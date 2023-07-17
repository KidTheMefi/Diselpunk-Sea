using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ShipCharacteristics;
using TMPro;
using UnityEngine;

namespace ShipSinkingScripts
{
    public class ShipFloodabilityHandler : MonoBehaviour
    {
        public event Action EndBattle;
        
        [SerializeField]
        private ValueBarView _sinkingBar;
        [SerializeField]
        private int criticalWaterLvl;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField,Range(0,4)]
        private int _basePumpPower;
        
        private int _currentWaterLvl;
        private int _criticalWaterLvl;

        private int _pumpPower;
        
        private Dictionary<ShipModulePlace, HoleInHull> _hullsWithHole = new Dictionary<ShipModulePlace, HoleInHull>();
        private RepairSkillHandler _repairSkillHandler;
        private List<IPumpPowerProvider> _pumpPowerProviders = new List<IPumpPowerProvider>();
        private CancellationTokenSource _waterPumpingCTS;
        
        public void Setup(RepairSkillHandler repairSkillHandler, List<IPumpPowerProvider> pumpPowerProviders, Action endBattle)
        {
            EndBattle = endBattle;
            _criticalWaterLvl = criticalWaterLvl;
            _pumpPowerProviders = pumpPowerProviders;
            _sinkingBar.Setup(0,_criticalWaterLvl);
            _hullsWithHole = new Dictionary<ShipModulePlace, HoleInHull>();
            _repairSkillHandler = repairSkillHandler;

            foreach (var pumpPower in _pumpPowerProviders)
            {
                pumpPower.PumpPowerChanged += RecalculatePumpPower;
            }
            RecalculatePumpPower();
            
            _waterPumpingCTS?.Cancel();
            _waterPumpingCTS = new CancellationTokenSource();
            WaterPumping().Forget();
        }


        private async UniTask WaterPumping()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _waterPumpingCTS.Token);
            _currentWaterLvl -= _pumpPower;
            _currentWaterLvl = _currentWaterLvl > 0 ? _currentWaterLvl : 0;
            ChangeWaterLwl(_currentWaterLvl);
            
            if (_waterPumpingCTS.IsCancellationRequested)
            {
                return;
            }
            WaterPumping().Forget();
        }
        
        private void RecalculatePumpPower()
        {
            int pumpPower = _basePumpPower;
            foreach (var pump in _pumpPowerProviders)
            {
                pumpPower += pump.GetPumpPower();
            }
            _pumpPower = pumpPower;
            _textMeshPro.text = $"Current pump power {_pumpPower}";
        }
        
        public void CreateHole(ShipModulePlace shipModulePlace, int holeSize)
        {
            if (_hullsWithHole.TryGetValue(shipModulePlace, out var hole))
            {
                hole.IncreaseHoleSize(holeSize);
            }
            else
            {
                var newHole = HoleFactory.Instance.CreateHoleInHull();
                newHole.SinkingBegin(shipModulePlace, _repairSkillHandler, holeSize);
                newHole.HoleRepairedAction+= NewHoleOnHoleRepairedAction;
                newHole.WaterLevelIncrease+= NewHoleOnWaterLevelIncrease;
                _hullsWithHole.Add(shipModulePlace, newHole);
            }
        }

        private void ChangeWaterLwl(int value)
        {
            _sinkingBar.SetCurrentValue(value);

            if (value >= criticalWaterLvl)
            {
                EndBattle?.Invoke();
                EndBattle = null;
            }
        }
        
        private void NewHoleOnWaterLevelIncrease(int waterLvl)
        {
            _currentWaterLvl += waterLvl;
            ChangeWaterLwl(_currentWaterLvl);
        }
        
        private void NewHoleOnHoleRepairedAction(ShipModulePlace place)
        {
            if (_hullsWithHole.TryGetValue(place, out var hole))
            {
                hole.HoleRepairedAction-= NewHoleOnHoleRepairedAction;
                hole.WaterLevelIncrease-= NewHoleOnWaterLevelIncrease;
                hole.Remove();
                _hullsWithHole.Remove(place);
            }
        }

        private void OnDestroy()
        {
            EndBattle = null;
            foreach (var hole in _hullsWithHole.Values)
            {
                hole.HoleRepairedAction-= NewHoleOnHoleRepairedAction;
                hole.WaterLevelIncrease-= NewHoleOnWaterLevelIncrease;
            }

            foreach (var pumpPower in _pumpPowerProviders)
            {
                pumpPower.PumpPowerChanged -= RecalculatePumpPower;
            }
        }
    }
}