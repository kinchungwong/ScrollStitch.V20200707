using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    public interface IDeconstruct<T1>
    {
        void Deconstruct(out T1 t1);
    }

    public interface IDeconstruct<T1, T2>
    {
        void Deconstruct(out T1 t1, out T2 t2);
    }

    public interface IDeconstruct<T1, T2, T3>
    {
        void Deconstruct(out T1 t1, out T2 t2, out T3 t3);
    }

    public interface IDeconstruct<T1, T2, T3, T4>
    {
        void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4);
    }

    public interface IDeconstruct<T1, T2, T3, T4, T5>
    {
        void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5);
    }

    public interface IDeconstruct<T1, T2, T3, T4, T5, T6>
    {
        void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6);
    }

    public interface IDeconstruct<T1, T2, T3, T4, T5, T6, T7>
    {
        void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7);
    }
}
