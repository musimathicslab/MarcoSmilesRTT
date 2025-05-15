using System;
using Utilities;

namespace SelectNotes
{
    public static class NoteMapper
    {
        public static Note.NoteNameEnum StringToNoteName(string noteName)
        {
            switch (noteName)
            {
                case "DO":
                    return Note.NoteNameEnum.DO;
                case "DO#":
                    return Note.NoteNameEnum.DO_SHARP;
                case "RE":
                    return Note.NoteNameEnum.RE;
                case "RE#":
                    return Note.NoteNameEnum.RE_SHARP;
                case "MI":
                    return Note.NoteNameEnum.MI;
                case "FA":
                    return Note.NoteNameEnum.FA;
                case "FA#":
                    return Note.NoteNameEnum.FA_SHARP;
                case "SOL":
                    return Note.NoteNameEnum.SOL;
                case "SOL#":
                    return Note.NoteNameEnum.SOL_SHARP;
                case "LA":
                    return Note.NoteNameEnum.LA;
                case "LA#":
                    return Note.NoteNameEnum.LA_SHARP;
                case "SI":
                    return Note.NoteNameEnum.SI;
                case "PAUSE":
                    return Note.NoteNameEnum.PAUSE;
                default:
                    throw new Exception("Invalid pitch!");
            }
        }

        public static Note.OctaveEnum StringToOctave(string octave)
        {
            switch (octave)
            {
                case "0":
                    return Note.OctaveEnum.ZERO;
                case "1":
                    return Note.OctaveEnum.ONE;
                case "2":
                    return Note.OctaveEnum.TWO;
                case "3":
                    return Note.OctaveEnum.THREE;
                case "4":
                    return Note.OctaveEnum.FOUR;
                case "5":
                    return Note.OctaveEnum.FIVE;
                case "6":
                    return Note.OctaveEnum.SIX;
                case "7":
                    return Note.OctaveEnum.SEVEN;
                case "PAUSE":
                    return Note.OctaveEnum.PAUSE;
                default:
                    throw new Exception("Invalid octave!");
            }
        }

        public static Note KeyToNote(string key)
        {
            //e.g. DO#4 noteNameString = DO#, octaveString = 4
            var noteNameString = key.Substring(0, key.Length - 1);
            var octaveString = key.Substring(key.Length - 1);

            var noteName = StringToNoteName(noteNameString);
            var octave = StringToOctave(octaveString);

            return new Note(noteName, octave);
        }
    }
}