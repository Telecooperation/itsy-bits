<h1 align="center">Itsy-Bits</h1>
<p align="center">Fabrication and Recognition of 3D-Printed Tangibles with Small Footprints on Capacitive Touchscreens</p>

<div align="center">
<img src="/assets/teaser.png" width="80%">
  <br />
    <!--<a href="https://github.com/othneildrew/Best-README-Template"><strong>Explore the docs »</strong></a>
    <br />
    <br />-->
  <h4 align="center">
    <a href="https://youtu.be/WrdRQlt2fsA">Watch Teaser</a>
    ·
    <a href="/assets/schmitz2021itsybits.pdf">View PDF</a>
    ·
    <a href="https://youtu.be/fI3zZz4fnMY">Watch Talk</a>
    ·
    <a href="https://doi.org/10.1145/3411764.3445502">Follow DOI</a>
    </h4>
</div>



## Paper Abstract
Tangibles on capacitive touchscreens are a promising approach to overcome the limited expressiveness of touch input. While research has suggested many approaches to detect tangibles, the corresponding tangibles are either costly or have a considerable minimal size. This makes them bulky and unattractive for many applications. At the same time, they obscure valuable display space for interaction. 

To address these shortcomings, we contribute Itsy-Bits: a fabrication pipeline for 3D printing and recognition of tangibles on capacitive touchscreens with a footprint as small as a fngertip. Each Itsy-Bit consists of an enclosing 3D object and a unique conductive 2D shape on its bottom. Using only raw data of commodity capacitive touchscreens, Itsy-Bits reliably identifes and locates a variety of shapes in diferent sizes and estimates their orientation. Through example applications and a technical evaluation, we demonstrate the feasibility and applicability of Itsy-Bits for tangibles with small footprints.

## How to use
We provide the models and code to print and detect Itsy-Bits on adequate capacitive screens, as well as the tool chain to capture ground-truth capacitive raw data and learn new models. 
We also provide the 3D models used for the example applications in the paper.

### 3D Models
The [3Dmodels](/3Dmodels/) folder contains all printable models used for the study and the example applications.

### Dataset
The [Dataset](/Dataset/) folder contain the ground-truth data sets of capacitive raw data for each shape in each size. Please see [Dataset/README.md](/Dataset/README.md) for more details on the data sets.


### Capture Pipeline
The folder [CapturePipeline](/CapturePipeline/) contains all code necessary for the study procedure, to capture Optitrack data on PC, to record capacitive raw data on an Android device, and to merge both log files based on timestamps.


## How to cite
Please cite this work like this:
<pre>
Martin Schmitz, Florian Müller, Max Mühlhäuser, Jan Riemann, and Huy Viet Le. 2021. 
Itsy-Bits: Fabrication and Recognition of 3D-Printed Tangibles with Small Footprints on Capacitive Touchscreens. In <i>Proceedings of the 2021 CHI Conference on Human Factors in Computing Systems</i> (<i>CHI '21</i>). Association for Computing Machinery, New York, NY, USA, Article 419, 1–12. https://doi.org/10.1145/3411764.3445502
</pre>


or use this

<pre>
@inproceedings{10.1145/3411764.3445502,
author = {Schmitz, Martin and M\"{u}ller, Florian and M\"{u}hlh\"{a}user, Max and Riemann, Jan and Le, Huy Viet},
title = {Itsy-Bits: Fabrication and Recognition of 3D-Printed Tangibles with Small Footprints on Capacitive Touchscreens},
year = {2021},
isbn = {9781450380966},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3411764.3445502},
doi = {10.1145/3411764.3445502},
booktitle = {Proceedings of the 2021 CHI Conference on Human Factors in Computing Systems},
articleno = {419},
numpages = {12},
keywords = {3D Printing, Machine Learning, Touchscreen, Tangibles},
location = {Yokohama, Japan},
series = {CHI '21}
}

</pre>
