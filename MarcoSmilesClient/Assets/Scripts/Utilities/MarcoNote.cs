using System.Collections.Generic;

namespace Utilities
{
    public class MarcoNote
    {
        public enum NoteEnum
        {
            Do,
            DoSharp,
            Re,
            ReSharp,
            Mi,
            Fa,
            FaSharp,
            Sol,
            SolSharp,
            La,
            LASharp,
            Si,
            Pause
        }

        private static readonly Dictionary<NoteEnum, string> NoteToString = new()
        {
            { NoteEnum.Do, "DO" },
            { NoteEnum.DoSharp, "DO#" },
            { NoteEnum.Re, "RE" },
            { NoteEnum.ReSharp, "RE#" },
            { NoteEnum.Mi, "MI" },
            { NoteEnum.Fa, "FA" },
            { NoteEnum.FaSharp, "FA#" },
            { NoteEnum.Sol, "SOL" },
            { NoteEnum.SolSharp, "SOL#" },
            { NoteEnum.La, "LA" },
            { NoteEnum.LASharp, "LA#" },
            { NoteEnum.Si, "SI" },
            { NoteEnum.Pause, "Pausa" }
        };

        private static readonly Dictionary<NoteEnum, string> NoteToStringInternational = new()
        {
            { NoteEnum.Do, "C" },
            { NoteEnum.DoSharp, "C#" },
            { NoteEnum.Re, "D" },
            { NoteEnum.ReSharp, "D#" },
            { NoteEnum.Mi, "E" },
            { NoteEnum.Fa, "F" },
            { NoteEnum.FaSharp, "F#" },
            { NoteEnum.Sol, "G" },
            { NoteEnum.SolSharp, "G#" },
            { NoteEnum.La, "A" },
            { NoteEnum.LASharp, "A#" },
            { NoteEnum.Si, "B" },
            { NoteEnum.Pause, "Pause" }
        };

        public MarcoNote(NoteEnum value)
        {
            Value = value;
            Name = NoteToString[Value];
        }

        public NoteEnum Value { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public string ToInternational()
        {
            return NoteToStringInternational[Value];
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var note = (MarcoNote)obj;
            return Value == note.Value;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}