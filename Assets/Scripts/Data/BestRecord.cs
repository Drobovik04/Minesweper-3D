using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BestRecord
    {
        public int FieldSize;
        public int MinesCount;
        public float ClearTime;
        public long Timestamp;

        public string FormattedTime =>
            ClearTime < 60 ? $"{ClearTime:F1}с" :
            $"{Mathf.FloorToInt(ClearTime / 60)}:{Mathf.FloorToInt(ClearTime % 60):D2}";

        public string DateString =>
            DateTimeOffset.FromUnixTimeSeconds(Timestamp).ToString("dd.MM.yyyy");
    }
}
