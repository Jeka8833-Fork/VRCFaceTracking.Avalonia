using VRCFaceTracking.Core.SDK.v2.Core;

namespace ExampleModule;

// ReSharper disable once InconsistentNaming
public static class ARKitParameters
{
    public static readonly ParameterName<float> EyeLookDownLeft = new("eyeLookDownLeft");
    public static readonly ParameterName<float> EyeLookDownRight = new("eyeLookDownRight");
}
