namespace Jakar.Telemetry;


public interface ITelemetryActivityContext
{
    string OperationName { get; }
    string SpanID        { get; }
    string TraceID       { get; }
}



[Serializable][StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly record struct TelemetryActivityContext( string OperationName, in ActivityTraceID TraceID, in ActivitySpanID SpanID ) : ITelemetryActivityContext
{
    public static readonly TelemetryActivityContext Empty         = new(nameof(Empty), in ActivityTraceID.Empty, in ActivitySpanID.Empty);
    public readonly        string                   OperationName = OperationName;
    public readonly        ActivitySpanID  SpanID        = SpanID;
    public readonly        ActivityTraceID TraceID       = TraceID;
    string ITelemetryActivityContext.               SpanID        => SpanID.ToString();
    string ITelemetryActivityContext.               TraceID       => TraceID.ToString();
    string ITelemetryActivityContext.               OperationName => OperationName;


    [Pure][MethodImpl(MethodImplOptions.AggressiveInlining)] public        TelemetryActivityContext CreateChild( string operationName )                                                                   => new(operationName, in TraceID, ActivitySpanID.CreateRandom());
    [Pure][MethodImpl(MethodImplOptions.AggressiveInlining)] public static TelemetryActivityContext Create( string      operationName )                                                                   => Create(operationName, ActivityTraceID.CreateRandom(), ActivitySpanID.CreateRandom());
    [Pure][MethodImpl(MethodImplOptions.AggressiveInlining)] public static TelemetryActivityContext Create( string      operationName, ActivityTraceID traceID, ActivitySpanID spanID ) => new(operationName, traceID, spanID);
}
