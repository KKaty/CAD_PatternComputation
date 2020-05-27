# CAD_PatternComputation
A C# program for processing CAD assembly models in STEP format and detectiong the patterns of repeated parts

## Usage
CAD_PatternComputation needs to be built referring the following SolidWorks dependences:
* solidworkstools.dll
* SolidWorks.Interop.sldworks.dll
* SolidWorks.Interop.swconst.dll
and adding (by NuGet) the following packages:
*Newtonsoft.Json
*Accord.Math

After built, the program (placed in RelationComputation\bin\Release) can be run from command line as:
**RelationComputation.exe CADfile Outputfile**
where
* CADfile is the absolute path of the input file
* Outputfile is the absolute path of the JSON resulted as output with no extension.

## Supported formats
The following CAD formats are supported:
* .STEP (AP203, AP214)
* .stp


## Authors
* Katia Lupinetti (CNR IMATI), Daniela Cabiddu (CNR IMATI)


## Acknowldegment
If you use CAD_InterfaceComputation in your academic projects, please consider citing the project using the following 
BibTeX entry:

```bibtex
@article{lupinetti2019content,
  title={Content-based multi-criteria similarity assessment of CAD assembly models},
  author={Lupinetti, Katia and Giannini, Franca and Monti, Marina and Pernot, Jean-Philippe},
  journal={Computers in Industry},
  volume={112},
  pages={103111},
  year={2019},
  publisher={Elsevier}
}


@inproceedings{lupinetticad3a,
  title={CAD3A: A Web-Based Application to Visualize and Semantically Enhance CAD Assembly Models},
  author={K. {Lupinetti} and D. {Cabiddu} and F. {Giannini} and M. {Monti}}, 
  booktitle={2019 15th International Conference on Signal-Image Technology   Internet-Based Systems (SITIS)},
  year={2019},
  volume={},
  number={},
  pages={462-469},
  organization={IEEE}
}
```