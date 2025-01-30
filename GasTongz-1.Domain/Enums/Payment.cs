using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Enums
{
    public enum PaymentMethod
    {
        Cash = 0,
        TNG = 1,
        QR = 2
    }
    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
