public static class FireTypeExtensions
{
    private const float A_SPREAD_CHANCE = .7f, B_SPREAD_CHANCE = .4f, D_SPREAD_CHANCE = .2f, C_SPREAD_CHANCE = .5f;

    public static float GetSpreadChance(this FireType fireType)
    {
        return fireType switch
        {
            FireType.A => A_SPREAD_CHANCE,
            FireType.B => B_SPREAD_CHANCE,
            FireType.C => C_SPREAD_CHANCE,
            FireType.D => D_SPREAD_CHANCE,
            _ => .3f
        };
    }

    public static string ToString(this FireType fireType)
    {
        return fireType switch
        {
            FireType.A => "Class A",
            FireType.B => "Class B",
            FireType.C => "Class C",
            FireType.D => "Class D",
            _ => "Null"
        };
    }
}
