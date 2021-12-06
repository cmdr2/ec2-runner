using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStream {
    List<long> data = new List<long>();

    public long Max = 0;

    public long Mean {
        get {
            if (data.Count == 0) {
                return 0;
            }

            long mean = 0;
            foreach (var d in data) {
                mean += d;
            }
            mean /= data.Count;

            return mean;
        }
    }

    public long TP10 {get => TP(10);}
    public long TP50 {get => TP(50);}
    public long TP90 {get => TP(90);}
    public long TP99 {get => TP(99);}

    public float StandardDev {
        get {
            if (data.Count == 0) {
                return 0;
            }

            var mean = Mean;
            var variance = 0f;
            foreach (var d in data) {
                variance += (d - mean) * (d - mean);
            }
            variance /= data.Count;

            var stdDev = Mathf.Sqrt(variance);
            return stdDev;
        }
    }

    public void Add(long val) {
        if (Max < val) {
            Max = val;
        }

        data.Add(val);
    }

    public long GetLatest() {
        if (data.Count == 0) {
            return 0;
        }

        return data[data.Count - 1];
    }

    public long TP(int percent) {
        float p = (float)percent/100f;
        int index = (int)(p * data.Count - 1);
        var d = new List<long>(data);
        d.Sort();

        index = Mathf.Clamp(index, 0, data.Count - 1);

        return d[index];
    }

    public override string ToString() {
        return "tp50: " + TP50 + ", tp90: " + TP90 + ", TP99: " + TP99 + ", TP10: " + TP10;
    }
}