using AVFoundation;
using CoreImage;
using Foundation;
using UIKit;

namespace App
{
    /// <summary>
    /// Delegate that will be called when the photo is taken and processed.
    /// </summary>
    public delegate void FinishedProcessingPhoto(byte[]? photodata, byte[]? maskdata);

    public class CameraProcessor : AVCapturePhotoCaptureDelegate
    {

        /// <summary>
        /// Event that will inform the UI when the photo is taken and processed.
        /// </summary>
        public event FinishedProcessingPhoto? FinishedProcessingPhotoEvent;

        /// <summary>
        /// This method is called when the photo is taken and processed.
        /// </summary>
        public override void DidFinishProcessingPhoto(AVCapturePhotoOutput output, AVCapturePhoto photo, NSError? error)
        {
            try
            {
                // Create a new autorelease pool to ensure unmanaged objects are released.
                using var pool = new NSAutoreleasePool();
                if (error != null)
                {
                    Console.WriteLine(error);
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                // Get the CGImage representation of the photo taken.
                using var cgimagerep = photo.CGImageRepresentation;
                if (cgimagerep == null)
                {
                    Console.WriteLine("Photo taken is null!");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                // Rotate the image so its upright
                using var rotatedCIImage = new CIImage(cgimagerep);
                var rotatedImage = rotatedCIImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

                // Create the UIImage from the rotated image
                using var uiimage = new UIImage(rotatedImage);
                if (uiimage == null)
                {
                    Console.WriteLine("Could not create UIImage");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                // Convert it to a JPG
                using var jpeg = uiimage.AsJPEG(0.5f);
                var uiimagedata = jpeg?.ToArray();
                if (uiimagedata == null)
                {
                    Console.WriteLine("UIImagedata is null");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                // Get the raw hair mask from the photo.
                using var hairmaskraw = photo.GetSemanticSegmentationMatte(AVSemanticSegmentationMatteType.Hair);

                if (hairmaskraw == null)
                {
                    Console.WriteLine("Hairmask raw is null");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                // Get the actual image of the hair mask.
                if (hairmaskraw.MattingImage == null)
                {
                    Console.WriteLine("Hair mask matting image is null");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                // Create a CIImage of the hair mask and rotate it upright.
                using var cihairmask = new CIImage(hairmaskraw.MattingImage);
                using var hairMask = cihairmask.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);

                if (hairMask == null)
                {
                    Console.WriteLine("Hairmask CIImage is null");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                // Create a UIImage of the hair mask so its ready to displayed on the UI.
                using var hairmaskuiimage = new UIImage(hairMask);
                if (hairmaskuiimage == null)
                {
                    Console.WriteLine("Could not create UIImage");
                    FinishedProcessingPhotoEvent?.Invoke(uiimagedata, null);
                    return;
                }

                // Conver to JPG
                using var hairmaskjpeg = hairmaskuiimage.AsJPEG(0.5f);
                var hairmaskuiimagedata = hairmaskjpeg?.ToArray();

                if (hairmaskuiimagedata == null)
                {
                    Console.WriteLine("UIImagedata is null");
                    FinishedProcessingPhotoEvent?.Invoke(null, null);
                    return;
                }

                // Return with the photo and the hair mask. Ready to display on the UI.
                FinishedProcessingPhotoEvent?.Invoke(uiimagedata, hairmaskuiimagedata);

            }
            finally
            {
                // Finally dispose the photo taken to free up memory.
                photo.Dispose();
            }
        }
    }
}
