using UnityEngine;

namespace DefaultNamespace
{
    public class TorpedoFactory : MonoBehaviour
    {
        private static TorpedoFactory _instance;
        public static TorpedoFactory Instance => _instance;
        
        [SerializeField]
        private TorpedoProjectile torpedoProjectilePrefab;
        private ObjectPool<TorpedoProjectile> _characterPool;

        private void Awake()
        {
            if (_instance == null)
            {
                
                _characterPool = new ObjectPool<TorpedoProjectile>(InstantiateTorpedoProjectile, TurnTorpedoOn, TurnTorpedoOff, 10, true);
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        
        private TorpedoProjectile InstantiateTorpedoProjectile()
        {
            var torpedo = Instantiate(torpedoProjectilePrefab, transform);
            torpedo.transform.position = Vector3.up*20;
            torpedo.RemoveEvent += BackToPool;
            return torpedo;
        }
        
        private void TurnTorpedoOff(TorpedoProjectile bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(false);
        }
        private void TurnTorpedoOn(TorpedoProjectile bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(true);
        }

        public void TorpedoLaunch(Torpedo torpedo, BaseShip target, Vector3 startPosition)
        {
            var torpedoProjectile = _characterPool.GetObject();
            torpedoProjectile.transform.position = startPosition;
            torpedoProjectile.Launch(torpedo, target);
        }
        
        private void BackToPool(TorpedoProjectile bombshellProjectile)
        {
            if (bombshellProjectile == null)
            {
                return;
            }
            bombshellProjectile.transform.SetParent(transform);
            bombshellProjectile.transform.position = Vector3.up*20;
            _characterPool.ReturnObject(bombshellProjectile);
        }
    }
}