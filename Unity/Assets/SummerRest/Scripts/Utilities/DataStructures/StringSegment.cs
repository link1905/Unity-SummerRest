using System;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    // Utility struct for string non-alloc splitting
    // https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm
    public ref struct StringSegment
    {
        private ReadOnlySpan<char> _str;
        private readonly char _separator;
        public StringSegment(ReadOnlySpan<char> str, char separator)
        {
            _str = str;
            Current = default;
            _separator = separator;
        }
        // Needed to be compatible with the foreach operator
        public StringSegment GetEnumerator() => this;
        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0) // Reach the end of the string
                return false;
            ReadOnlySpan<char> escapes = stackalloc char[] {' ', '\n', '\r', '\t', _separator};
            var index = span.IndexOfAny(escapes);
            if (index == -1) // The string is composed of only one line
            {
                _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                return true;
            }

            if (index < span.Length - 1 && span[index] == '\r')
            {
                // Try to consume the '\n' associated to the '\r'
                var next = span[index + 1];
                if (next == '\n')
                {
                    Current = new LineSplitEntry(span[..index], span.Slice(index, 2));
                    _str = span[(index + 2)..];
                    return true;
                }
            }

            Current = new LineSplitEntry(span[..index], span.Slice(index, 1));
            _str = span[(index + 1)..];
            return true;
        }

        public LineSplitEntry Current { get; private set; }
    }

    public readonly ref struct LineSplitEntry
    {
        public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
        {
            Line = line;
            Separator = separator;
        }

        public ReadOnlySpan<char> Line { get; }
        public ReadOnlySpan<char> Separator { get; }
        public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
        {
            line = Line;
            separator = Separator;
        }
        public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
    }
}