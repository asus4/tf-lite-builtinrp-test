using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TensorFlowLite;

public class MicrophoneTest : MonoBehaviour
{
    [SerializeField]
    private MicrophoneBuffer.Options microphoneOptions = new();

    [SerializeField]
    private RectTransform waveformView = null;



    private MicrophoneBuffer mic;
    private PrimitiveDraw waveFormDrawer;
    private readonly Vector3[] rtCorners = new Vector3[4];
    private float[] samples = new float[4096];


    IEnumerator Start()
    {
        mic = new MicrophoneBuffer();
        yield return mic.StartRecording(microphoneOptions);
        waveFormDrawer = new PrimitiveDraw(Camera.main, gameObject.layer);
    }

    private void Update()
    {
        if (waveFormDrawer == null)
        {
            return;
        }

        mic.GetLatestSamples(samples);
        UpdateWaveform(samples);

        waveFormDrawer.Apply(drawEditor: true, clear: false);
    }

    private void UpdateWaveform(float[] input)
    {
        waveformView.GetWorldCorners(rtCorners);
        float3 min = rtCorners[0];
        float3 max = rtCorners[2];

        waveFormDrawer.Clear();
        waveFormDrawer.color = Color.green;

        int length = input.Length;
        float delta = 1f / length;

        Vector3 prev = math.lerp(min, max, new float3(0, input[0] * 0.5f + 0.5f, 0));
        for (int i = 1; i < length; i++)
        {
            float3 t = new(i * delta, input[i] * 0.5f + 0.5f, 0);
            Vector3 point = math.lerp(min, max, t);
            waveFormDrawer.Line3D(prev, point, 0.01f);
            prev = point;
        }
    }
}
