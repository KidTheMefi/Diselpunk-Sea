using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CrewCommanders
{
    public class Commander : MonoBehaviour
    {
        public event Action<Commander> OnCommanderClick = delegate(Commander place) { };
        public event Action<Commander> ReturnedTo = delegate(Commander place) { };
        
        [SerializeField, Range(0.1f , 12f)]
        private float _moveSpeed;
        [SerializeField, Range(1, 5)]
        private int _repairLevel;
        [SerializeField, Range(1, 5)]
        private int _battleLevel;
        [SerializeField, Range(1, 5)]
        private int _fireFightLevel;
        [SerializeField]
        private ValueVisualView _valueVisualView;
        
        private IntValue _hpValue = new IntValue(3);
        private Vector3 _defaultPosition;
        public int RepairLevel => _repairLevel;
        
        private ShipModulePlace _shipModulePlaceTarget;
        private ShipModulePlace _shipModulePlaceOnDuty;
        private CancellationTokenSource _movingCTS;
        public bool Moving { get; private set;}

        [SerializeField]
        private SpriteRenderer _selectRenderer;
        
        
        
        private void OnMouseDown()
        {
            if (Moving)
            {
                return;
            }
            
            OnCommanderClick.Invoke(this);
            //CommanderSignal.GetInstance().InvokeCommanderClick(this);
        }


        public void Damage()
        {
            _hpValue.ChangeValueFor(-1);
            _valueVisualView.UpdateVisualPoints(_hpValue);
            
            if (_hpValue.CurrentValue <= 0)
            {
                MoveBackToDefaultPosition().Forget();
            }
        }

        private void Start()
        {
            _defaultPosition = transform.position;
            _valueVisualView.UpdateVisualPoints(_hpValue);
        }
        
        public async UniTask MoveToPlaceAsync(ShipModulePlace shipModulePlace)
        {
            if (shipModulePlace.CommanderOnPost != null || Moving)
            {
                return;
            }

            if (_shipModulePlaceTarget != shipModulePlace)
            {
                LeavePost();
                
                _movingCTS?.Cancel();
                _movingCTS = new CancellationTokenSource();
                _shipModulePlaceTarget = shipModulePlace;
                var token = _movingCTS.Token;
                await MoveToAsync(_shipModulePlaceTarget.transform.position, token);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                OnDuty();
            }
        }

        public async UniTask MoveBackToDefaultPosition()
        {
            if (Moving)
            {
                return;
            }
            LeavePost();
            _movingCTS?.Cancel();
            _movingCTS = new CancellationTokenSource();
            await MoveToAsync(_defaultPosition, _movingCTS.Token);
            _hpValue.SetValueTo(_hpValue.MaxValue);
            _valueVisualView.UpdateVisualPoints(_hpValue);
            ReturnedTo.Invoke(this);
        }
        
        private void LeavePost()
        {
            _shipModulePlaceTarget = null;
            if (_shipModulePlaceOnDuty != null)
            {
                _shipModulePlaceOnDuty.RemoveCommander();
                _shipModulePlaceOnDuty = null;
            }
        }

        private void OnDuty()
        {
            if (_shipModulePlaceTarget.CommanderOnPost != null)
            {
                _shipModulePlaceTarget.CommanderOnPost.MoveBackToDefaultPosition().Forget();
            }
            _shipModulePlaceOnDuty = _shipModulePlaceTarget;
            _shipModulePlaceOnDuty.SetCommander(this);
            Moving = false;
        }

        private async UniTask MoveToAsync(Vector3 pos, CancellationToken movingToken)
        {
            Moving = true;
            float t = 0;
            Vector3 moveTargetPosition = pos + Vector3.back*0.2f;
            Vector3 moveStartPosition = transform.position;
            var direction = moveTargetPosition - moveStartPosition;
            var directionDistance = direction.magnitude;

            await UniTask.WaitUntil(() =>
            {
                //
                transform.position = Vector3.Lerp(moveStartPosition, moveTargetPosition, t);
                t += _moveSpeed*Time.deltaTime/directionDistance;
                return t >= 1 || movingToken.IsCancellationRequested;
                
               /* interpolation = _moveSpeed * Time.deltaTime;
                position += direction * interpolation;
                transform.position = position;
                return Mathf.Abs((position - moveTargetPosition).sqrMagnitude) < 0.1 || movingToken.IsCancellationRequested;*/
            }, cancellationToken: movingToken);
            Moving = false;
        }


        private void OnDestroy()
        {
            _movingCTS?.Cancel();
        }


        public void Select(bool value)
        {
            _selectRenderer.color = value ? Color.green : Color.white;
        }
    }
}