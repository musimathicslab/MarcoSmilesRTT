using System;
using Sanford.Multimedia.Midi;

namespace MidiLib{
    public class MidiClass{
        private OutputDevice? outD;
        private ChannelMessageBuilder? builder;

        public MidiClass(){
            builder = new ChannelMessageBuilder();
        }


        //SendEvent allow to send a midi event (note_on/note_off) to the MarcoSmiles port 
        public string SendEvent(int note, int octave, OutputDevice outDev, string command){

            //To print the note
            String note_str = "";
            String octave_str = "";
            String[] note_names = new String[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            //Data1 => MIDI note index
            builder.Data1 = note + (octave * 12);

            //Data2 => Velocity
            builder.Data2 = 105;

            builder.MidiChannel = 0;

            //choosing the command
            if (command.Equals("on")){
                builder.Command = ChannelCommand.NoteOn;
            }else if (command.Equals("off")){
                builder.Command = ChannelCommand.NoteOff;
            }

            //Building the message
            builder.Build();
            //Sending the message 
            outDev.Send(builder.Result);

            //Printing the note on terminal
            note_str = note_names[note % 12];
            if (note > 11){
                octave_str = "" + (octave + 1);
            }else{
                octave_str = "" + octave;
            }
            string note_sent = command + note_str + octave_str;
            return note_sent;
        }

        //OutPutDevice allow to find the MIDI port of MarcoSmiles
        public OutputDevice FindMidi(){
            int DevId = 0;

            //find MarcoSmiles port MIDI
            int numDevice = OutputDevice.DeviceCount;

            for (int i = 0; i < numDevice; i++){

                MidiOutCaps dev = OutputDevice.GetDeviceCapabilities(i);

                if (dev.name == "MarcoSmiles"){
                    DevId = i;
                }
            }
            //Select the output Device 
            outD = new OutputDevice(DevId);
            return outD;
        }
    }
}