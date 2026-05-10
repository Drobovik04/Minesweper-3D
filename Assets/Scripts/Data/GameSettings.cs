using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GameSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }
        public int Bombs { get; set; }
    }
}
