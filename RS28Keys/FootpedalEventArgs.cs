using System;
using System.Collections.Generic;
using System.Text;

namespace RS28Keys
{
    public class FootpedalEventArgs : EventArgs
    {
        public FootpedalEventArgs(FootpedalStatus status)
        {
            Status = status;
        }

        public FootpedalStatus Status { get; }
    }
}
