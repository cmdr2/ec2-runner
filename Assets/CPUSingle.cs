using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUSingle : Test {
    public void Test(float[] a, float[] b) {
        var n = a.Length;
        var sum = new float[n];
        for (int i = 0; i < n; i++) {
            sum[i] = Mathf.Sqrt(a[i]) + Mathf.Sin(b[i]);
        }

        //Debug.Log("sum[10]: " + sum[10]);
    }
}
