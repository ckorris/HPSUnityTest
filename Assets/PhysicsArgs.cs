using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct PhysicsArgs
{
    public float BBDiameterMM;

    public float BBMassGrams;

    public float StartSpeedMPS;

    public float SpinRPM;

    public float TemperatureCelsius;

    public float RelativeHumidity01;

    public float PressureHPa;

    public float BBToAirFrictionCoefficient;
}
