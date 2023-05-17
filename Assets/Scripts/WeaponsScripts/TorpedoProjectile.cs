using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class TorpedoProjectile : MonoBehaviour
    {
        public event Action<TorpedoProjectile> RemoveEvent = delegate(TorpedoProjectile torpedoProjectile) { };

        [SerializeField]
        private GameObject _torpedoObject;
        [SerializeField]
        private SpriteRenderer _detectedSpriteRenderer;
        [SerializeField]
        private Transform _rotationTransform;
        [SerializeField]
        private ParticleSystem _explosion;
        [SerializeField]
        private ParticleSystem _failed;
        [SerializeField]
        private ParticleSystem _trail;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        float _percentPerManeuverability = 5f;
        [SerializeField, Range(0.5f, 2f)]
        private float _checkTime;
        private float _fullMoveTime = 10f;

        private CancellationTokenSource _movementCTS;
        private BaseShip _targetShip;
        private ShipModulePlace _targetPlace;
        private Torpedo _torpedo;
        private DateTime _startTime;
        private int _detectionRound;
        private bool _byEnemy;

        public void Launch(Torpedo torpedo, BaseShip target)
        {
            _byEnemy = !target.ControlledByPlayer;
            _detectedSpriteRenderer.enabled = false;
            _rotationTransform.gameObject.SetActive(_byEnemy);
            _targetShip = target;
            _torpedo = torpedo;
            _movementCTS = new CancellationTokenSource();
            _detectionRound = 0;
            _textMeshPro.transform.localPosition = Vector3.zero;
            _targetPlace = _targetShip.Modules.LowerDeckPlaces[Random.Range(0, _targetShip.Modules.LowerDeckPlaces.Length)];
            TorpedoMovement().Forget();
        }


        private async UniTask TorpedoMovement()
        {
            _startTime = DateTime.Now;
            var torpedoTransform = transform;
            torpedoTransform.position += new Vector3(Random.Range(-0.65f, 0.65f), 0);
            var position = torpedoTransform.position;
            var startPosition = position;
            var targetPosition = _targetPlace.transform.position
                + new Vector3(Random.Range(-0.65f, 0.65f), 0);
            var direction = targetPosition - position;
            var quaternion = Quaternion.FromToRotation(Vector3.right, direction);
            _rotationTransform.rotation = quaternion;

            _torpedoObject.SetActive(true);

            var fullMoveTime = _fullMoveTime - _torpedo.Speed * _checkTime;
            int checkRound = 0;
            for (float i = 0; i < fullMoveTime; i += _checkTime)
            {
                checkRound++;
                float time = 0;
                await UniTask.WaitUntil(() =>
                {
                    if (_movementCTS.IsCancellationRequested)
                    {
                        return true;
                    }
                    time += Time.deltaTime;
                    position = Vector3.Lerp(startPosition, targetPosition, (i + time) / fullMoveTime);
                    torpedoTransform.position = position;
                    return time > _checkTime;
                });

                if (_movementCTS.IsCancellationRequested)
                {
                    RemoveEvent.Invoke(this);
                    return;
                }
                ;
                DetectionCheck(checkRound);

                //var timeCheck = DateTime.Now - _startTime;
                //Debug.Log($"Torpedo checked target at: {timeCheck.TotalSeconds}");
            }
            _torpedoObject.SetActive(false);
            _detectedSpriteRenderer.enabled = false;
            var timeSecond = DateTime.Now - _startTime;
            Debug.Log($"Torpedo reach target at: {timeSecond.TotalSeconds}");
            HitCheck();
        }


        private void HitCheck()
        {
            var missChance = _detectionRound == 0 ? _targetShip.ManeuverabilityValue() : _targetShip.ManeuverabilityValue() + (_fullMoveTime / _checkTime - _detectionRound);

            // !!! TODO: Think about Detection formula
            missChance *= _percentPerManeuverability;
            var random = Random.Range(0, 100);

            var missed = random < missChance ? "MISSED" : "";
            Debug.Log($"Miss chance {random}/{missChance}%. {missed}");
            if (random < missChance)
            {
                FailAsync("Missed").Forget();
            }
            else
            {
                TorpedoContact();
            }
        }

        private void TorpedoContact()
        {
            Debug.Log("Contact");
            int damage = _torpedo.Damage - _targetPlace.Armor;

            if (damage > 0)
            {
                _targetPlace.DamageDurability(damage);
                _targetPlace.DamageCrew(damage);
                _targetShip.DamageRecoverAbility(damage);
                ExplosionVisual().Forget();
            }
            else
            {
                FailAsync("Armor protect").Forget();
            }
           
        }

        private async UniTask FailAsync(string text = null)
        {
            _failed.Play();
            await ShowTextAsync(text);
        }

        private async UniTask ShowTextAsync(string text)
        {
            _textMeshPro.gameObject.SetActive(true);
            _textMeshPro.text = text;
            var textMeshTransform = _textMeshPro.transform;
            float time = 1f;
            await UniTask.WaitUntil(() =>
            {

                textMeshTransform.localPosition += 0.5f * Vector3.up * Time.deltaTime;
                time -= Time.deltaTime;
                return time < 0.1;
            }, cancellationToken: _movementCTS.Token);

            _textMeshPro.gameObject.SetActive(false);
            RemoveEvent.Invoke(this);
        }



        private void DetectionCheck(int round)
        {
            if (_detectionRound > 0)
            {
                return;
            }
            int sonarValue = _torpedo.SonarValue;
            var detectionChance = (_torpedo.Noise + sonarValue * 2 + round) * 2; // TODO: Think about Detection formula

            var random = Random.Range(0, 100);


            if (random < detectionChance)
            {
                //  var detected = random < detectionChance ? "DETECTED" : "";
                //Debug.Log($"{detectionChance}%) Chance {random}/{detectionChance}. (. On round {round}. + {detected}");
                _detectionRound = round;
                TorpedoDetected();
            }
        }

        private void TorpedoDetected()
        {
            _rotationTransform.gameObject.SetActive(true);
            _detectedSpriteRenderer.enabled = true;
        }



        private async UniTask ExplosionVisual()
        {
            _explosion.Play();
            await UniTask.WaitUntil(() => _explosion.isStopped, cancellationToken: _movementCTS.Token);
            RemoveEvent.Invoke(this);
        }

        private void OnDestroy()
        {
            _movementCTS?.Cancel();
        }
    }
}