using AVFoundation;
using UIKit;

namespace App;

public class CameraSetup : UIView
{

    public AVCaptureSession captureSession { get; set; }
    public AVCapturePhotoOutput stillImageOutput { get; set; }
    public AVCaptureVideoPreviewLayer videoPreviewLayer { get; set; }

    public CameraSetup()
    {
        Console.WriteLine("CAMERASETUP()");
        Initialize();
    }

    public async void Initialize()
    {

        if (!await CheckCameraPermissionAsync())
        {
            throw new Exception("No camera permission!");
        }

        captureSession = new AVCaptureSession
        {
            SessionPreset = AVCaptureSession.PresetPhoto
        };

        var device = (AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualWideCamera, AVMediaTypes.Video, AVCaptureDevicePosition.Back) ?? null) ?? throw new Exception("Device not found!");

        Console.WriteLine("DEVICE OK!");

        var input = new AVCaptureDeviceInput(device, out var error);

        if (error != null)
        {
            throw new Exception("Input has error!");
        }

        Console.WriteLine("INPUT OK");

        stillImageOutput = new AVCapturePhotoOutput();

        Console.WriteLine("OUTPUT OK");

        if (captureSession.CanAddInput(input) && captureSession.CanAddOutput(stillImageOutput))
        {
            Console.WriteLine("CAN ADD INPUT AND OUTPUT!");
            captureSession.AddInput(input);
            Console.WriteLine("INPUT ADDED!");
            captureSession.AddOutput(stillImageOutput);
            Console.WriteLine("OUTPUT ADDED!");

            captureSession.CommitConfiguration();

            Console.WriteLine("CONFIGURATION COMMITTED!");
            setupLivePreview();
        }
    }

    private void setupLivePreview()
    {
        videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
        {
            VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
            Frame = Bounds
        };
        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;

        Layer.AddSublayer(videoPreviewLayer);

        Console.WriteLine("Live preview setup!");
        Task.Run(() =>
        {
            StartCamera();
        });

        Task.Run(() =>
        {
            Thread.Sleep(1000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                videoPreviewLayer.Frame = Layer.Bounds;
            });
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

    public void StartCamera()
    {
        Layer.Hidden = false;
        Console.WriteLine("Session is running!");
        captureSession.StartRunning();
    }

}