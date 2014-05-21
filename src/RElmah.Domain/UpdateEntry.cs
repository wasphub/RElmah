using System.Collections.Generic;

namespace RElmah.Domain
{
    public enum UpdateEntryType
    {
        Create,
        Update,
        Remove
    }

    public class UpdateEntry<T>
    {
        public UpdateEntry(IEnumerable<T> entries, UpdateEntryType type)
        {
            Entries = entries;
            Type = type;
        }

        public IEnumerable<T> Entries { get; private set; }
        public UpdateEntryType Type { get; private set; }
    }
}