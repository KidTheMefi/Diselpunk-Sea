using DefaultNamespace;
using UnityEngine;

namespace ShipSinkingScripts
{
    public class HoleFactory : MonoBehaviour
    {
        private static HoleFactory _instance;
        public static HoleFactory Instance => _instance;
        
        [SerializeField]
        private HoleInHull _holePrefab;
        private ObjectPool<HoleInHull> _characterPool;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _characterPool = new ObjectPool<HoleInHull>(InstantiateHole, TurnHoleOn, TurnHoleOff, 10, true);
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        
        private HoleInHull InstantiateHole()
        {
            var hole = Instantiate(_holePrefab, transform);
            hole.transform.position = Vector3.up*20;
            hole.RemoveEvent += BackToPool;
            return hole;
        }
        
        private void TurnHoleOff(HoleInHull bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(false);
        }
        private void TurnHoleOn(HoleInHull bombshellProjectile)
        {
            bombshellProjectile.gameObject.SetActive(true);
        }

        public HoleInHull CreateHoleInHull()
        {
           return _characterPool.GetObject();
        }
        
        private void BackToPool(HoleInHull holeInHull)
        {
            if (holeInHull == null)
            {
                return;
            }
            holeInHull.transform.SetParent(transform);
            holeInHull.transform.position = Vector3.up*20;
            _characterPool.ReturnObject(holeInHull);
        }
    }
}