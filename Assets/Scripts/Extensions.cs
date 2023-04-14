using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static int RawDirection(this float axisInput)
    {
        if (axisInput < 0f)
            return -1;
        else if (axisInput > 0f)
            return 1;
        return 0;
    }

    public static T ChooseRandomElement<T>(this IEnumerable<T> set) => set.Skip(Random.Range(0, set.Count())).Single();
}
