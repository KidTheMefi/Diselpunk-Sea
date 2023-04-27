using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShipCharacteristics;
using TMPro;
using UnityEngine;

public class RetreatHandler : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _retreatLockSprite;
    [SerializeField]
    private SpriteRenderer _retreatBarSprite;
    [SerializeField]
    private TextMeshPro _textMeshPro;
    [SerializeField]
    private TextMeshPro _retreatChangeText;

    private int valueChange;
    private bool _retreatOn;
    private SpeedHandler _thisShipSpeed;
    private SpeedHandler _enemyShipSpeed;

    private IntValue _retreatValue = new IntValue(30);
    private CancellationTokenSource _cancellationTokenSource;
    private DateTime _startTime;

    private void OnMouseDown()
    {
        StartRetreat();
    }

    public void StartRetreat()
    {
        if (!_retreatOn)
        {
            _startTime = DateTime.Now;
            _retreatChangeText.enabled = true;
            RecalculateRetreatChangeValue();
            _retreatLockSprite.enabled = false;
            _textMeshPro.color = Color.yellow;
            RetreatAsync().Forget();
            _retreatOn = true;
        }
    }

    public void Setup(SpeedHandler thisShip, SpeedHandler enemyShip, bool isButton)
    {
        _thisShipSpeed = thisShip;
        _enemyShipSpeed = enemyShip;
        _thisShipSpeed.SpeedChange += RecalculateRetreatChangeValue;
        _enemyShipSpeed.SpeedChange += RecalculateRetreatChangeValue;
        _retreatValue.SetValueTo(0);
        _textMeshPro.color = isButton ? Color.cyan : Color.clear;
        UpdateBar();
    }



    private async UniTask RetreatAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        while (!_retreatValue.FullValue)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: _cancellationTokenSource.Token);
            _retreatValue.ChangeValueFor(valueChange);
            UpdateBar();
        }
        ShipRetreated();
    }
    private void RecalculateRetreatChangeValue()
    {
        valueChange = 2 + _thisShipSpeed.Speed - _enemyShipSpeed.Speed;
        _retreatChangeText.text = valueChange.ToString();
    }
    private void UpdateBar()
    {
        _retreatBarSprite.size = new Vector2(_retreatValue.CurrentValue, _retreatBarSprite.size.y);
    }

    private void ShipRetreated()
    {
        var time = DateTime.Now - _startTime;
        Debug.Log($"Retreated after: {time.TotalSeconds} sec");
    }

    private void OnDestroy()
    {
        _thisShipSpeed.SpeedChange -= RecalculateRetreatChangeValue;
        _enemyShipSpeed.SpeedChange -= RecalculateRetreatChangeValue;
        _cancellationTokenSource?.Cancel();
    }
}