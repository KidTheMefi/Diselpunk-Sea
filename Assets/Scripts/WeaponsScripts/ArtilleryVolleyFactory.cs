using UnityEngine;

namespace DefaultNamespace
{
    public class ArtilleryVolleyFactory : MonoBehaviour
    {
        private static ArtilleryVolleyFactory _instance;
        public static ArtilleryVolleyFactory Instance => _instance;
        
        [SerializeField]
        private BombshellProjectile bombshellProjectilePrefab;
        private ObjectPool<BombshellProjectile> _characterPool;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _characterPool = new ObjectPool<BombshellProjectile>(InstantiateBombshell, TurnBombshellOn, TurnBombshellOff, 20, true);
            }
            else
            {
                Destroy(this);
            }
        }
        
        private BombshellProjectile InstantiateBombshell()
        {
            var bombshell = Instantiate(bombshellProjectilePrefab, transform);
            bombshell.transform.position = Vector3.up*20;
            bombshell.RemoveEvent += BackToPool;
            return bombshell;
        }
        
        private void TurnBombshellOff(BombshellProjectile bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(false);
        }
        private void TurnBombshellOn(BombshellProjectile bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(true);
        }

        public void FireBombshell(Shell shell, ShipModulePlace target, Vector3 startPosition, bool miss = false)
        {
            var bombshell = _characterPool.GetObject();
            bombshell.transform.position = startPosition;
            bombshell.Fire(shell, target, miss);
        }
        
        private void BackToPool(BombshellProjectile bombshellProjectile)
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