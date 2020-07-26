using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Functional
{
    // ====== TODO ====== STUB ======
    // This is a stub class.
    // It performs as expected, except that there is no performance gain from using it.
    // ======

    public static class Hash2DVectorPool
    {
        public static uint[] Rent()
        {
            return Hash2DVectorFactory.Create();
        }

        public static void Return(uint[] _)
        { 
        }
    }
}
