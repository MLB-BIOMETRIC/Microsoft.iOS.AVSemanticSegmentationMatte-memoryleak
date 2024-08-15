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
        captureSession = new AVCaptureSession
        {
            SessionPreset = AVCaptureSession.PresetPhoto
        };

        stillImageOutput = new AVCapturePhotoOutput()
        {
            MaxPhotoQualityPrioritization = AVCapturePhotoQualityPrioritization.Speed,
        };

        videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
        {
            VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
            Frame = Bounds
        };

        Initialize();
    }

    /// <summary>
    /// Initialize the camera
    /// </summary>
    public async void Initialize()
    {
        Console.WriteLine("Setting up camera!");
        if (!await CheckCameraPermissionAsync())
        {
            throw new Exception("No camera permission!");
        }

        // Use the BuiltInDualWideCamera with DepthData.
        var device = (AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualWideCamera, AVMediaTypes.DepthData, AVCaptureDevicePosition.Back) ?? null) ?? throw new Exception("Device not found!");

        var input = new AVCaptureDeviceInput(device, out var error);

        if (error != null)
        {
            throw new Exception("Input has error!");
        }

        // Add the input and output to the captureSession
        if (captureSession.CanAddInput(input) && captureSession.CanAddOutput(stillImageOutput))
        {
            captureSession.AddInput(input);
            captureSession.AddOutput(stillImageOutput);
            captureSession.CommitConfiguration();
            setupLivePreview();
        }

        // Enable depth data and semantic segmentation
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

    /// <summary>
    /// Setup the live preview of the camera
    /// </summary>
    private void setupLivePreview()
    {
        Console.WriteLine("Setting up live preview!");

        // Make sure the videoPreviewLayer has a connection to the captureSession before setting the orientation
        if (videoPreviewLayer.Connection != null)
        {
            videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
            Layer.AddSublayer(videoPreviewLayer);
            StartLivePreview();
        }

    }

    /// <summary>
    /// Start the live preview of the camera
    /// </summary>
    public void StartLivePreview()
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

    /// <summary>
    /// Stop the live preview of the camera
    /// </summary>
    public void StopLivePreview()
    {
        Console.WriteLine("Live preview stopped!");
        Task.Run(captureSession.StartRunning);
    }

    /// <summary>
    /// Helper method to check camera permission - or ask for it.
    /// </summary>
    public static async Task<bool> CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        return status == PermissionStatus.Granted;
    }

}