using System.Collections.Generic;
using UnityEngine;

public static class ExtinguisherTypeExtensions
{
    private static readonly Dictionary<FireType, Dictionary<ExtinguisherType, float>> _effectiveness = new Dictionary<FireType, Dictionary<ExtinguisherType, float>>
        {
            {
                FireType.A, new Dictionary<ExtinguisherType, float>
                {
                    { ExtinguisherType.Water, 25f },
                    { ExtinguisherType.Foam, 20f },
                    { ExtinguisherType.Powder, 15f },
                    { ExtinguisherType.MetalX, 0f }
                }
            },
            {
                FireType.B, new Dictionary<ExtinguisherType, float>
                {
                    { ExtinguisherType.Water, -10f },
                    { ExtinguisherType.Foam, 30f },
                    { ExtinguisherType.Powder, 25f },
                    { ExtinguisherType.MetalX, 0f }
                }
            },
            {
                FireType.C, new Dictionary<ExtinguisherType, float>
                {
                    { ExtinguisherType.Water, 0f },
                    { ExtinguisherType.Foam, 10f },
                    { ExtinguisherType.Powder, 25f },
                    { ExtinguisherType.MetalX, 0f }
                }
            },
            {
                FireType.D, new Dictionary<ExtinguisherType, float>
                {
                    { ExtinguisherType.Water, -50f },
                    { ExtinguisherType.Foam, -20f },
                    { ExtinguisherType.Powder, 10f },
                    { ExtinguisherType.MetalX, 40f }
                }
            }
        };

    public static float GetEffectiveness(this ExtinguisherType extinguisherType, FireType fireType)
    {
        if (_effectiveness.TryGetValue(fireType, out var fireEffectiveness))
        {
            if (fireEffectiveness.TryGetValue(extinguisherType, out var effectiveness))
            {
                return effectiveness;
            }
        }

        return 0f;
    }

    public static Dictionary<ExtinguisherType, float> GetEffectivenessForFireType(FireType fireType)
    {
        if (_effectiveness.TryGetValue(fireType, out var fireEffectiveness))
        {
            return new Dictionary<ExtinguisherType, float>(fireEffectiveness);
        }

        return new Dictionary<ExtinguisherType, float>();
    }

    public static Color GetColorByType(this ExtinguisherType type)
    {
        return type switch
        {
            ExtinguisherType.Water => Color.blue,
            ExtinguisherType.Foam => Color.red,
            ExtinguisherType.Powder => new Color(.8f, .8f, .8f),
            ExtinguisherType.MetalX => Color.white,
            _ => Color.white,
        };
    }
}
