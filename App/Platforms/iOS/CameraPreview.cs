using Microsoft.Maui.Controls;

namespace App
{
    public class CameraPreview : View
    {
        // Custom properties and methods (if any)

        public CameraPreview()
        {
            Console.WriteLine("CAMERAPREVIEW INIT()");
        }

        public static void StartCamera()
        {
            Console.WriteLine("starting camera");
        }
    }
}
