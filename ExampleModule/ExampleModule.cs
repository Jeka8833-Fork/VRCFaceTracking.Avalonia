using Microsoft.Extensions.Logging;
using VRCFaceTracking.Core.SDK.v2;
using VRCFaceTracking.Core.SDK.v2.Facade;
using ExampleModule.Resources;
using VRCFaceTracking.Core.SDK.v2.Core.Pipeline;
using VRCFaceTracking.Core.SDK.v2.Util;

namespace ExampleModule;

public class ExampleModule : VrcftModuleV2
{
    private readonly ILogger<ExampleModule> _logger;

    public ExampleModule(IModuleController controller) : base(controller)
    {
        _logger = Module.GetLoggerFactory().CreateLogger<ExampleModule>();

        Module.GetUi().SetTitle(MyModuleStrings.WelcomeMessage);
        Module.GetUi().SetStatus("This is an example module.\nIt waits starting.");
        Module.GetUi().SetIcons("ExampleModule.Assets.logo.png");

        Module.GetModuleManager().SubscribeToModuleListChanged(moduleList =>
        {
            bool isStartAllowed = true;
            foreach (VrcftModuleV2 vrcftModuleV2 in moduleList)
            {
                if (vrcftModuleV2.GetCachedModuleId() ==
                    Guid.Parse("815d2d1d-4d1d-4d1d-815d-2d1d4d1d815d") && // It's better to cache uuid
                    vrcftModuleV2.Module.GetModuleManager().IsStarted())
                {
                    Module.GetUi()
                        .SetStatus($"Please disable {vrcftModuleV2.Module.GetUi().GetTitle()} module first.");
                    isStartAllowed = false;
                    break;
                }
            }

            Module.GetModuleManager().SetAllowStart(isStartAllowed);
        });

        // You can connect libs or other things here
    }

    protected override Guid GetModuleId() => Guid.Parse("815d2d1d-4d1d-4d1d-815d-2d1d4d1d815d");

    public override void StartModule(CancellationToken cancellationToken)
    {
        IModuleParameterManager parameterManager = Module.GetParameterManager();
        parameterManager.SetParameterTimeout(TimeSpan.FromSeconds(1));

        IPipelineNode listener = new CachedNodeListener(PipelineStage.AfterModule,
            (header, parameters, cache) =>
            {
                float? left =
                    (parameters.TryGetValue(ARKitParameters.EyeLookDownLeft, out object? leftValue)
                        ? leftValue
                        : cache.TryGetValue(ARKitParameters.EyeLookDownLeft, out object? leftValueCached)
                            ? leftValueCached
                            : null) as float?;
                float? right =
                    (parameters.TryGetValue(ARKitParameters.EyeLookDownRight, out object? rightValue)
                        ? rightValue
                        : cache.TryGetValue(ARKitParameters.EyeLookDownRight, out object? rightValueCached)
                            ? rightValueCached
                            : null) as float?;

                if (left.HasValue)
                {
                    parameters[ARKitParameters.EyeLookDownLeft] = left.Value * 2f;
                }

                if (right.HasValue)
                {
                    parameters[ARKitParameters.EyeLookDownRight] = right.Value * 2f;
                }

                return header.Modify()
                    .WithExpireTime(WarpedMutableTime.GetCurrentTime())
                    .Build();
            },
            ARKitParameters.EyeLookDownLeft, ARKitParameters.EyeLookDownRight);

        parameterManager.RegisterPipelineNode(listener);

        try
        {
            Module.GetUi().SetStatus("Trying to find connection...\nLine 1\nLine 2\nLine 3");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // This is an example of a long-running task, you can do udp wait for example
                    Thread.Sleep(
                        5000); // It's preferable to use cancellationToken.WaitHandle.WaitOne() if you need to wait to avoid receiving ThreadInterruptedException unnecessarily.


                    int secondsCounter = 0;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Module.GetUi().SetStatus("Module is running.\nSeconds: " + secondsCounter);

                        cancellationToken.WaitHandle.WaitOne(1000);
                        secondsCounter++;

                        parameterManager.SetValue(ARKitParameters.EyeLookDownLeft, (float)Math.Sin(secondsCounter));
                        parameterManager.Flush();
                    }
                }
                catch (ThreadInterruptedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Error in second loop");

                    cancellationToken.WaitHandle.WaitOne(10);
                }
            }
        }
        catch (ThreadInterruptedException)
        {
            Module.GetUi().SetStatus("Module disabled with interrupt.");

            throw;
        }
        catch (Exception e)
        {
            Module.GetUi().SetStatus("Module crashed.");

            _logger.LogWarning(e, "Error in first loop");

            throw;
        }

        Module.GetUi().SetStatus("Module disabled without interrupt.");
    }

    public override void Shutdown()
    {
        _logger.LogInformation("We closed all threads, sockets, etc. VRCFT is shutting down.");
    }
}
