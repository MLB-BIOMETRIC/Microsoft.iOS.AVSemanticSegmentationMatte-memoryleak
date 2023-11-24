using AVFoundation;
using CoreImage;
using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

namespace App
{

    public delegate void FinishedProcessingPhoto(byte[] photodata);

    public class CameraProcessor : AVCapturePhotoCaptureDelegate
    {

        public event FinishedProcessingPhoto? FinishedProcessingPhotoEvent;

        public override void DidFinishProcessingPhoto(AVCapturePhotoOutput output, AVCapturePhoto photo, NSError? error)
        {

            if (error != null)
            {
                Console.WriteLine(error);
                return;
            }

            if (photo.CGImageRepresentation == null)
            {
                Console.WriteLine("Photo taken is null!");
                FinishedProcessingPhotoEvent?.Invoke([]);
                return;
            }

            var rotatedImage = new CIImage(photo.CGImageRepresentation).CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

            var uiimage = new UIImage(rotatedImage);

            if (uiimage == null)
            {
                Console.WriteLine("Could not create UIImage");
                FinishedProcessingPhotoEvent?.Invoke([]);
                return;
            }

            Console.WriteLine("didFinishProcessingPhoto");

            // Send the photo back to the showcasepage
            FinishedProcessingPhotoEvent?.Invoke(uiimage.AsJPEG(0.5f).ToArray());
        }

        public override void DidCapturePhoto(AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
        {
            Console.WriteLine("didCapturePhoto!");
        }
    }
}
