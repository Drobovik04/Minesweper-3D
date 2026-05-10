using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class AllData
    {
        public GameSettings GameSettings { get; set; }
        public BestRecord BestRecord;

        public AllData()
        {
            GameSettings = new GameSettings();
            BestRecord = new BestRecord();
        }
    }
}
