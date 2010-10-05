using System;

namespace SharpMonitor.IO
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataBlockAttribute : Attribute
    {
        public readonly int StartIndex;
        public readonly bool Is32BitValue;

        public DataBlockAttribute(int index)
            : this(index, false)
        {
        }

        public DataBlockAttribute(int startIndex, bool is32BitValue)
        {
            StartIndex = startIndex;
            Is32BitValue = is32BitValue;
        }
    }
}
