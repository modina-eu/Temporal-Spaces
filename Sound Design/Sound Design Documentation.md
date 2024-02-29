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

Upon arriving for the official residency period of MODINA in Jan 2024, my focus came immediately to reconsidering which motion-captured values would be the most useful. When building the OSC Bridge (see documentation), I took this chance to calculate the speed/per frame of each Vector3; rather than work with 3 values per joint at each frame, the values were condensed per frame into a single value of its distance since the previous frame. 

This was quite an important realization and method change, not only from a technical perspective on reducing excessive values, but from a user-experience perspective: movement over time is the basis of the installation: no movement should make no sound, slow and fast movements should each have their respective feedback.



