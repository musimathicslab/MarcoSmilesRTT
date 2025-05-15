import clr

# This class use a Wrapper of the C# Sanford.Multimedia.Midi
clr.AddReference('dlls/MidiLib')
from MidiLib import MidiClass

Midi = MidiClass()

# Find the virtual port called "MarcoSmiles"
out_device = Midi.FindMidi()


def sendMidi(current_note, previous_note, octave=1,pause):

    if previous_note is not None:
        if current_note != previous_note:
            # No pause
            if current_note != pause:
                # Send Midi Event Off for the previous note played
                previous_note_sent = Midi.SendEvent(previous_note, octave, out_device, "off")
                print(previous_note_sent)

                # Send Midi Event On for the current note
                current_note_sent = Midi.SendEvent(current_note, octave, out_device, "on")
                print(current_note_sent)
                print("No pause.")
            else:
                # Pause
                # Send Midi Event Off for the previous note played
                previous_note_sent = Midi.SendEvent(previous_note, octave, out_device, "off")
                print(previous_note_sent)
                print("Pause.")
        else:
            print("Current note equals to previous note.")
    else:
        # No pause
        # Send Midi Event On for the current note
        if current_note != pause:
            current_note_sent = Midi.SendEvent(current_note, octave, out_device, "on")
            print(current_note_sent)
            print("No pause.")
        else:
            print("Pause.")

    print("--------")