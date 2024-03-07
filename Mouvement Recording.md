# Image Recording

In order to be analyzed by the system later, the first 11.38 seconds of the user's interaction are recorded in a 64 by 64 pixel image. The choice of what is recorded has not changed since the prototype, which implies that it does not take into account the position in depth of the various body parts, but only the position seen by the camera facing the video projection. The red and green colors correspond to the x and y positions of the body parts. With the exception of the hips, all these values are relative to the position of the center of the hip (from -1. to 1., to 0. to 1. to avoid negative values). A ratio calculated from the distance between the shoulders and the hips is also applied to these values, so that each movement stays at the same amplitude regardless of its distance of the body from the camera.
Finally, the blue color is equivalent to the model's detection score (the closer the value is to 1, the more certain the model is of having detected a face).


Here are some images recorded during the presentation of the intallation at the Sõltumatu Tantsu Lava in Tallinn.

![capture0001](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/4bedeca8-c50a-4340-919f-b2c4c71077de)
![capture0127](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/c2e72432-d70b-4c3e-82d6-5aeab3705cf4)
![capture0006](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/e4e2d8d7-da8d-43a2-9a57-e5fd09b1d154)

All registered images can be found on this git at this location : https://github.com/modina-eu/Temporal-Spaces/tree/main/Detection-Light/temporal/Assets/StreamingAssets/Capture

This system is designed to capture real-time information on the movement of people in the interactive space, for use later in the experience. However, to make it easier to test and generate content in advance, a version capturing the same data on a video has been implemented. Below is an example video of the system (mocap with PosNet) recording the position of the nose position (wich is used which is used to position the head in the project).

https://github.com/modina-eu/Temporal-Spaces/assets/43936968/73d4937f-c480-46cc-98c8-e7e639ccdea2

Data is recorded along the x coordinates of the image, and once a line is completed, writing shifts by one pixel on y coordinates until the entire image is recorded.

Below you can see a vizualization in red of the order in which the pixels are currently read in the app. 

![readingPattern](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/e290077e-128c-45c7-b553-27c62d351117)

Below is an example of a script for reading data form the exemple above:
``` HLSL
// _resx, _resy are respectivly the resolution on x and y of the resulte image (here 64 for each)
// actually the _time float is not equal to the application time but to the frameCount wich is clamped at 30 fps

float2 u2 = float2(frac(_time  / _resx), frac((_time )/(_resx* _resyx)));
float3 result = tex2D(_PosTex, u2).xyz;
```

And here there is the code wich is use to write the texture of this exemple:
``` HLSL
Texture2D<float4> reader; 
RWTexture2D<float4> writer;
SamplerState _pointClamp; 
// _resx, _res, _time are the same that in the previous script
float _resx;
float _resy;
float _time;
// _pos contain the information of the x,y position of the head and on z the score of the detection (w is equal at 0)
float4 _pos;
[numthreads(8,8,1)]
void CSMain (uint2 id : SV_DispatchThreadID) 
{
	// coordonate of the pixels
	float2 f = float2(id.x,id.y);
	// resolution of the image
	float2 res=float2(_resx, _resy);
	// normalize coordonate
	float2 uv = f / res;
	// mask of the positon on x of the currently written pixel
	float mask1 = step(frac(_time / _resx), uv.x);
	// mask positon on y of the currently written pixel
	float mask2 = step(frac((_time -(_resx-1.)) / (_resx* _resy)),uv.y+1./_resy);
	// previous coordonates writed on the buffer
	float4 previous = reader.SampleLevel(_pointClamp, uv + 0.5 / res, 0);
	// adding the current coordonates the previous ones accordingly to the mask
	float4 result = lerp(previous, _pos,mask1*mask2);
	writer[id] = result;
}

```

For this project 12 body parts were recorded on the texture, these are respectively and in the order in which they are recorded (from bottom to top on the image) : 0-head, 1-leftShoulder, 2-rightShoulder, 3-leftElbow, 4-rightElbow, 5-leftWrist, 6-rightWrist, 7-hip, 8-leftKnee, 9-rightKnee, 10-leftAnkle, 11-rightAnkle.

Below is a small graphic example of how these textures are read (each color is a different body part).

![readingPattern02](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/75716277-bc68-4abc-96bb-8ab6d2df9c20)

And another small capture showing the recording of all the points on the texture, with the system applied to a video (from a recording of a Max Levy training session taken in 2021).

https://github.com/modina-eu/Temporal-Spaces/assets/43936968/a7b90523-b579-435f-9c4f-b161c40912e0

We can see a clear difference on the 7th line from the bottom, because it's the only one that's a screen space position (the hips). 
The function for reading the vapors inscribed on the texture is similar to the one above, with the addition of a value (v here) to select the body part of interest.

``` HLSL
//with v to choose the body part (0 for the head, 7 for the hip, 11 for the rightAnkle, ect...)
  vec2 tc(float time, float _resx2, float v, float itn, float it) {
      return vec2(fract(time / _resx2), 1.-(fract(time / _resx2 / floor(_resx2 / 12.)) / it+v/it));
    }
```
An illustrative vvvv patch showing how to read the texture can be found on this git here : https://github.com/modina-eu/Temporal-Spaces/tree/main/Patchvvvv-BodyViz

it's made up of two parts, the first (and main) one in blue is used to output an array of 24 floats equivalent to the x and y positions of the 12 body parts. The second, in green, is just a preview of the data.
For this example, the score is not displayed, but it can be retrieved by creating a new set of floats corresponding to the texture's z-value.
For aesthetic purposes, a small translation is made between the positions via the "ta" value, this can be removed to simplify the system. 
If you don't want to download vvvv to see this example, you can look directly at the https://github.com/modina-eu/Temporal-Spaces/blob/main/Patchvvvv-BodyViz/shaders/PixelReading_ComputeFX.sdsl file to see the implementation in question.

![image](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/6978f55e-86c4-4aa7-855b-e58c46df7684)

This example is still very raw, in the project these same data are sent in 60 fps and are processed to avoid jitter then sent via osc to other the machines.
The patch doing this can be found here( coming soon :))

Also, to avoid the risk of synchronization errors, the positions are also recorded on each frame of the video recording in the form of a pixel strip at the top right of the image (just the x,y position at each frame, you can see an exemple bellow).
![image](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/212a6d38-e7c5-49f0-8445-297b3dfbcc92)


Another useful resource would be the visualization page https://louiscortes.github.io/Modina-model-viz/
contained on a small git https://github.com/LouisCortes/Modina-model-viz
Originally created to visualize the results of texture generation, it has since been slightly expanded to also visualize all the images recorded during the initial project presentation (click in this case on the "select record texture" button then choose the desired texture number with the slider below. The line display has been modified on this one to be similar to that seen during installation.

