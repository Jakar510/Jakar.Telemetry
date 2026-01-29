// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

namespace Jakar.Telemetry;


[Serializable]
[StructLayout(LayoutKind.Auto)]
public readonly struct ActivityTraceID : IEquatable<ActivityTraceID>
{
    public const             int             SIZE       = sizeof(ulong) * 2;
    public const             int             LENGTH     = sizeof(ulong) * 4;
    internal static readonly string          _empty     = new('0', LENGTH);
    public static readonly   ActivityTraceID Empty      = new(_empty);
    internal readonly        string          _hexString = _empty;
    internal readonly        int             _hash      = _empty.GetHashCode();


    internal ActivityTraceID( string hexString ) => _hexString = hexString;
    private ActivityTraceID( ReadOnlySpan<byte> idData )
    {
        if ( idData.Length != 32 ) { throw new ArgumentOutOfRangeException(nameof(idData)); }

        Span<ulong> span = stackalloc ulong[2];

        if ( !Utf8Parser.TryParse(idData[..16], out span[0], out _, 'x') )
        {
            // Invalid Id, use random https://github.com/dotnet/runtime/issues/29859
            _hexString = CreateRandom()
               ._hexString;

            return;
        }

        if ( !Utf8Parser.TryParse(idData.Slice(16, 16), out span[1], out _, 'x') )
        {
            // Invalid Id, use random https://github.com/dotnet/runtime/issues/29859
            _hexString = CreateRandom()
               ._hexString;

            return;
        }

        if ( BitConverter.IsLittleEndian )
        {
            span[0] = BinaryPrimitives.ReverseEndianness(span[0]);
            span[1] = BinaryPrimitives.ReverseEndianness(span[1]);
        }

        _hexString = Convert.ToHexStringLower(MemoryMarshal.AsBytes(span));
    }


    public static ActivityTraceID CreateRandom()                                    => new(CreateRandom(SIZE));
    public static ActivityTraceID CreateFromUtf8String( ReadOnlySpan<byte> idData ) => new(idData);
    public static ActivityTraceID CreateFromBytes( ReadOnlySpan<byte> idData ) => idData.Length != SIZE
                                                                                      ? throw new ArgumentOutOfRangeException(nameof(idData))
                                                                                      : new ActivityTraceID(Convert.ToHexStringLower(idData));
    public static ActivityTraceID CreateFromString( ReadOnlySpan<char> idData ) => idData.Length != LENGTH || !IsLowerCaseHexAndNotAllZeros(idData)
                                                                                       ? throw new ArgumentOutOfRangeException(nameof(idData))
                                                                                       : new ActivityTraceID(idData.ToString());
    public override string ToString() => _hexString;

    public bool Equals( ActivityTraceID traceId ) => _hexString == traceId._hexString;
    public override bool Equals( [NotNullWhen(true)] object? obj )
    {
        if ( obj is ActivityTraceID traceId ) { return _hexString == traceId._hexString; }

        return false;
    }
    public override int GetHashCode() => _hash;


    public bool CopyTo( Span<byte> destination )                       => Convert.FromHexString(_hexString, destination, out _, out _) is OperationStatus.Done;
    public bool CopyTo( Span<byte> destination, out int bytesWritten ) => Convert.FromHexString(_hexString, destination, out _, out bytesWritten) is OperationStatus.Done;


    public static bool operator ==( ActivityTraceID traceId1, ActivityTraceID traceId2 ) => traceId1._hexString == traceId2._hexString;
    public static bool operator !=( ActivityTraceID traceId1, ActivityTraceID traceId2 ) => traceId1._hexString != traceId2._hexString;
}
