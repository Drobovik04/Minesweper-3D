using Assets.Scripts.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        private CellView[,,] _views;

        public CellView[,,] Views => _views;

        public void Build(int size, float gap)
        {
            _views = new CellView[size, size, size];
            var sizeOfPrefab = _prefab.GetComponent<MeshRenderer>().bounds.size.x;
            var step = sizeOfPrefab + gap;

            float centerOffset = (size - 1) / 2f;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    for (int z = 0; z < size; z++)
                    {
                        var cell = Instantiate(_prefab, transform);

                        var cellView = cell.GetComponent<CellView>();


                        float posX = (x - centerOffset) * step;
                        float posY = (y - centerOffset) * step;
                        float posZ = (z - centerOffset) * step;

                        cellView.SetPosition(posX, posY, posZ);
                        cellView.SetIndexPosition(x, y, z);
                        _views[x, y, z] = cellView;
                    }
        }

        public void AnimateSpawn()
        {
            foreach (var cell in _views)
            {
                cell.PlaySpawn();
            }
        }

        public void Reveal(int x, int y, int z, CellData data)
        {
            _views[x, y, z].Reveal(data);
        }
    }
}
