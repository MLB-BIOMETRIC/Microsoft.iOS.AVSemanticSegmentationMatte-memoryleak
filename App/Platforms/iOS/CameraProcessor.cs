using AVFoundation;
using CoreImage;
using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

namespace App
{

    public delegate void FinishedProcessingPhoto(byte[]? photodata, byte[]? maskdata);

    public class CameraProcessor : AVCapturePhotoCaptureDelegate
    {

        public event FinishedProcessingPhoto? FinishedProcessingPhotoEvent;

        public override void DidFinishProcessingPhoto(AVCapturePhotoOutput output, AVCapturePhoto photo, NSError? error)
        {

            if (error != null)
            {
                Console.WriteLine(error);
                FinishedProcessingPhotoEvent?.Invoke(null, null);
                return;
            }

            if (photo.CGImageRepresentation == null)
            {
                Console.WriteLine("Photo taken is null!");
                FinishedProcessingPhotoEvent?.Invoke(null, null);
                return;
            }

            var rotatedImage = new CIImage(photo.CGImageRepresentation).CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

            var uiimage = new UIImage(rotatedImage);

            if (uiimage == null)
            {
                Console.WriteLine("Could not create UIImage");
                FinishedProcessingPhotoEvent?.Invoke(null, null);
                return;
            }

            var uiimagedata = uiimage.AsJPEG(0.5f)?.ToArray();

            if (uiimagedata == null)
            {
                Console.WriteLine("UIImagedata is null");
                FinishedProcessingPhotoEvent?.Invoke(null, null);
                return;
            }

            var hairmaskraw = photo.GetSemanticSegmentationMatte(AVSemanticSegmentationMatteType.Hair);

            if (hairmaskraw == null)
            {
                Console.WriteLine("Hairmask raw is null");
                FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                return;
            }

            var hairMask = new CIImage(hairmaskraw).CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

            if (hairMask == null)
            {
                Console.WriteLine("Hairmask CIImage is null");
                FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                return;
            }

            var hairmaskuiimage = new UIImage(hairMask);

            if (hairmaskuiimage == null)
            {
                Console.WriteLine("Could not create UIImage");
                FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                return;
            }

            var hairmaskuiimagedata = hairmaskuiimage.AsJPEG(0.5f)?.ToArray();

            if (hairmaskuiimagedata == null)
            {
                Console.WriteLine("UIImagedata is null");
                FinishedProcessingPhotoEvent?.Invoke(null, null);
                return;
            }

            // Send the photo back to the showcasepage
            FinishedProcessingPhotoEvent?.Invoke(uiimagedata, hairmaskuiimagedata);
        }
    }
}
