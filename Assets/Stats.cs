
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Stats
{
    public float SpeedImpact;

    public float AirPressureHPa;

    public float AirDensityKGM3;

    public float AirViscosityPaS;

    public float DragCoefficientBarrel;

    public float DragCoefficientImpact;

    public float DragForceBarrel;

    public float DragForceImpact;

    public float MagnusCoefficientBarrel;

    public float MagnusCoefficientImpact;

    public float MagnusForceBarrel;

    public float MagnusForceImpact;
}
