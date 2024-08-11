using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public class RelicUtils
{
    public static readonly Relic[] AllRelics = new Relic[]
    {
        new BasePointsRelic(),
        new BaseMultiplierRelic(),
        new MultiplierRelic(),
    };

    public static Relic RandomRelic()
    {
        return AllRelics[UnityEngine.Random.Range(0, AllRelics.Length)];
    }
}

public abstract class Relic
{
    public abstract string Name();
    public abstract string Description();
    public virtual int GetBasePoints(Hand hand)
    {
        return 0;
    }

    public virtual int GetBaseMultiplier(Hand hand)
    {
        return 0;
    }

    public virtual float GetMultiplier(Hand hand)
    {
        return 1f;
    }
}

public class BasePointsRelic: Relic
{
    public override string Name()
    {
        return "Chips";
    }
    public override string Description()
    {
        return "+100 Chips";
    }
    public override int GetBasePoints(Hand hand)
    {
        return 100;
    }
}

public class BaseMultiplierRelic : Relic
{
    public override string Name()
    {
        return "Mult";
    }
    public override string Description()
    {
        return "+10 Mult";
    }
    public override int GetBaseMultiplier(Hand hand)
    {
        return 10;
    }
}

public class MultiplierRelic : Relic
{
    public override string Name()
    {
        return "xMult";
    }
    public override string Description()
    {
        return "x1.5 Mult";
    }
    public override float GetMultiplier(Hand hand)
    {
        return 1.5f;
    }
}
