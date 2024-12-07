using System;

namespace Monads
{
    /// <summary>
    /// marker attribute indicating no heap garbage generated
    /// does not ensure calling code is garbage free
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GarbageFreeAttribute : Attribute
    {
    }
}