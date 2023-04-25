using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace CrewCommanders
{
    public class Commander : MonoBehaviour
    {
        public event Action<Commander> OnCommanderClick = delegate(Commander place) { };
        
        
        [SerializeField, Range(0.1f , 12f)]
        private float _moveSpeed;
        [SerializeField, Range(1, 5)]
        private int _repairLevel;
        [SerializeField, Range(1, 5)]
        private int _battleLevel;
        [SerializeField, Range(1, 5)]
        private int _fireFightLevel;

        public int RepairLevel => _repairLevel;
        
        private ShipModulePlace _shipModulePlaceTarget;
        private ShipModulePlace _shipModulePlaceOnDuty;
        private CancellationTokenSource _movingCTS;

        [SerializeField]
        private SpriteRenderer _selectRenderer;
        private void OnMouseDown()
        {
            OnCommanderClick.Invoke(this);
            //CommanderSignal.GetInstance().InvokeCommanderClick(this);
        }


        public async UniTask MoveToPlaceAsync(ShipModulePlace shipModulePlace)
        {
            if (shipModulePlace.CommanderOnPost != null)
            {
                Debug.Log(shipModulePlace.CommanderOnPost != null);
                return;
            }

            if (_shipModulePlaceTarget != shipModulePlace)
            {
                GoFromPost();
                
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

        public void MoveFromPost( Vector3 moveTo)
        {
            GoFromPost();
            _movingCTS?.Cancel();
            _movingCTS = new CancellationTokenSource();
            MoveToAsync(moveTo, _movingCTS.Token).Forget();
        }
        
        private void GoFromPost()
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
            _shipModulePlaceOnDuty = _shipModulePlaceTarget;
            _shipModulePlaceOnDuty.SetCommander(this);
        }

        private async UniTask MoveToAsync(Vector3 pos, CancellationToken movingToken)
        {
            float t = 0;
            Vector3 moveTargetPosition = pos + Vector3.back*0.2f;
            float interpolation = 0;
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