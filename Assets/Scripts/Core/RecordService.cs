using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class RecordService
    {
        private readonly SaveManager _saveManager;
        private AllData _data;

        public RecordService(SaveManager saveManager)
        {
            _saveManager = saveManager;
            _data = _saveManager.Load();
        }

        public bool TryUpdateRecord(int fieldSize, int minesCount, float clearTime)
        {
            var current = _data.BestRecord;

            bool isBetter = false;

            if (fieldSize == current.FieldSize)
            {
                if (minesCount > current.MinesCount)
                    isBetter = true;
                else if (minesCount == current.MinesCount && clearTime < current.ClearTime)
                    isBetter = true;
            }
            else if (fieldSize > current.FieldSize)
            {
                isBetter = true;
            }

            if (isBetter)
            {
                current.FieldSize = fieldSize;
                current.MinesCount = minesCount;
                current.ClearTime = clearTime;
                current.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                _saveManager.Save(_data);
                Debug.Log($"New record! {fieldSize}³, {minesCount} mines, {clearTime:F1}s");
                return true;
            }

            return false;
        }

        public string GetRecordDisplay()
        {
            var r = _data.BestRecord;
            if (r.MinesCount == 0) return YG.YG2.lang == "en" ? "No record" : "Нет рекорда";

            return YG.YG2.lang == "en" ? $"Fastest record: {r.FieldSize}³ | {r.MinesCount} mines | {r.FormattedTime} | {r.DateString}"
                : $"Самый быстрый рекорд: {r.FieldSize}³ | {r.MinesCount} мин | {r.FormattedTime} | {r.DateString}";
        }

        public BestRecord GetCurrentRecord() => _data.BestRecord;

        public void ResetRecord()
        {
            _data.BestRecord = new BestRecord();
            _saveManager.Save(_data);
        }
    }
}
