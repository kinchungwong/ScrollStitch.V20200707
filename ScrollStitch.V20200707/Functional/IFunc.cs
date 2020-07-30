using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    public interface IFunc<TResult>
    {
        TResult Invoke();
    }

    public interface IFunc<TResult, T1>
    {
        TResult Invoke(T1 t1);
    }

    public interface IFunc<TResult, T1, T2>
    {
        TResult Invoke(T1 t1, T2 t2);
    }

    public interface IFunc<TResult, T1, T2, T3>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3);
    }

    public interface IFunc<TResult, T1, T2, T3, T4>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface IFunc<TResult, T1, T2, T3, T4, T5>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }

    public interface IFunc<TResult, T1, T2, T3, T4, T5, T6>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
