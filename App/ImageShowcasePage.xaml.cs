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
        HairMask.Source = null;
    }

    private async void goback_button(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void showPhotoTaken(byte[] photodata, byte[] hairmaskdata)
    {

        if (photodata != null)
        {
            Photo.Source = ImageSource.FromStream(() => new MemoryStream(photodata));
            MainImageText.IsVisible = false;
        }
        else
        {
            MainImageText.Text = "Error";
        }

        if (hairmaskdata != null)
        {
            HairMask.Source = ImageSource.FromStream(() => new MemoryStream(hairmaskdata));
            HairMaskText.IsVisible = false;
        }
        else
        {
            HairMaskText.Text = "Error";
        }

    }

}

