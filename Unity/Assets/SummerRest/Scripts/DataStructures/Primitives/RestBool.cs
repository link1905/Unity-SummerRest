using System;
using MemoryPack;

namespace SummerRest.DataStructures.Primitives
{
    [Serializable]
    [MemoryPackable]
    public partial class RestBool : Primitive<bool>
    { }
}