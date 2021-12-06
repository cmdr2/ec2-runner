using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUComputeShader : Test {
    public ComputeShader shader;
    public int kernel;
    public uint threadGroupSize;

    public void Test(int n, float[] a) {
        ComputeBuffer res = new ComputeBuffer(n, sizeof(float));

        float[] output = new float[n];

        shader.SetBuffer(kernel, "Result", res);

        //int threadGroups = (int)((n + (threadGroupSize - 1)) / threadGroupSize);
        shader.Dispatch(kernel, 64, 1, 1);
        res.GetData(output);

        //Debug.Log("sum[10]: " + output[10]);
    }
}
