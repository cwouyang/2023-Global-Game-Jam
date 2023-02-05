public static class TierComputer
{
    public enum Tier
    {
        None = -1,
        F = 0,
        C,
        B,
        A,
        S
    }

    public static Tier Run(ResourceTracker tracker)
    {
        //        ulong totalResources = tracker.Time + tracker.Energy + tracker.Water + tracker.Fertilizer;
        ulong totalScore = tracker.Energy + tracker.Water + tracker.Fertilizer;
        var tier = Tier.F;

        if (totalScore > 210)
            tier = Tier.S;
        else if (totalScore > 180)
            tier = Tier.A;
        else if (totalScore > 140)
            tier = Tier.B;
        else if (totalScore > 100)
            tier = Tier.C;
        else
            tier = Tier.F;

        if (tracker.Water <= 0)
            tier = Tier.F;

        return tier;
    }
}