using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Bombshell : MonoBehaviour
    {
        public event Action<Bombshell> RemoveEvent = delegate(Bombshell bombshell) { };
        private CancellationTokenSource _movementCTS;
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private GameObject _shellObject;
        [SerializeField]
        private ParticleSystem _explosion;
        [SerializeField]
        private ParticleSystem _failed;
        [SerializeField]
        private ParticleSystem _trail;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        private ShipModulePlace _target;
        private Shell _shell;
        
        

        public void Fire(Shell shell, ShipModulePlace target, bool miss = false)
        {
            
            _textMeshPro.gameObject.SetActive(false);
            _textMeshPro.transform.localPosition = Vector3.zero;
            var main = _trail.main;
            main.startColor = shell.ShellType switch
            {
                ShellType.Common => Color.white,
                ShellType.Shrapnel => Color.grey,
                ShellType.ArmorPiercing => Color.cyan,
                ShellType.HighExplosive => Color.yellow,
                _ => main.startColor
            };

            _shellObject.SetActive(true);
            _target = target;
            _shell = shell;
            _movementCTS = new CancellationTokenSource();
            BombshellFlight(miss).Forget();
        }

        private async UniTask BombshellFlight(bool miss = false)
        {
            var targetPosition = _target.transform.position + new Vector3(Random.Range(-0.65f,0.65f), Random.Range(-0.3f,0.3f));
            var bombshellTransform = transform;
            float interpolation = 0;
            var position = bombshellTransform.position;

            var direction = targetPosition - position;
            //targetPosition += miss ? direction : Vector3.zero;
            direction = direction.normalized;
            await UniTask.WaitUntil(() =>
            {
                if (_movementCTS.IsCancellationRequested)
                {
                    return true;
                }
                interpolation += _moveSpeed * Time.deltaTime;

                //position = Vector3.Lerp(position, targetPosition, interpolation);
                position += direction * interpolation;
                bombshellTransform.position = position;
                return Mathf.Abs((position - targetPosition).sqrMagnitude) < 0.1;
            });
            
            
            if (_movementCTS.IsCancellationRequested)
            {
                RemoveEvent.Invoke(this);
                return;
            };
            _shellObject.SetActive(false);
            if (miss)
            {
                ShowTextAsync("Miss").Forget();
                return;
            }
            BombshellContact();
        }

        private async UniTask ShowTextAsync(string text)
        {
            _textMeshPro.gameObject.SetActive(true);
            _textMeshPro.text = text;
            var textMeshTransform = _textMeshPro.transform;
            float time =1f;
            await UniTask.WaitUntil(() =>
            {
                
                textMeshTransform.localPosition += 0.5f*Vector3.up*_moveSpeed * Time.deltaTime;
                time -= Time.deltaTime;
                return time < 0.1;
            }, cancellationToken: _movementCTS.Token);
            
            _textMeshPro.gameObject.SetActive(false);
            RemoveEvent.Invoke(this);
        }
        
        private void BombshellContact()
        {
            switch (_shell.ShellType)
            {
                case ShellType.Common:
                    CommonContact();
                    return;
                case ShellType.ArmorPiercing:
                    ArmourPiercingContact();
                    return;
                case ShellType.HighExplosive:
                    HighExplosionContact();
                    return;
                case ShellType.Shrapnel:
                    ShrapnelContact();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        private bool PenetrationSuccess()
        {
            var penetrationChance = 100 -  (_target.Armor - _shell.ArmorPiercingClass) * 25;
            return Random.Range(0, 100) < penetrationChance;
        }
        
        private void CommonContact()
        {
            if (PenetrationSuccess())
            {
                _target.DamageDurability(_shell.BaseDamage);
                _target.DamageCrew(Random.Range(0, _shell.BaseCrewDamage+1));
                ExplosionVisual().Forget();
            }
            else
            {
                FailAsync("Armor").Forget();
            }
        }
        
        
        private void ArmourPiercingContact()
        {
            if (_target.ModuleLocation == ModuleLocation.DeckHouse && !PenetrationSuccess())
            {
                FailAsync("Went through").Forget();
            }
            else
            {
                CommonContact();
            }
        }

        private void HighExplosionContact()
        {
            var success = PenetrationSuccess();
            
            if (_target.ModuleLocation == ModuleLocation.LowerDeck && !success)
            {
                FailAsync("Armor").Forget();
                return;
            }
            
            if (success)
            {
                _target.DamageDurability(_shell.BaseDamage);
                _target.DamageCrew(Random.Range(0, _shell.BaseCrewDamage+1));
            }
            
            ExplosionVisual().Forget();
            //TODO: FIRE
            _target.AddFire(_shell.BaseDamage);
        }

        private void ShrapnelContact()
        {
            var success = PenetrationSuccess();
            
            if (_target.ModuleLocation == ModuleLocation.LowerDeck && !success)
            {
                FailAsync("Armor").Forget();
                return;
            }
            
            if (success)
            {
                _target.DamageDurability(_shell.BaseDamage);
                _target.DamageCrew(_shell.BaseCrewDamage);
                ExplosionVisual().Forget();
            }
            else
            {
                _target.DamageCrew(Random.Range(0, _shell.BaseCrewDamage+1));
                FailAsync("Superficial damage").Forget();
            }
            
        }

        private async UniTask FailAsync(string text = null)
        {
            _failed.Play();
            await ShowTextAsync(text);
            //Debug.Log("Failed. " + text);
            //await UniTask.WaitUntil(() => _failed.isStopped, cancellationToken: _movementCTS.Token);
            //RemoveEvent.Invoke(this);
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