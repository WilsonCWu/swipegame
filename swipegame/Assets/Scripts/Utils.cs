using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static System.Random _R = new System.Random();
    public static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(_R.Next(v.Length));
    }
}
