using UnityEngine;

namespace DefaultNamespace
{
    public class ArtilleryVolleyFactory : MonoBehaviour
    {
        
        private static ArtilleryVolleyFactory _instance;
        public static ArtilleryVolleyFactory Instance => _instance;
        
        [SerializeField]
        private Bombshell bombshellPrefab;
        private ObjectPool<Bombshell> _characterPool;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _characterPool = new ObjectPool<Bombshell>(InstantiateBombshell, TurnBombshellOn, TurnBombshellOff, 20, true);
            }
            else
            {
                Destroy(this);
            }
        }
        
        private Bombshell InstantiateBombshell()
        {
            var bombshell = Instantiate(bombshellPrefab, transform);
            bombshell.transform.position = Vector3.up*20;
            bombshell.RemoveEvent += BackToPool;
            return bombshell;
        }
        
        private void TurnBombshellOff(Bombshell bombshell)
        {
            bombshell.gameObject.SetActive(false);
        }
        private void TurnBombshellOn(Bombshell bombshell)
        {
            bombshell.gameObject.SetActive(true);
        }

        public void FireBombshell(Shell shell, ShipModulePlace target, Vector3 startPosition, bool miss = false)
        {
            var bombshell = _characterPool.GetObject();
            bombshell.transform.position = startPosition;
            bombshell.Fire(shell, target, miss);
        }
        
        private void BackToPool(Bombshell bombshell)
        {
            if (bombshell == null)
            {
                return;
            }
            bombshell.transform.SetParent(transform);
            bombshell.transform.position = Vector3.up*20;
            _characterPool.ReturnObject(bombshell);
        }
        
    }
}