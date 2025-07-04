using Microsoft.Extensions.Logging;
using VRCFaceTracking.Core.SDK.v2;
using VRCFaceTracking.Core.SDK.v2.Facade;

namespace ExampleModule;

public class ExampleModule : VrcftModuleV2
{
    private readonly ILogger<ExampleModule> _logger;

    public ExampleModule(IModuleController controller) : base(controller)
    {
        _logger = Module.GetLoggerFactory().CreateLogger<ExampleModule>();

        Module.GetUi().SetName("Example Module");
        Module.GetUi().SetDescription("This is an example module.\nIt waits starting.");
        Module.GetUi().SetAllowStart(true);
        Module.GetUi().SetIcons("Assets.example.png");

        // You can connect libs or other things here
    }

    public override void StartModule(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override void Shutdown()
    {
        _logger.LogInformation("We closed all threads, sockets, etc. VRCFT is shutting down.");
    }
}
