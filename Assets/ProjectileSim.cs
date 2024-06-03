using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class ProjectileSim : MonoBehaviour
{
    [DllImport("HoppedProjectileSim", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool RunSimulation(float distbetweensamples, int maxSamples, Float3 camPosOffset, Float3 camRotOffset,
                                       PhysicsArgs physicsArgs, Float3 gravityVector,
                                       [MarshalAs(UnmanagedType.FunctionPtr)] CollisionDetectionFunc collisionDetectionFunc,
                                       ref float collisiondepth, ref float totaltime,
                                       ref IntPtr linePoints, ref int linePointsCount, ref Stats stats);

    public delegate bool CollisionDetectionFunc(Float3 pos1, Float3 pos2);

    [SerializeField]
    private float _distanceBetweenSamples = 0.05f;
    [SerializeField]
    private int _maxSamples = 1000;

    [SerializeField]
    private float _bbDiameterMM = 6f;
    [SerializeField]
    private float _bbMassGrams = 0.28f;
    [SerializeField]
    private float _startSpeedMPS = 105f;
    [SerializeField]
    [Range(0f, 150000f)]
    private float _spinRPM = 19000f;
    [SerializeField]
    private float _temperatureCelsius = 20f;
    [SerializeField]
    [Range(0f, 1f)]
    private float _relativeHumidity01 = 0.5f;
    [SerializeField]
    private float _pressureHPa = 1000f; //Looking this up as I wrote this in Chicago on June 2, 2024 around 8:30pm, it was 1014hPa.
    [SerializeField]
    [Range(0f, 1f)]
    private float _bbToAirFrictionCoefficient = 0.5f;



    private LineRenderer _lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;

        TryToSimulate();
    }


    // Update is called once per frame
    void Update()
    {
        TryToSimulate();
    }


    private void TryToSimulate()
    {
        CollisionDetectionFunc counterFunc = (last, current) =>
        {
            Vector3 lastWorld = transform.InverseTransformPoint(last);
            Vector3 currentWorld = transform.InverseTransformPoint(current);

            return Physics.Linecast(lastWorld, currentWorld);
        };

        PhysicsArgs physicsArgs = new PhysicsArgs()
        {
            BBDiameterMM = _bbDiameterMM,
            BBMassGrams = _bbMassGrams,
            StartSpeedMPS = _startSpeedMPS,
            SpinRPM = _spinRPM,
            TemperatureCelsius = _temperatureCelsius,
            RelativeHumidity01 = _relativeHumidity01,
            PressureHPa = _pressureHPa,
            BBToAirFrictionCoefficient = _bbToAirFrictionCoefficient //No idea if this is normal.
        };

        float collisionDepth = 0;
        float totalTime = 0;
        Stats stats = new Stats();

        CollisionDetectionFunc collisionFunc = new CollisionDetectionFunc(counterFunc);

        IntPtr linePointsPtr = IntPtr.Zero;
        int linePointsCount = 0;

        bool hit = RunSimulation(_distanceBetweenSamples, _maxSamples, new Float3() { x = 0, y = 0, z = 0 }, new Float3() { x = 0, y = 0, z = 0 }, physicsArgs,
            transform.InverseTransformDirection(Vector3.down), collisionFunc, ref collisionDepth, ref totalTime, ref linePointsPtr, 
            ref linePointsCount,  ref stats);


        if (linePointsCount > 0 && linePointsPtr != IntPtr.Zero)
        {
            //Marshal the data from the unmanaged memory to a managed array.
            Float3[] linePoints = new Float3[linePointsCount];
            IntPtr currentPtr = linePointsPtr;

            for (int i = 0; i < linePointsCount; i++)
            {
                linePoints[i] = Marshal.PtrToStructure<Float3>(currentPtr);
                currentPtr = IntPtr.Add(currentPtr, Marshal.SizeOf(typeof(Float3)));
            }

            //Free the unmanaged memory.
            Marshal.FreeCoTaskMem(linePointsPtr);

            //Use linePoints array as needed.
            _lineRenderer.positionCount = linePointsCount;
            _lineRenderer.SetPositions(linePoints.Select(point => new Vector3(point.x, point.y, point.z)).ToArray());
        }
        else
        {
            Debug.LogError("Simulation failed or no points generated.");
        }
    }
}
