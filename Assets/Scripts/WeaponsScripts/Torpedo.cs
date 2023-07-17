using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Torpedo")]
    public class Torpedo : ScriptableObject
    {
        [SerializeField]
        private int damage;
        [SerializeField, Range(1,5)]
        private int speed;
        [SerializeField, Range(1,5)]
        private int noise;
        
        public int Damage => damage;
        public int Speed => speed;
        public int Noise => noise;
    }
}