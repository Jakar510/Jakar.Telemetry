// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

using CommunityToolkit.Diagnostics;



namespace Jakar.Telemetry;


[Serializable]
[StructLayout(LayoutKind.Auto)]
public readonly struct ActivitySpanID : IEquatable<ActivitySpanID>, ISpanParsable<ActivitySpanID>
{
    public const             int            SIZE       = sizeof(ulong);
    public const             int            LENGTH     = sizeof(ulong) * 2;
    internal static readonly string         _empty     = new('0', LENGTH);
    public static readonly   ActivitySpanID Empty      = new(_empty);
    internal readonly        string         _hexString = _empty;
    internal readonly        int            _hash      = _empty.GetHashCode();


    internal ActivitySpanID( string hexString )
    {
        Guard.IsNotNullOrWhiteSpace(hexString);
        _hexString = hexString;
        _hash      = hexString.GetHashCode();
    }
    private ActivitySpanID( params ReadOnlySpan<byte> idData )
    {
        if ( idData.Length != LENGTH ) { throw new ArgumentOutOfRangeException(nameof(idData)); }

        if ( !Utf8Parser.TryParse(idData, out ulong id, out _, 'x') )
        {
            _hexString = CreateRandom()
               ._hexString;

            return;
        }

        if ( BitConverter.IsLittleEndian ) { id = BinaryPrimitives.ReverseEndianness(id); }

        Span<byte> span = stackalloc byte[SIZE];
        id.TryFormat(span, out _);
        _hexString = Convert.ToHexStringLower(span);
    }


    public static ActivitySpanID CreateRandom()                                           => new(ActivityTraceID.CreateRandom(SIZE));
    public static ActivitySpanID CreateFromUtf8String( params ReadOnlySpan<byte> idData ) => new(idData);
    public static ActivitySpanID CreateFromBytes( params ReadOnlySpan<byte> idData ) => idData.Length != SIZE
                                                                                            ? throw new ArgumentOutOfRangeException(nameof(idData))
                                                                                            : new ActivitySpanID(Convert.ToHexStringLower(idData));


    public static ActivitySpanID[] Create( string idData )
    {
        ReadOnlySpan<char> data    = idData;
        int                index   = 0;
        ActivitySpanID[]   results = new ActivitySpanID[data.Count('|')];

        foreach ( Range range in data.Split('|') )
        {
            ReadOnlySpan<char> span = data[range];
            results[index++] = Parse(span);
        }

        return results;
    }


    public static ActivitySpanID Parse( string idData, IFormatProvider? provider = null )
    {
        ReadOnlySpan<char> data = idData;
        return Parse(data, provider);
    }
    public static bool TryParse( [NotNullWhen(true)] string? idData, IFormatProvider? provider, out ActivitySpanID result )
    {
        ReadOnlySpan<char> data = idData;
        return TryParse(data, provider, out result);
    }
    public static ActivitySpanID Parse( ReadOnlySpan<char> data, IFormatProvider? provider = null )
    {
        int index = data.LastIndexOf('|');
        if ( index > 0 ) { data = data[index..]; }

        return new ActivitySpanID(data.ToString());
    }
    public static bool TryParse( ReadOnlySpan<char> data, IFormatProvider? provider, out ActivitySpanID result )
    {
        result = default;
        return false;
    }


    public override string ToString() => _hexString;


    public bool Equals( ActivitySpanID spanId ) => _hexString == spanId._hexString;
    public override bool Equals( [NotNullWhen(true)] object? obj )
    {
        if ( obj is ActivitySpanID spanId ) { return _hexString == spanId._hexString; }

        return false;
    }
    public override int GetHashCode() => _hash;


    public bool CopyTo( Span<byte> destination ) => Convert.FromHexString(_hexString, destination, out _, out _) is OperationStatus.Done;


    public static bool operator ==( ActivitySpanID left, ActivitySpanID right ) => left._hexString == right._hexString;
    public static bool operator !=( ActivitySpanID left, ActivitySpanID right ) => left._hexString != right._hexString;
}
