using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Collections;

public class TestBench : MonoBehaviour
{
    public int iters = 100;
    public int samplesPerIter = 2000000;
    public bool runAllTests = false;
    public TestType testType = TestType.CPUSingle;

    public ComputeShader computeShader;

    float[] a, b;
    NativeArray<float> nativeA, nativeB;

    public enum TestType {
        CPUSingle, CPUBurst, GPUComputeShader
    };

    void Start() {
        iters = 3;
        samplesPerIter = 100;
        print("Iters: " + iters + ", samplesPerIter: " + samplesPerIter);

        try {
            if (runAllTests) {
                RunTest(TestType.CPUSingle);
                RunTest(TestType.CPUBurst);
                RunTest(TestType.GPUComputeShader);
            } else {
                RunTest(testType);
            }
        } finally {
            UnityEngine.Debug.Break();
            Application.Quit();
        }
    }

    void RunTest(TestType type) {
        Test test = null;

        if (type == TestType.CPUSingle) {
            test = new CPUSingle();
        } else if (type == TestType.CPUBurst) {
            test = new CPUBurst();
        } else if (type == TestType.GPUComputeShader) {
            if (!SystemInfo.supportsComputeShaders) {
                UnityEngine.Debug.Log("No compute shader supported!");
                return;
            }

            test = new GPUComputeShader();
            var t = (GPUComputeShader)test;
            t.shader = computeShader;
            t.kernel = t.shader.FindKernel("CSMain");
            t.shader.GetKernelThreadGroupSizes(t.kernel, out uint threadGroupSize, out _, out _);
            t.threadGroupSize = threadGroupSize;
        }

        var stats = RunTest(test, iters, samplesPerIter);

        print(test);
        print(stats);
    }
    
    DataStream RunTest(Test test, int iters, int samplesPerIter) {
        a = Data.GetSequence(samplesPerIter);
        b = Data.GetSequence(samplesPerIter);

        nativeA = new NativeArray<float>(a, Allocator.Persistent);
        nativeB = new NativeArray<float>(b, Allocator.Persistent);

        ComputeBuffer aBuf = new ComputeBuffer(a.Length, sizeof(float));
        aBuf.SetData(a);
        ComputeBuffer bBuf = new ComputeBuffer(b.Length, sizeof(float));
        bBuf.SetData(b);

        if (test is GPUComputeShader) {
            var x = ((GPUComputeShader)test);
            x.shader.SetBuffer(x.kernel, "a", aBuf);
            x.shader.SetBuffer(x.kernel, "b", bBuf);
        }

        var t = new Stopwatch();
        var stats = new DataStream();

        for (int i = 0; i < iters; i++) {
            t.Restart();

            if (test is CPUSingle) {
                ((CPUSingle)test).Test(a, b);
            } else if (test is CPUBurst) {
                ((CPUBurst)test).Test(nativeA, nativeB);
            } else if (test is GPUComputeShader) {
                ((GPUComputeShader)test).Test(a.Length, a);
            }

            t.Stop();

            stats.Add(t.ElapsedMilliseconds);
        }

        return stats;
    }

    private void OnDestroy() {
        if (nativeA != null) {
            nativeA.Dispose();
            nativeB.Dispose();
        }
    }
}

public interface Test {}
