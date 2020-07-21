using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    public interface IFunc
    { 
    }

    public interface IFunc<TImpl, TResult>
        : IFunc
        where TImpl: struct, IFunc
    {
        TResult Invoke();
    }

    public interface IFunc<TImpl, T1, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1);
    }

    public interface IFunc<TImpl, T1, T2, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1, T2 t2);
    }

    public interface IFunc<TImpl, T1, T2, T3, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3);
    }

    public interface IFunc<TImpl, T1, T2, T3, T4, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface IFunc<TImpl, T1, T2, T3, T4, T5, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }

    public interface IFunc<TImpl, T1, T2, T3, T4, T5, T6, TResult>
        : IFunc
        where TImpl : struct, IFunc
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
