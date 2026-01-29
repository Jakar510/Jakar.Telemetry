// Jakar.Telemetry :: Jakar.Telemetry
// 01/27/2026  14:07

using Jakar.Extensions;



namespace Jakar.Telemetry;


public static class Activities
{
    internal static string CreateRandom( int size )
    {
        Span<byte> span = stackalloc byte[size];
        RandomNumberGenerator.Fill(span);
        return Convert.ToHexStringLower(span);
    }
    internal static bool IsLowerCaseHexAndNotAllZeros( this ReadOnlySpan<char> idData )
    {
        bool isNonZero = false;

        foreach ( char c in idData )
        {
            if ( char.IsUpper(c) ) { return false; }

            if ( c != '0' ) { isNonZero = true; }
        }

        return isNonZero;
    }


    public static string Collate( this IEnumerable<ActivitySpanID>  spans ) => $"|{string.Join('|', spans.Select(static x => x._hexString))}";
    public static string Collate( this IEnumerable<ActivityTraceID> spans ) => $"|{string.Join('|', spans.Select(static x => x._hexString))}";


    public static string Collate( this TelemetryActivity? current, bool isSpans )
    {
        using Buffer<ActivitySpanID>  spans  = new(DEFAULT_CAPACITY);
        using Buffer<ActivityTraceID> traces = new(DEFAULT_CAPACITY);

        while ( current is not null )
        {
            spans.Add(current.context.SpanID);
            traces.Add(current.context.TraceID);
            current = current.parent;
        }

        spans.Reverse();
        traces.Reverse();

        return isSpans
                   ? spans.Values.Collate()
                   : traces.Values.Collate();
    }


    public static string Collate<TEnumerator>( this ValueEnumerable<TEnumerator, ActivitySpanID> spans )
        where TEnumerator : struct, IValueEnumerator<ActivitySpanID>, allows ref struct
    {
        if ( !spans.TryGetNonEnumeratedCount(out int count) ) { throw new InvalidOperationException("The enumerator does not support non-enumerated count retrieval."); }

        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent(count);
        Span<string?>               ids   = owner.Memory.Span[..count];
        int                         i     = 0;

        foreach ( ActivitySpanID context in spans ) { ids[i++] = context._hexString; }

        return $"|{string.Join('|', ids)}";
    }
    public static string Collate( this ReadOnlySpan<ActivitySpanID> spans )
    {
        int                        count = spans.Length;
        using IMemoryOwner<string> owner = MemoryPool<string>.Shared.Rent(count);
        Span<string>               span  = owner.Memory.Span[..count];
        for ( int i = 0; i < count; i++ ) { span[i] = spans[i]._hexString; }

        using ValueStringBuilder sb = new();
        sb.AppendJoin('|', span);
        return sb.ToString();
    }


    public static string Collate<TEnumerator>( this ValueEnumerable<TEnumerator, ActivityTraceID> spans )
        where TEnumerator : struct, IValueEnumerator<ActivityTraceID>, allows ref struct
    {
        if ( !spans.TryGetNonEnumeratedCount(out int count) ) { throw new InvalidOperationException("The enumerator does not support non-enumerated count retrieval."); }

        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent(count);
        Span<string?>               ids   = owner.Memory.Span[..count];
        int                         i     = 0;

        foreach ( ActivityTraceID context in spans ) { ids[i++] = context._hexString; }

        return $"|{string.Join('|', ids)}";
    }
    public static string Collate( this ReadOnlySpan<ActivityTraceID> spans )
    {
        int                        count = spans.Length;
        using IMemoryOwner<string> owner = MemoryPool<string>.Shared.Rent(count);
        Span<string>               span  = owner.Memory.Span[..count];
        for ( int i = 0; i < count; i++ ) { span[i] = spans[i]._hexString; }

        using ValueStringBuilder sb = new();
        sb.AppendJoin('|', span);
        return sb.ToString();
    }
}
