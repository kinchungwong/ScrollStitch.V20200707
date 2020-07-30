using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    public interface IAction
    {
        void Invoke();
    }

    public interface IAction<T1>
    {
        void Invoke(T1 t1);
    }

    public interface IAction<T1, T2>
    {
        void Invoke(T1 t1, T2 t2);
    }

    public interface IAction<T1, T2, T3>
    {
        void Invoke(T1 t1, T2 t2, T3 t3);
    }

    public interface IAction<T1, T2, T3, T4>
    {
        void Invoke(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface IAction<T1, T2, T3, T4, T5>
    {
        void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }

    public interface IAction<T1, T2, T3, T4, T5, T6>
    {
        void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
