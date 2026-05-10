using Assets.Scripts.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class FieldService
    {
        private CellData[,,] _field;
        private int _size;
        public int TotalMines { get; private set; }

        public void Generate(int size, int mines)
        {
            TotalMines = mines;
            _size = size;
            _field = new CellData[size, size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    for (int z = 0; z < size; z++)
                        _field[x, y, z] = new CellData();

            PlaceMines(mines);

            CalculateNumbers();
        }

        public CellData Get(int x, int y, int z) => _field[x, y, z];

        public void Open(int x, int y, int z)
        {
            var startCell = _field[x, y, z];
            if (startCell.IsOpened || startCell.IsMine) return;

            var stack = new Stack<Vector3Int>();
            stack.Push(new Vector3Int(x, y, z));

            while (stack.Count > 0)
            {
                var pos = stack.Pop();
                var cell = _field[pos.x, pos.y, pos.z];

                if (cell.IsOpened) continue;
                cell.IsOpened = true;

                if (cell.AdjacentMines == 0)
                {
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                            for (int dz = -1; dz <= 1; dz++)
                            {
                                if (dx == 0 && dy == 0 && dz == 0) continue;

                                int nx = pos.x + dx, ny = pos.y + dy, nz = pos.z + dz;

                                if (nx >= 0 && nx < _size && ny >= 0 && ny < _size && nz >= 0 && nz < _size)
                                {
                                    if (!_field[nx, ny, nz].IsOpened)
                                        stack.Push(new Vector3Int(nx, ny, nz));
                                }
                            }
                }
            }
        }

        private List<Vector3Int> GetNeighbors(int x, int y, int z)
        {
            List<Vector3Int> neigbours = new();

            for (int i = Math.Clamp(x - 1, 0, _size); i <= Math.Clamp(x + 1, 0, _size - 1); i++)
            {
                for (int j = Math.Clamp(y - 1, 0, _size); j <= Math.Clamp(y + 1, 0, _size - 1); j++)
                {
                    for (int k = Math.Clamp(z - 1, 0, _size); k <= Math.Clamp(z + 1, 0, _size - 1); k++)
                    {
                        if (!(x == i && y == j && z == k))
                        {
                            neigbours.Add(new Vector3Int(i, j, k));
                        }
                    }
                }
            }

            return neigbours;
        }
        private void PlaceMines(int mines)
        {
            var rnd = new Unity.Mathematics.Random((uint) DateTime.Now.Ticks % 100000);

            while(mines > 0)
            {
                var x = rnd.NextInt(0, _size);
                var y = rnd.NextInt(0, _size);
                var z = rnd.NextInt(0, _size);

                var cell = _field[x, y, z];

                if (cell.IsMine)
                {
                    continue;
                }
                else
                {
                    cell.IsMine = true;
                    mines--;
                }
            }
        }
        private void CalculateNumbers()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    for (int k = 0; k < _size; k++)
                    {
                        CalculateAdjCount(i, j, k);
                    }
                }
            }
        }
        private void CalculateAdjCount(int x, int y, int z)
        {
            var count = 0;

            for (int i = Math.Clamp(x - 1, 0, _size - 1); i <= Math.Clamp(x + 1, 0, _size - 1); i++)
            {
                for (int j = Math.Clamp(y - 1, 0, _size - 1); j <= Math.Clamp(y + 1, 0, _size - 1); j++)
                {
                    for (int k = Math.Clamp(z - 1, 0, _size - 1); k <= Math.Clamp(z + 1, 0, _size - 1); k++)
                    {
                        if (!(x == i && y == j && z == k))
                        {
                            if (_field[i, j, k].IsMine)
                                count++;
                        }
                    }
                }
            }

            _field[x, y, z].AdjacentMines = count;
        }
    }
}
