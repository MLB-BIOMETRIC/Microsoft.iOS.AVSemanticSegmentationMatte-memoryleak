using AVFoundation;
using CoreVideo;
using UIKit;

namespace App;

public class CameraSetup : UIView
{

    public AVCaptureSession captureSession { get; set; }
    public AVCapturePhotoOutput stillImageOutput { get; set; }
    public AVCaptureVideoPreviewLayer videoPreviewLayer { get; set; }
    public CameraProcessor cameraProcessor { get; set; }

    public CameraSetup()
    {
        cameraProcessor = new CameraProcessor();
        Initialize();
    }

    public async void Initialize()
    {
        Console.WriteLine("Setting up camera!");
        if (!await CheckCameraPermissionAsync())
        {
            throw new Exception("No camera permission!");
        }

        captureSession = new AVCaptureSession
        {
            SessionPreset = AVCaptureSession.PresetPhoto
        };

        var device = (AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualWideCamera, AVMediaTypes.DepthData, AVCaptureDevicePosition.Back) ?? null) ?? throw new Exception("Device not found!");

        var input = new AVCaptureDeviceInput(device, out var error);

        if (error != null)
        {
            throw new Exception("Input has error!");
        }

        stillImageOutput = new AVCapturePhotoOutput()
        {
            MaxPhotoQualityPrioritization = AVCapturePhotoQualityPrioritization.Speed,
        };

        if (captureSession.CanAddInput(input) && captureSession.CanAddOutput(stillImageOutput))
        {
            captureSession.AddInput(input);
            captureSession.AddOutput(stillImageOutput);
            captureSession.CommitConfiguration();
            setupLivePreview();
        }

        if (stillImageOutput.DepthDataDeliverySupported)
        {
            stillImageOutput.DepthDataDeliveryEnabled = true;
        };

        if (stillImageOutput.PortraitEffectsMatteDeliverySupported)
        {
            stillImageOutput.EnabledSemanticSegmentationMatteTypes = stillImageOutput.AvailableSemanticSegmentationMatteTypes; // Set all the available
            stillImageOutput.PortraitEffectsMatteDeliveryEnabled = true; // Enable sementaing delivery
        }
    }

    private void setupLivePreview()
    {
        Console.WriteLine("Setting up live preview!");
        videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
        {
            VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
            Frame = Bounds
        };
        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;

        Layer.AddSublayer(videoPreviewLayer);

        startLivePreview();
    }

    public void startLivePreview()
    {
        Console.WriteLine("Live preview started!");
        Task.Run(() =>
            {
                captureSession.StartRunning();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    videoPreviewLayer.Frame = Layer.Bounds;
                });
            });
    }

    public void stopLivePreview()
    {
        Console.WriteLine("Live preview stopped!");
        Task.Run(() =>
            {
                captureSession.StartRunning();
            });
    }

    public async Task<bool> CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        return status == PermissionStatus.Granted;
    }

}