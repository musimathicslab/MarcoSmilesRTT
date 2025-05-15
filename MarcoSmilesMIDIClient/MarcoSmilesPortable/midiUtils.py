import clr

# This class use a Wrapper of the C# Sanford.Multimedia.Midi
clr.AddReference('dlls/MidiLib')
from MidiLib import MidiClass

Midi = MidiClass()

# Find the virtual port called "MarcoSmiles"
out_device = Midi.FindMidi()
last_octave=0

def sendMidi(current_note, previous_note, octave=1, pause = 9999):
    global last_octave
    if previous_note is not None:
        if current_note != previous_note:
            # No pause
            if current_note != pause:
                # Send Midi Event Off for the previous note played
                previous_note_sent = Midi.SendEvent(previous_note, octave, out_device, "off")
                print(previous_note_sent)
                last_octave=octave

                # Send Midi Event On for the current note
                current_note_sent = Midi.SendEvent(current_note, octave, out_device, "on")
                print(current_note_sent)
                print("No pause.")
            else:
                # Pause
                # Send Midi Event Off for the previous note played
                previous_note_sent = Midi.SendEvent(previous_note, last_octave, out_device, "off")
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



def sendMidiUPDATED(current_note, previous_note, octave=1, pause = 9999, is_ribattuta = False):
    global last_octave
    print(f"Current Note: {current_note}")
    print(f"Previous Note: {previous_note}")
    print(f"Octave: {octave}")
    print(f"Pause: {pause}")
    print(f"Ribattuta: {is_ribattuta}")

    if previous_note is not None:
        if current_note != previous_note:
            # No pause
            if current_note != pause:
                # Send Midi Event Off for the previous note played
                if (previous_note != pause):
                    previous_note_sent = Midi.SendEvent(previous_note, octave, out_device, "off")
                    print(previous_note_sent)
                    last_octave=octave

                # Send Midi Event On for the current note
                current_note_sent = Midi.SendEvent(current_note, octave, out_device, "on")
                print(current_note_sent)
                print("No pause.")
            else:
                # Pause
                # Send Midi Event Off for the previous note played
                if(previous_note != pause):
                    print(previous_note)
                    previous_note_sent = Midi.SendEvent(previous_note, last_octave, out_device, "off")
                
                print("Pause.")
        else:
            if is_ribattuta:
                previous_note_sent = Midi.SendEvent(previous_note, octave, out_device, "off")
                print(previous_note_sent)
                last_octave = octave

                current_note_sent = Midi.SendEvent(current_note, octave, out_device, "on")
                print(current_note_sent)
                print("Ribattuta.")
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