using AVFoundation;
using CoreVideo;
using Foundation;
using UIKit;

namespace App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		CameraRenderer.CurrentCamera?.StartLivePreview();
	}

	protected override void OnDisappearing()
	{
		CameraRenderer.CurrentCamera?.StopLivePreview();
	}

	private async void Photocapture_clicked(object sender, EventArgs e)
	{
		if (CameraRenderer.CurrentCamera != null)
		{
			Console.WriteLine("Capturing photo...");
			// Set the format of the photo to be taken.
			var format = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, new NSNumber((uint)CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange));
			// Get the setttings for the photo.
			var settings = AVCapturePhotoSettings.FromFormat(format);

			// Ensure that PortraitEffectsMatteDeliveryEnabled and EnabledSemanticSegmentationMatteTypes is enabled
			settings.PortraitEffectsMatteDeliveryEnabled = CameraRenderer.CurrentCamera.stillImageOutput.PortraitEffectsMatteDeliveryEnabled;
			settings.EnabledSemanticSegmentationMatteTypes = CameraRenderer.CurrentCamera.stillImageOutput.AvailableSemanticSegmentationMatteTypes;

			// Capture the photo
			CameraRenderer.CurrentCamera.stillImageOutput.CapturePhoto(settings, CameraRenderer.CurrentCamera.cameraProcessor);

			// Navgiate to the ImageShowcasePage
			await Navigation.PushAsync(new ImageShowcasePage());
		}
		else
		{
			throw new Exception("Camera is not initalized yet!");

		}


	}

}

