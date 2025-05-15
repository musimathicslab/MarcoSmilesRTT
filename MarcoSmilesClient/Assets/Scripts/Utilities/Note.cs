namespace Utilities
{
    public class Note
    {
        public enum NoteNameEnum
        {
            DO,
            DO_SHARP,
            RE,
            RE_SHARP,
            MI,
            FA,
            FA_SHARP,
            SOL,
            SOL_SHARP,
            LA,
            LA_SHARP,
            SI,
            PAUSE
        }

        public enum OctaveEnum
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            PAUSE
        }

        public NoteNameEnum NoteName;
        public OctaveEnum Octave;

        public Note(NoteNameEnum noteName, OctaveEnum octave)
        {
            NoteName = noteName;
            Octave = octave;
        }
    }
}