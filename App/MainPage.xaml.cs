using AVFoundation;
using CoreVideo;
using Foundation;

namespace App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		CameraRenderer.CurrentCamera?.startLivePreview();
	}

	protected override void OnDisappearing()
	{
		CameraRenderer.CurrentCamera?.stopLivePreview();
	}

	private async void photocapture_clicked(object sender, EventArgs e)
	{
		if (CameraRenderer.CurrentCamera != null)
		{
			Console.WriteLine("Capturing photo...");
			var format = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, new NSNumber((uint)CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange));
			var settings = AVCapturePhotoSettings.FromFormat(format);

			settings.PortraitEffectsMatteDeliveryEnabled = CameraRenderer.CurrentCamera.stillImageOutput.PortraitEffectsMatteDeliveryEnabled;
			settings.EnabledSemanticSegmentationMatteTypes = CameraRenderer.CurrentCamera.stillImageOutput.AvailableSemanticSegmentationMatteTypes;

			CameraRenderer.CurrentCamera.stillImageOutput.CapturePhoto(settings, CameraRenderer.CurrentCamera.cameraProcessor);
			await Navigation.PushAsync(new ImageShowcasePage());
		}
		else
		{
			throw new Exception("Camera is not initalized yet!");

		}


	}

}

