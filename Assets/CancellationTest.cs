using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CancellationTest : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;

    [SerializeField]
    private TextMeshPro _timerText;
    [SerializeField]
    private TextMeshPro _amotherText;

    [SerializeField]
    private TextMeshPro _realTimeText;

    public void CancelCTS()
    {
        _cancellationTokenSource?.Cancel();

        //_cancellationTokenSource?.Dispose();
    }

    public void StartTimer()
    {
        CancelCTS();
        _cancellationTokenSource = new CancellationTokenSource();
        TimerAsync("timeDeltaTime:", 15f, _cancellationTokenSource.Token).Forget();
        TimerWithStepAsync("timeStep:", 15f, 0.01f, _cancellationTokenSource.Token).Forget();
        RealTimerAsync(15f, _cancellationTokenSource.Token).Forget();
    }


    private async UniTask RealTimerAsync(float time, CancellationToken token)
    {
        _realTimeText.text = "Timer work";
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: token);
        _realTimeText.text = "Done";
        await UniTask.Yield();
    }

    private async UniTask TimerAsync(string timerName, float time, CancellationToken token)
    {
        _timerText.gameObject.SetActive(true);
        var timerStep = Time.deltaTime;
        time = time < 0 ? 0 : time;

        string timeS = $"{time:f1}";

        await UniTask.WaitUntil(() =>
        {
            timerStep = Time.deltaTime;
            _timerText.text = $"{timerName}: {time:f1}/{timeS} sec.";
            time -= timerStep;

            return time <= 0 || token.IsCancellationRequested;
        });
        
        if (token.IsCancellationRequested)
        {
            _timerText.text = "Cancel!";
            return;
        }
        _timerText.text = "Done!";

        //_timerText.gameObject.SetActive(false);
    }

    private async UniTask TimerWithStepAsync(string timerName, float time, float step, CancellationToken token)
    {
        _amotherText.gameObject.SetActive(true);
        var timerStep = step > 0 ? step : 0.01f;
        time = time < 0 ? 0 : time;

        string timeS = $"{time:f1}";

        while (time > 0)
        {
            _amotherText.text = $"{timerName}: {time:f1}/{timeS} sec.";
            await UniTask.Delay(TimeSpan.FromSeconds(timerStep) /*, cancellationToken: token*/);
            time -= timerStep;
            if (token.IsCancellationRequested)
            {
                break;
            }
        }
        _amotherText.text = "";
        _amotherText.gameObject.SetActive(false);
    }



}