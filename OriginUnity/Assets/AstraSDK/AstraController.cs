using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AstraController : MonoBehaviour
{
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        // set up Astra
        AstraDotNetAssemblyResolver.Init();
        Astra.Context.Initialize();
        var connectionString = $"device/sensor{SENSOR_INDEX}";
        _streamSet = Astra.StreamSet.Open(connectionString);
        _streamReader = _streamSet.CreateReader();

        // set up body stream
        if (BodyTrackingEnabled)
        {
            _bodyStream = _streamReader.GetStream<Astra.BodyStream>();
            _bodyStream.Start();
        }

        // set up depth stream
        if (DepthEnabled)
        {
            _depthStream = _streamReader.GetStream<Astra.DepthStream>();
            _depthStream.Start();
        }

        // set up color stream
        if (ColorEnabled)
        {
            _colorStream = _streamReader.GetStream<Astra.ColorStream>();
            _colorStream.Start();
        }

    }
    void Update()
    {
        try
        {
            if (_streamReader.TryOpenFrame(10, out var frame))
            {
                using (frame)
                {
                    if (BodyTrackingEnabled)
                    {
                        var bodyFrame = frame.GetFrame<Astra.BodyFrame>();
                        bodyFrame.CopyBodyData(ref _bodies);
                        OnBodyTrackEvent?.Invoke(_bodies);
                    }
                    if (DepthEnabled)
                    {
                        var _depthFrame = frame.GetFrame<Astra.DepthFrame>();
                        OnDepthFrameEvent?.Invoke(_depthFrame);
                    }
                    if (ColorEnabled)
                    {
                        var _colorFrame = frame.GetFrame<Astra.ColorFrame>();
                        OnColorFrameEvent?.Invoke(_colorFrame);
                    }
                }
            }
        }
        catch (Astra.AstraException exc)
        {
            // As a rule, exception here means that sensor was unplugged.
            print(exc.Message);
            return;
        }
    }

    private void OnDisable()
    {
        StopStreamNoThrow(_bodyStream);
        StopStreamNoThrow(_depthStream);
        _streamReader.Dispose();
        _streamSet.Dispose();
        Astra.Context.Terminate();
    }

    private static void StopStreamNoThrow(Astra.DataStream stream)
    {
        try
        {
            stream?.Stop();
        }
        catch (Astra.AstraException exc)
        {
            Trace.TraceWarning($"Cannot stop stream {stream.GetType().FullName}: {exc.Message}");
        }
    }

    public bool DepthEnabled = true;
    public bool BodyTrackingEnabled = true;
    public bool ColorEnabled = true;

    public event Action<Astra.Body[]> OnBodyTrackEvent;
    public event Action<Astra.DepthFrame> OnDepthFrameEvent;
    public event Action<Astra.ColorFrame> OnColorFrameEvent;

    public const int MAX_BODIES = 6;
    public static AstraController Instance { get { return _instance; } }

    private const int SENSOR_INDEX = 0; // first camera
    private Astra.StreamReader _streamReader;
    private Astra.StreamSet _streamSet;
    private Astra.BodyStream _bodyStream;
    private Astra.Body[] _bodies = new Astra.Body[MAX_BODIES];
    private Astra.DepthStream _depthStream;
    private Astra.ColorStream _colorStream;
    private static AstraController _instance;
}
