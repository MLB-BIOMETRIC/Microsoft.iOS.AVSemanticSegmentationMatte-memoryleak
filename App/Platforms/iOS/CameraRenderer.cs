using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using App;
using Microsoft.Maui.Controls.Compatibility;

// Custom renderer for the CameraPreview class
[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraRenderer))]
namespace App
{
    public class CameraRenderer : ViewRenderer<CameraPreview, CameraSetup>
    {
        // Reference the current camera setup.
        public static CameraSetup? CurrentCamera;

        // Called when a new element is attached to the renderer.
        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            // Ensure that the new element is not null.
            if (e.NewElement != null)
            {
                // If the Control does not exist. Then create a new CameraSetup and set it as the native control.
                // The control is the native view that is rendered on the screen.
                if (Control == null)
                {
                    CurrentCamera = new CameraSetup();
                    SetNativeControl(CurrentCamera);
                }
            }
        }
    }
}