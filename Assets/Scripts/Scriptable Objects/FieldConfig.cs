using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "FieldConfig", menuName = "Scriptable Objects/FieldConfig")]
    public class FieldConfig : ScriptableObject
    {
        public int size;
        public int mines;
        public float gap;
    }
}


