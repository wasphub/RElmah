using System.Collections.Generic;

namespace RElmah.Common
{
    public class Delta<T>
    {
        public Delta(IEnumerable<T> targets, DeltaType type)
        {
            Targets = targets;
            Type = type;
        }

        public IEnumerable<T> Targets { get; private set; }
        public DeltaType Type { get; private set; }
    }
}