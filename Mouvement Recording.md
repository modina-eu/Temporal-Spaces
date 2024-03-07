# Image Recording

In order to be analyzed by the system later, the first 11.38 seconds of the user's interaction are recorded in a 64 by 64 pixel image. The choice of what is recorded has not changed since the prototype, which implies that it does not take into account the position in depth of the various body parts, but only the position seen by the camera facing the video projection. The red and green colors correspond to the x and y positions of the body parts. With the exception of the hips, all these values are relative to the position of the center of the hip (from -1. to 1., to 0. to 1. to avoid negative values). A ratio calculated from the distance between the shoulders and the hips is also applied to these values, so that each movement stays at the same amplitude regardless of its distance of the body from the camera.
Finally, the blue color is equivalent to the model's detection score (the closer the value is to 1, the more certain the model is of having detected a face).


Here are some images recorded during the presentation of the intallation at the Sõltumatu Tantsu Lava in Tallinn.

![capture0001](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/4bedeca8-c50a-4340-919f-b2c4c71077de)
![capture0127](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/c2e72432-d70b-4c3e-82d6-5aeab3705cf4)
![capture0006](https://github.com/modina-eu/Temporal-Spaces/assets/43936968/e4e2d8d7-da8d-43a2-9a57-e5fd09b1d154)

All registered images can be found on this git at this location : https://github.com/modina-eu/Temporal-Spaces/tree/main/Detection-Light/temporal/Assets/StreamingAssets/Capture

This system is designed to capture real-time information on the movement of people in the interactive space, for use later in the experience. However, to make it easier to test and generate content in advance, a version capturing the same data in relation to a video has been implemented.
