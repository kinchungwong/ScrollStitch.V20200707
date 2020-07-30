using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Functional
{
    /// <summary>
    /// <c>IFuncInline</c> is a family of interfaces representing a functor that is marked for 
    /// inlining by the JIT code generation.
    /// 
    /// <para>
    /// Concrete implementations must inherit from one of the generic interfaces, and must 
    /// supply the full list of: 
    /// <br/>
    /// ... The concrete implementation's own type, <c>TImpl</c>, <br/>
    /// ... The list of input argument types, <c>T1, T2, ...</c> <br/>
    /// ... The return value type, <c>TResult</c>
    /// </para>
    /// 
    /// <para>
    /// The following two base interface do not need to be directly inherited by any concrete 
    /// implementations: <br/>
    /// ... <c>IFuncInline</c> <br/>
    /// ... <c>IFuncInline&lt;TImpl&gt;</c> <br/>
    /// These are already include dindirectly. Their purpose is to help with type inferences.
    /// </para>
    /// 
    /// <para>
    /// To facilitate inlining during JIT code generation, the concrete implementation shall 
    /// follow these guidelines:
    /// <br/>
    /// The concrete implementation shall be a <see langword="struct"/>. 
    /// <br/>
    /// The <c>Invoke(...)</c> method shall have an <c>AggressiveInlining</c> attribute.
    /// </para>
    /// 
    /// <para>
    /// Meanwhile, the algorithm classes or methods consuming these functors shall follow 
    /// these guidelines: 
    /// <br/>
    /// The consuming type or method shall be generic. 
    /// <br/>
    /// The consuming type or method shall take the concrete functor implementation's type
    /// as one of its generic type parameters.
    /// <br/>
    /// The consuming type or method shall use generic constraints to enforce that the 
    /// concrete functor implementation type is a struct, a non-generic <c>IFuncInline</c>,
    /// a generic <c>IFuncInline</c> with the concrete functor implementation itself, and finally 
    /// a generic <c>IFuncInline</c> where the full list of input argument types and the 
    /// output type are specified.
    /// </para>
    /// 
    /// <para>
    /// The consuming type or method shall take care not to introduce inadvertent boxing,
    /// which may occur if the call to the concrete <c>Invoke(...)</c> happens through an 
    /// interface type (such as through <c>IFuncInline</c>). The earlier mentioned 
    /// guidelines work together to prevent such boxing.
    /// </para>
    /// 
    /// </summary>
    /// 
    public interface IFuncInline
    { 
    }

    /// <summary>
    /// This is a base interface for the <see cref="IFuncInline"/> family of interfaces, which 
    /// representing a functor that is marked for inlining by the JIT code generation.
    /// 
    /// <para>
    /// Concrete implementation should not directly implement <see cref="IFuncInline"/> nor 
    /// <see cref="IFuncInline{TImpl}"/>. <br/>
    /// Refer to <see cref="IFuncInline"/> for usage guideline.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TImpl">
    /// The concrete implementation type.
    /// </typeparam>
    /// 
    public interface IFuncInline<TImpl>
        : IFuncInline
        where TImpl: struct, IFuncInline, IFuncInline<TImpl>
    {
    }

    /// <summary>
    /// <see cref="IFuncInline{TImpl, TResult}"/> represents a functor with zero argument and 
    /// one return value, where the functor is marked for inlining.
    /// 
    /// <para>
    /// Refer to <see cref="IFuncInline"/> for overview.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TImpl">
    /// The concrete implementation type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The return value type.
    /// </typeparam>
    /// 
    public interface IFuncInline<TImpl, TResult>
        : IFuncInline<TImpl>
        where TImpl: struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke();
    }

    /// <summary>
    /// <see cref="IFuncInline{TImpl, T1, TResult}"/> represents a functor with one argument and 
    /// one return value, where the functor is marked for inlining.
    /// 
    /// <para>
    /// Refer to <see cref="IFuncInline"/> for overview.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first argument to the functor.
    /// </typeparam>
    /// <inheritdoc cref="IFuncInline{TImpl, TResult}"/>
    /// 
    public interface IFuncInline<TImpl, T1, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1);
    }

    public interface IFuncInline<TImpl, T1, T2, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1, T2 t2);
    }

    public interface IFuncInline<TImpl, T1, T2, T3, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3);
    }

    public interface IFuncInline<TImpl, T1, T2, T3, T4, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface IFuncInline<TImpl, T1, T2, T3, T4, T5, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }

    public interface IFuncInline<TImpl, T1, T2, T3, T4, T5, T6, TResult>
        : IFuncInline<TImpl>
        where TImpl : struct, IFuncInline, IFuncInline<TImpl>
    {
        TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
