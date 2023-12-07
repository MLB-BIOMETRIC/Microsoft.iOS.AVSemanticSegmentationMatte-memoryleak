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

            try
            {
                using var pool = new NSAutoreleasePool();
                if (error != null)
                {
                    Console.WriteLine(error);
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                using var cgimagerep = photo.CGImageRepresentation;
                if (cgimagerep == null)
                {
                    Console.WriteLine("Photo taken is null!");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                using var rotatedCIImage = new CIImage(cgimagerep);
                var rotatedImage = rotatedCIImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

                using var uiimage = new UIImage(rotatedImage);
                if (uiimage == null)
                {
                    Console.WriteLine("Could not create UIImage");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                using var jpeg = uiimage.AsJPEG(0.5f);
                var uiimagedata = jpeg?.ToArray();
                if (uiimagedata == null)
                {
                    Console.WriteLine("UIImagedata is null");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                using var hairmaskraw = photo.GetSemanticSegmentationMatte(AVSemanticSegmentationMatteType.Hair);

                if (hairmaskraw == null)
                {
                    Console.WriteLine("Hairmask raw is null");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                using var cihairmask = new CIImage(hairmaskraw);
                using var hairMask = cihairmask.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

                if (hairMask == null)
                {
                    Console.WriteLine("Hairmask CIImage is null");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                using var hairmaskuiimage = new UIImage(hairMask);
                if (hairmaskuiimage == null)
                {
                    Console.WriteLine("Could not create UIImage");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                using var hairmaskjpeg = hairmaskuiimage.AsJPEG(0.5f);
                var hairmaskuiimagedata = hairmaskjpeg?.ToArray();

                if (hairmaskuiimagedata == null)
                {
                    Console.WriteLine("UIImagedata is null");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                FinishedProcessingPhotoEvent?.Invoke(uiimagedata, hairmaskuiimagedata);

            }
            finally
            {
                photo.Dispose();
            }
        }
    }
}
