using AVFoundation;
using CoreVideo;
using Foundation;

namespace App;

public partial class ImageShowcasePage : ContentPage
{

    public ImageShowcasePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        if (CameraRenderer.CurrentCamera != null)
        {
            CameraRenderer.CurrentCamera.cameraProcessor.FinishedProcessingPhotoEvent += showPhotoTaken;
        }
    }

    protected override void OnDisappearing()
    {
        if (CameraRenderer.CurrentCamera != null)
        {
            CameraRenderer.CurrentCamera.cameraProcessor.FinishedProcessingPhotoEvent -= showPhotoTaken;
        }
        Photo.Source = null;
    }

    private async void goback_button(object sender, EventArgs e)
    {
        Console.WriteLine("Go back!");
        await Navigation.PopAsync();
    }

    private void showPhotoTaken(byte[] photodata)
    {
        Photo.Source = ImageSource.FromStream(() => new MemoryStream(photodata));
        Console.WriteLine("completely done!");
    }

}

