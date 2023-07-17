using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Fire : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;
    private ShipModulePlace _shipModulePlace ;
    [SerializeField]
    private SpriteRenderer _fireRenderer;
    [SerializeField]
    private SpriteRenderer _fireLevelSpriteRenderer;

    private int _fireLvl;
    private bool _isOnFire = false;
    public bool IsOnFire => _isOnFire;
    readonly System.Random _random = new System.Random();

    public void Setup(ShipModulePlace shipModulePlace)
    {
        gameObject.SetActive(false);
        _shipModulePlace = shipModulePlace;
    }


    private void ShowFireLevel(int lvl)
    {
        if (_fireLevelSpriteRenderer == null)
        {
            return;
        }
        _fireLevelSpriteRenderer.size = new Vector2(_fireLevelSpriteRenderer.size.x, 0.5f + lvl * 2.5f / 100);
    }
    
    
    public void AddFire(int fireLvl)
    {
        if (_isOnFire)
        {
            _fireLvl += fireLvl * 10;
            
        }
        else
        {
            _isOnFire = true;
            gameObject.SetActive(true);
            _fireLvl = fireLvl * 10;
           
            _cancellationTokenSource = new CancellationTokenSource();
            OnFireAsync(_cancellationTokenSource.Token).Forget();
        }
        ShowFireLevel(_fireLvl);
    }

    private async UniTask OnFireAsync(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
        
        var randomBool = _random.Next(2) == 1;


        if (randomBool)
        {
            _shipModulePlace.DamageCrew(1);;
        }
        else
        {
            _shipModulePlace.DamageDurability(1);
        }
        FireVisual().Forget();
        FireValueChangePerSecond();
        
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            OnFireAsync(token).Forget();
        }
    }

    private async UniTask FireVisual()
    {
        _fireRenderer.color = Color.red;
        await UniTask.Delay(TimeSpan.FromSeconds(0.2), cancellationToken: _cancellationTokenSource.Token);
        _fireRenderer.color = Color.white;
    }

    private void FireValueChangePerSecond()
    {
        int fireChangeValue = 0;
        fireChangeValue -= _shipModulePlace.GetFireExtinguishing();
        _fireLvl += fireChangeValue;
        ShowFireLevel(_fireLvl);
        if (_fireLvl <= 0)
        {
            FireEnd();
        }
    }

    private void FireEnd()
    {
        _cancellationTokenSource?.Cancel();
        _fireLvl = 0;
        gameObject.SetActive(false);
        _isOnFire = false;
        _fireRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }
}
