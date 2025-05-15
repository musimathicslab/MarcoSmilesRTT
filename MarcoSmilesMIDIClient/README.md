# Installation on Mini PC Windows

*N.B. This section is for developers only!*

- Python 3.9.13;
  - numpy 1.24.3;
  - pandas 2.0.2;
  - [pythonnet](https://github.com/pythonnet/pythonnet);
- [LoopMidi](https://www.tobias-erichsen.de/software/loopmidi.html). Create a virtual MIDI port with the name `MarcoSmiles` (it's essential to give this name to the port in order to give the possibility to the MIDI management module to find this virtual MIDI port);
- [LeapMotion SDK v5.12](https://developer.leapmotion.com/tracking-software-download).

## How to use LeapSDK with Python

_References: https://github.com/seanschneeweiss/RoSeMotion_.

Unfortunately there's no official Python wrapper for Gemini V5. You’ll have to write Python bindings to the LeapC API.

You can find all the necessary files (/lib, Leap.py, LeapC++.dll, LeapC.dll, LeapPython.pyd) in the `MarcoSmilesPortable` folder.
   
## Create/Modify DLL for MidiLib
This section is useful for creating a customised Midi library. If you do not want to modify the version we created, you can find the result of the next steps in `MarcoSmilesPortable/ddls`.
   
1. Install Visual Studio.
2. Create a new project of type `Class Library(.NET Standard)`.
3. Copy and paste the content of the file `Midi_Library_File.cs` placed in `MarcoSmilesPortable/dlls`.
4. Build the project and locate the dll created (the path can be found in the output console).
5. Copy the `MidiLib.dll` in the `MarcoSmilesPortable/dlls` folder. 
6. You can use it in a Python script in this way.
      
   ```
   import clr
   clr.AddReference('dlls/MidiLib')
   from MidiLib import MidiClass
   ```  
   Where MidiLib is the name of the dll file (_and of the namespace_) and MidiClass is the name of the class that you want to import.
7. At this point you can instantiate an object of that class in this way and access to all the public methods.
   
   ```    
   Midi = MidiClass()
   ```
   MidiClass offer two methods:
   1. `FindMidi()` that allow to retrieve the MIDI port of MarcoSmiles and return an object of the type `OutputDevice`.
   2. `SendEvent(int note,int octave,OutputDevice outDev, string command)` that allow to send note_on/note_off message on a MIDI output port:
      1. note --> 0 to 24 (_represent the key pressed on a piano keyboard of 24 keys_) 
      2. octave --> 0 to 9 (_represent the starting octave_)
      3. outDev --> (_represent the midi output device to which you want to send the message_)
      4. command --> on/off (_represent the command of note_on/note_off_)

   In order to use this class you need to create a virtual port named `MarcoSmiles` on LoopMidi software.
   You can check if it is working correctly using a DAW (for example [FLStudio](https://www.image-line.com/)), selecting the MarcoSmiles MIDI port and running the script Dlltest.py.


## Use MarcoSmiles Portable
1. Download the portable version of MarcoSmiles placed in `MarcoSmilesPortable`.
2. Import the dataset exported from MarcoSmiles5.0, pasting the exported files in the `utils` folder.
   1. Exporting the dataset from MarcoSmiles generates the following directory. Since the MarcoSmilesPortable allows the use of only one Leap Motion, MarcoSmiles5.0 trains the neural network using two Leap Motion Controllers and simultaneously with only one device. 
   <p align="center"><img src="../readme_images/exported_dataset.png"/></p>
   
   Many of these files are not necessary for the operation of MarcoSmiles Portable, and it is therefore necessary to clean the directory of unused files. At system start-up, therefore, the imported dataset will be cleaned (using the `clean_utils_dir()` function in `predict.py`) leaving the following files:
   - lbl_notes.txt;
   - min&max_values_dataset_out_1H.txt;
   - model_1H.pkl;

3. Go into your terminal and execute the following instructions (make sure you have connected the Leap Motion):
   
   ```
   cd MarcoSmilesPortable
   ```
   ```
   python LeapEventListener.py
   ```