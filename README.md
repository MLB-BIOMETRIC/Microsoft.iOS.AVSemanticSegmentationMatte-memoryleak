# MEMORY LEAK FIXED

The memory leak can be fixed by using the MattingImage instead of the hairmaskraw directly!

```csharp
using var cihairmask = new CIImage(hairmaskraw.MattingImage);
```

Read more here: https://github.com/xamarin/xamarin-macios/issues/19518#issuecomment-1840364389

# Microsoft.iOS.AVSemanticSegmentationMatte-memoryleak

This repo reproduces the AVSemanticSegmentationMatte memory leak of 2,98 MB.

This memory leak specifically happens in the code below:

```csharp
var hairmaskraw = photo.GetSemanticSegmentationMatte(AVSemanticSegmentationMatteType.Hair);
```

Furthermore, there is also a bug where the code line above only works for the first 4 images. Then it stops working and returns null for images taken after.

## Apple Instruments

Use Apple Instruments software and run an "Allocation" test on the repo. This will show the the memory leak.

![Instruments with 4 memory leaks](/Images/InstrumentsMemoryLeakHairMask.png)