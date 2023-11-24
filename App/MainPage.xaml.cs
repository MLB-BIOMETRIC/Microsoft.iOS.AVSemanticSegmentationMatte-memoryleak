using AVFoundation;
using CoreVideo;
using Foundation;

namespace App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		Console.WriteLine("MAINPAGE INIT()");
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
		// I WANT TO BE ABLE TO TAKE PHOTO HERE
		Console.WriteLine("photocapture_clicked clicked!");

		if (CameraRenderer.CurrentCamera != null)
		{
			var format = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, new NSNumber((uint)CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange));
			var settings = AVCapturePhotoSettings.FromFormat(format);
			CameraRenderer.CurrentCamera.stillImageOutput.CapturePhoto(settings, CameraRenderer.CurrentCamera.cameraProcessor);
			await Navigation.PushAsync(new ImageShowcasePage());
		}
		else
		{
			Console.WriteLine("cant take picture!");
		}


	}

}

