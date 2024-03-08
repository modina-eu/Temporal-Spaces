# Mocap system

An executable can be found here : https://github.com/modina-eu/Temporal-Spaces/tree/main/BuildMocap

This retrieves the stream from your webcam to feed a poseNet model.
The detection result is then sent locally to osc via port 1111.

Currently, the build returns the coordinates of 3 people simultaneously.
All accessible addresses are listed below:
<details>
  <summary>Click to expand/collapse the adresses </summary>
  
 	public string address2 = "/P1HeadY";
    	public string address3 = "/P1HeadS";
    	public string address4 = "/P1ShoulderLeftX";
    	public string address5 = "/P1ShoulderLeftY";
    	public string address6 = "/P1ShoulderLeftS";
    	public string address7 = "/P1ShoulderRightX";
    	public string address8 = "/P1ShoulderRightY";
    	public string address9 = "/P1ShoulderRightS";
    	public string address10 = "/P1ElbowLeftX";
    	public string address11 = "/P1ElbowLeftY";
    	public string address12 = "/P1ElbowLeftS";
    	public string address13 = "/P1ElbowRightX";
    	public string address14 = "/P1ElbowRightY";
    	public string address15 = "/P1ElbowRightS";
    	public string address16 = "/P1HandLeftX";
    	public string address17 = "/P1HandLeftY";
    	public string address18 = "/P1HandLeftS";
    	public string address19 = "/P1HandRightX";
    	public string address20 = "/P1HandRightY";
    	public string address21 = "/P1HandRightS";
    	public string address22 = "/P1HipX";
    	public string address23 = "/P1HipY";
    	public string address24 = "/P1HipS";
    	public string address25 = "/P1KneeLeftX";
    	public string address26 = "/P1KneeLeftY";
    	public string address27 = "/P1KneeLeftS";
    	public string address28 = "/P1KneeRightX";
    	public string address29 = "/P1KneeRightY";
    	public string address30 = "/P1KneeRightS";
    	public string address31 = "/P1AnkleLeftX";
    	public string address32 = "/P1AnkleLeftY";
    	public string address33 = "/P1AnkleLeftS";
    	public string address34 = "/P1AnkleRightX";
    	public string address35 = "/P1AnkleRightY";
    	public string address36 = "/P1AnkleRightS";


    	public string address58 = "/P2HeadX";
    	public string address59 = "/P2HeadY";
    	public string address61 = "/P2ElbowLeftX";
    	public string address62 = "/P2ElbowLeftY";
    	public string address64 = "/P2ElbowRightX";
    	public string address65 = "/P2ElbowRightY";
    	public string address67 = "/P2HandLeftX";
    	public string address68 = "/P2HandLeftY";
    	public string address70 = "/P2HandRightX";
    	public string address71 = "/P2HandRightY";
    	public string address73 = "/P2HipX";
    	public string address74 = "/P2HipY";
    	public string address76 = "/P2KneeLeftX";
    	public string address77 = "/P2KneeLeftY";
    	public string address79 = "/P2KneeRightX";
    	public string address80 = "/P2KneeRightY";
    	public string address82 = "/P2AnkleLeftX";
    	public string address83 = "/P2AnkleLeftY";
    	public string address85 = "/P2AnkleRightX";
    	public string address86 = "/P2AnkleRightY";
    	public string address88 = "/P2HeadS";
    	public string address89 = "/P2ElbowLeftS";
    	public string address90 = "/P2ElbowRightS";
    	public string address91 = "/P2HandLeftS";
    	public string address92 = "/P2HandRightS";
    	public string address93 = "/P2HipS";
    	public string address94 = "/P2KneeLeftS";
    	public string address95 = "/P2KneeRightS";
    	public string address96 = "/P2AnkleLeftS";
    	public string address97 = "/P2AnkleRightS";

    	public string address98 = "/P3HeadX";
    	public string address99 = "/P3HeadY";
    	public string address101 = "/P3ElbowLeftX";
    	public string address102 = "/P3ElbowLeftY";
    	public string address104 = "/P3ElbowRightX";
    	public string address105 = "/P3ElbowRightY";
    	public string address107 = "/P3HandLeftX";
    	public string address108 = "/P3HandLeftY";
    	public string address110 = "/P3HandRightX";
    	public string address111 = "/P3HandRightY";
    	public string address113 = "/P3HipX";
    	public string address114 = "/P3HipY";
    	public string address116 = "/P3KneeLeftX";
    	public string address117 = "/P3KneeLeftY";
    	public string address119 = "/P3KneeRightX";
    	public string address120 = "/P3KneeRightY";
    	public string address122 = "/P3AnkleLeftX";
    	public string address123 = "/P3AnkleLeftY";
    	public string address125 = "/P3AnkleRightX";
    	public string address126 = "/P3AnkleRightY";
    	public string address128 = "/P3HeadS";
    	public string address129 = "/P3ElbowLeftS";
    	public string address130 = "/P3ElbowRightS";
    	public string address131 = "/P3HandLeftS";
    	public string address132 = "/P3HandRightS";
    	public string address133 = "/P3HipS";
    	public string address134 = "/P3KneeLeftS";
    	public string address135 = "/P3KneeRightS";
    	public string address136 = "/P3AnkleLeftS";
    	public string address137 = "/P3AnkleRightS";

    	public string address138 = "/P1Check";
    	public string address139 = "/P2Check";
    	public string address140 = "/P3Check";

