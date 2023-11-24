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