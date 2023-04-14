using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class Global
{
    public static class Inputs
    {
        public const string AxisHorizontal = "Horizontal";

        public const string AxisVertical = "Vertical";
    }

    public static class LayerMasks
    {
        private static readonly Dictionary<LayerMaskType, IEnumerable<string>> DefinedLayerMasks = new Dictionary<LayerMaskType, IEnumerable<string>>
        {
            { 
                LayerMaskType.Default,
                new[] { "Wall", "Enemy" }
            },
            {
                LayerMaskType.WallFloor,
                new[] { "Wall", "Floor" }
            }
        };

        public static int GetLayerMask(LayerMaskType type)
        {
            return DefinedLayerMasks.TryGetValue(type, out IEnumerable<string> layers) ? LayerMask.GetMask(layers.ToArray()) : 0;
        }
    }

    public static class Tags
    {
        public const string Player = "Player";
    }

    public static bool TestTileCollision(Vector3 target, LayerMaskType type = LayerMaskType.Default)
    {
        return Physics2D.OverlapBox(
            target,
            Vector3.one * 0.9f,
            0,
            LayerMasks.GetLayerMask(type)) != null;
    }
}