</details>

Each element starting with "P1", "P2", "P3" is information relating to the first, second and third person detected respectively.
"X" and "Y" at the end refer respectively to the horizontal and vertical position of the body parts.
finally "S" at the end refer to the detection score (the closer the value is to 1, the more certain the model is of having detected a face).


# Texture Recording

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

Contained on a small git https://github.com/LouisCortes/Modina-model-viz

Originally created to visualize the results of texture generation, it has since been slightly expanded to also visualize all the images recorded during the initial project presentation (click in this case on the "select record texture" button then choose the desired texture number with the slider below.

The line display has been modified on this one to be similar to that seen during installation. We felt that these lines allowed the user to see the person even when they move in areas that are too dark or when the blur (which seemed important to us to anonymize the users in the bank) distorts their silhouette too much.
Also sometimes showing aberrant detection behaviors, allowing for a better understanding of the choices the system makes.

![image](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/faa9982e-97d9-4dac-8bc8-5a211c86164e)

Above are some photos of the installation taken by Kris Moor where we can see the same kind of lines.

# Texture analysis

One of the main ones was to find in the bank of all the interactions that had taken place in this space the one that corresponded most with the one that had just taken place, so that they could be replayed together in the observing phase.
The solution integrated into the project here: https://github.com/modina-eu/Temporal-Spaces/blob/main/Detection-Light/temporal/Assets/PoseNet/Models/ResNEt/TextureComparator.cs

Was initially proposed by Andreia  ([andreianmatos
andreianmatos](https://github.com/andreianmatos)) through this repository : https://github.com/modina-eu/temporal_spaces_image_similarities

This repository demonstrates image similarity using a pre-trained ResNet50 model. It compares a current person's image with images from a dataset of people, finding the most similar image.

As mentioned above, these analyses don't take depth into account (z-axis calculated by the second webcam), notably to avoid losing elements already captured in this form, but also because the video-projection footage only comes from the first camera, we found it simpler for the observer to use only the information that can be seen visually in this "2d" projection.
On the other hand, the score (the closer the value is to 1, the more certain the model is of its detection)
has been kept to make it easier to see why certain matches are made, especially aberrant detections that end up with others having a very low score.
This match system, while seeking similarities, is also partly intended to remain open to imprecision and to the unexpected, allowing the user to trick and mislead the system.
Allowing playfulness and engaging with other kinds of movement independently of visual and sonic reactions, a bit like a kind of meta of the installation.


Finally, one of the other great interests of these textures was to generate movement scores via an AI model, enabling sounds and generative visuals to be made when no one is using the space.The little vvvv patch above was expressly designed for this purpose, but unfortunately we realized during playtesting that it was important to keep the playback of the last recorded movement to visually convey to users that the space at the center was an interactive space, and to encourage them to do the same when they saw other people interacting, as a kind of virtuous loop. This sudden direction didn't allow us to use this part of the research carried out during the residency as we'd hoped, leaving it full of interesting potential.

Andreia Matos' contribution was also essential for this part of the project, via the solutions proposed on this git: https://github.com/modina-eu/temporal_spaces_texture_gen

With this notebook for overall visual comparison: https://github.com/modina-eu/temporal_spaces_texture_gen/blob/main/notebooks/results_comparison.ipynb

The results of this research can be seen on this page: https://modina-eu.github.io/temporal_spaces_texture_gen/

A small graphic and sound visualization can be seen by default on the visualization page https://louiscortes.github.io/Modina-model-viz/

![image](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/bbc24a7f-e1d9-4373-a79a-30e10e1cf69b)

