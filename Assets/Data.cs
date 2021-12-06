using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public static float[] GetRandom(int n) {
        var a = new float[n];
        for (int i = 0; i < n; i++) {
            a[i] = Random.value;
        }

        return a;
    }

    public static float[] GetSequence(int n) {
        var a = new float[n];
        for (int i = 0; i < n; i++) {
            a[i] = i;
        }

        return a;
    }
}
