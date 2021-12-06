using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class CPUBurst : Test {
    [BurstCompile]
    public struct TestJob : IJob {
        [ReadOnly]
        public NativeArray<float> a, b;

        public NativeArray<float> sum;

        public void Execute() {
            var n = a.Length;
            //var sum = 0f;
            for (int i = 0; i < n; i++) {
                sum[i] = Mathf.Sqrt(a[i]) + Mathf.Sin(b[i]);
            }
            //this.sum[0] = sum;
        }
    }

    public void Test(NativeArray<float> a, NativeArray<float> b) {
        var job = new TestJob {
            a = a,
            b = b,
            sum = new NativeArray<float>(a.Length, Allocator.Persistent)
        };

        job.Schedule().Complete();

        //Debug.Log("sum[10]: " + job.sum[10]);
    }
}
