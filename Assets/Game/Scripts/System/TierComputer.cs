public static class TierComputer
{
    public enum Tier
    {
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

        if (totalScore > 240)
            tier = Tier.S;
        else if (totalScore > 200)
            tier = Tier.A;
        else if (totalScore > 150)
            tier = Tier.B;
        else if (totalScore > 100)
            tier = Tier.C;
        else
            tier = Tier.F;

        return tier;
    }
}