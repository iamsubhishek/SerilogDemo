using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NewSerilogApplication.Interface
{
    public interface ILog<T> 
    {
        void TraceMessage([CallerMemberName] string memberName = "");
        void TraceError(Exception ex, [CallerMemberName] string memberName = "");
    }
}
