# SOUND DESIGN

Contribution: Max Levy

## Introduction

This text is a documentation of my contribution to Temporal Spaces's Sound Design, both in its technical and creative areas. 

## VCVRack

In working with OSC as a protocol that would function well for fast-changing values of motion capture, I decided early on to study and use VCV Rack's modular approach. I was aware of other programs to sample, but I committed quickly to picking a program and using it, to save time and work within whatever limitations presented itself. Note: in the future, I would definitely take more time to sample Max/MSP as an alternative, as we ended up using a Max patch to accommodate our application of RAVE.

**About VCVRack**: VCV Rack is a free open-source virtual modular synthesizer: multiple modules can be connected to synthesize a sound. By default, the software contains several VCOs, LFOs, mixers, and other standard synthesizer modules. However, more can be added as plugins through the VCV Rack website.

https://vcvrack.com/

## First Hurdles

The first hurdles were simply deciding what motion capture values to send via the OSC signals, and how these values would interact with the instruments and modulations of the patch. The entire process became a feedback loop of working with a given value, designing sounds around it, re-routing modulation, and iterating upon it. This was 

Initial tests with VCV Rack during the summer and early Fall had me taking individual Float data from specfic Vector3 coordinates, splitting X, Y, and Z into their values and mapping them to various modulations.

e.g. 
LeftHandX -> Oscillator Feedback
LeftHandY -> VCF Cutoff
LeftHandZ -> Oscillator Pitch/Oct

The first experiments utilized 24 separate values: Vector3's from each hand, each elbow, each knee, and each foot. Suffice to say, extrapolating these separate floats, especially from more than one tracked individual, would prove chaotic and overwhelming when trying to map eache one to separate audio modulations. The results of using these values and the modulations relationships speak for themslves:

INSERT CCL AUDIO TEST

## Experimentation in Sound Design

After the first dip at the CCL with VCV, I spent the months of Fall 2024 stepping away from the motion-tracking elements to learn the elementary blocks of sound design in VCV. This exploration was self-guided with various online tutorials, and would vary greatly depending on one's artistic interests. As such, I will not about this at lnegth. The results of some of these test patches can be found below.

INSERT EXP PATCHES

## Arrival at MODINA//Re-evaluating OSC Values


## First prototype

This first research prototype produced with ***[VVVV gamma](https://visualprogramming.net/)*** incorporates the basic system of recording in the first instance and replay in the second.

Recording takes the form of a sequence of images that is then played back by an interactive player.

https://github.com/Cosamentale/TemporalSpace_Documentation/assets/83541800/515d5a11-5972-4aba-9622-aabf09c9f2c4

Here is the GitHub link to the prototype:

<https://github.com/Cosamentale/Sound_TemporalSpace>

The replay system enables different compositions to be obtained by varying parameters such as the speed at which the recorded sequence is played back or its arrangement in space.

![image(2)](https://github.com/Cosamentale/TemporalSpace_Documentation/assets/83541800/ea428693-795a-49f4-a613-d9bca0571739)

![image(1)](https://github.com/Cosamentale/TemporalSpace_Documentation/assets/83541800/9097ae75-b692-461a-854e-ce97a40fdbd2)

![image](https://github.com/Cosamentale/TemporalSpace_Documentation/assets/83541800/79890c57-2618-49ce-a416-d37f5df5158a)
