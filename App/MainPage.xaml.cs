using AVFoundation;

namespace App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		Console.WriteLine("MAINPAGE INIT()");
	}

	private void photocapture_clicked(object sender, EventArgs e)
	{
		Console.WriteLine("photocapture_clicked clicked!");

		CameraPreview.StartCamera();

	}

}

