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
}
