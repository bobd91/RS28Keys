using System;
using System.Collections.Generic;
using System.Text;

namespace RS28Keys
{
    public class FootpedalStatus
    {
        public FootpedalStatus() : this(false, false, false)
        { 
        }

        public FootpedalStatus(bool leftDown, bool rightDown, bool middleDown)
        {
            IsLeftDown = leftDown;
            IsRightDown = rightDown;
            IsMiddleDown = middleDown;
        }

        public bool IsLeftDown { get; }
        public bool IsRightDown { get; }
        public bool IsMiddleDown { get; }
    }
}
