using System;
using System.Collections;
using System.Numerics;
using System.Reflection;

namespace Blackbird.Stark.Math;

public static class Hash
{
    /// <summary>
    /// Calculate hash for object using multiplication algorithm.
    /// Each object or array is a sum of hashes of it's parts.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="seed"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int CalculateForObject(object obj, int seed = 3, int offset = 7)
    {
        var objType = obj.GetType();
        if (objType.IsPrimitive || obj is string)
            return obj.GetHashCode();

        var result = new BigInteger(seed);
        
        if (objType.IsArray || obj is IEnumerable)
        {
            var arr = obj as IEnumerable;
            foreach (var e in arr)
            {
                unchecked
                {
                    var a = offset * CalculateForObject(e, seed, offset);
                    result += a;
                }
            }
        }

        if (objType.IsClass)
        {
            foreach (var member in objType.GetFields(BindingFlags.Instance))
            {
                unchecked
                {
                    var v = member.GetValue(obj);
                    var a = offset * CalculateForObject(v, seed, offset);
                    result += a;
                }
            }
        }

        return (int) (result % int.MaxValue);
    }
}