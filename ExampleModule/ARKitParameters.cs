using VRCFaceTracking.Core.SDK.v2;

namespace ExampleModule;

public static class ARKitParameters
{
    public static readonly ParameterName<float> EyeLookDownLeft = new("eyeLookDownLeft");
    public static readonly ParameterName<float> EyeLookDownRight = new("eyeLookDownRight");
}
