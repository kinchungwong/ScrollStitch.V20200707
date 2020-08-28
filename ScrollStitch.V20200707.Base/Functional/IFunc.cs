using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    public interface IFunc<out TResult>
    {
        TResult Invoke();
    }

    public interface IFunc<in T1, out TResult>
    {
        TResult Invoke(T1 t1);
    }

    public interface IFunc<in T1, in T2, out TResult>
    {
        TResult Invoke(T1 t1, T2 t2);
    }

    public interface IFunc<in T1, in T2, in T3, out TResult>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3);
    }

    public interface IFunc<in T1, in T2, in T3, in T4, out TResult>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface IFunc<in T1, in T2, in T3, in T4, in T5, out TResult>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }

    public interface IFunc<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
