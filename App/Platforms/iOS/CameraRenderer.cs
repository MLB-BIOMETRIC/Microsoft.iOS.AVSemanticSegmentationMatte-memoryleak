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

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            Console.WriteLine("ON ELEMENT CHANGED");
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    // Create your native UIView here
                    var currentCamera = new CameraSetup();
                    // Configure your UIView as needed

                    SetNativeControl(currentCamera);
                }
            }
        }
    }
}