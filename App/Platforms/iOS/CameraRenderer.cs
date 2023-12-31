using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls;
using UIKit;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using App;
using Microsoft.Maui.Controls.Compatibility;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraRenderer))]
namespace App
{
    public class CameraRenderer : ViewRenderer<CameraPreview, CameraSetup>
    {

        public static CameraSetup? CurrentCamera;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {

                    CurrentCamera = new CameraSetup();
                    // Configure your UIView as needed

                    SetNativeControl(CurrentCamera);
                }
            }
        }
    }
}