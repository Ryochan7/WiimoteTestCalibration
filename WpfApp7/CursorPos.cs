using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp7
{
    public class CursorPos
    {
        public float medianX;
        public float medianY;

        public bool OutOfReach;
        public bool fourPointsFound;

        public CursorPos(float medianX, float medianY, bool outOfReach)
        {
            this.medianX = medianX;
            this.medianY = medianY;

            this.OutOfReach = outOfReach;
            this.fourPointsFound = false;
        }
    }
}
