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
            // Subscribe to the FinishedProcessingPhotoEvent event.
            CameraRenderer.CurrentCamera.cameraProcessor.FinishedProcessingPhotoEvent += ShowPhotoTaken;
        }
    }

    protected override void OnDisappearing()
    {
        if (CameraRenderer.CurrentCamera != null)
        {
            // Unsubscribe from the FinishedProcessingPhotoEvent event.
            CameraRenderer.CurrentCamera.cameraProcessor.FinishedProcessingPhotoEvent -= ShowPhotoTaken;
        }
        Photo.Source = null;
        HairMask.Source = null;
    }

    private async void Goback_button(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    /// <summary>
    /// Helper methdo to show the photo taken.
    /// </summary>
    private void ShowPhotoTaken(byte[]? photodata, byte[]? hairmaskdata)
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

