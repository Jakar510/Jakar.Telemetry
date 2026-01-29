// Jakar.Telemetry :: Jakar.Telemetry
// 01/27/2026  00:10

using Jakar.Extensions;
using Jakar.Telemetry.Meters;



namespace Jakar.Telemetry;


public interface ITelemetryActivity : IDisposable
{
    TelemetryActivityContext Context           { get; }
    string                   DisplayName       { get; set; }
    TimeSpan?                Duration          { get; set; }
    DateTimeOffset?          EndTimeUtc        { get; }
    bool                     IsStopped         { get; }
    ActivityKind             Kind              { get; }
    string                   OperationName     { get; }
    string                   SpanID            { get; }
    DateTimeOffset?          StartTimeUtc      { get; }
    StatusCode?              Status            { get; }
    string?                  StatusDescription { get; }


    void SetStatus( StatusCode? code, string? description = null );
}



public interface ITelemetryActivity<TSelf> : ITelemetryActivity
    where TSelf : ITelemetryActivity<TSelf>
{
    TSelf?                          Child  { get; set; }
    ValueLinkedList<TelemetryEvent> Events { get; }
    Pairs                           Tags   { get; }
    TelemetryMeters                 Meters { get; }
    TelemetryActivityContext?       Parent { get; }


    new TSelf SetStatus( StatusCode? code, string? description = null );
    TSelf     SetKind( ActivityKind  kind );
    TSelf     Start();
    TSelf     Stop();
    TSelf     AddChild( string                            operationName );
    TSelf     SetParent( TSelf?                           parentContext );
    TSelf     AddEvent( in                 TelemetryEvent value );
    TSelf     AddEvent( [CallerMemberName] string         caller = EMPTY );
    void      AddTag( string                              key, string? value );
    void      AddTag( in     Pair                         pair );
    void      AddTag( params ReadOnlySpan<Pair>           pairs );
    TSelf     AddException( Exception                     exception );
    TSelf     AddException( Exception                     exception, in Pairs tags );
    TSelf     AddException( Exception                     exception, in Pairs tags, in DateTimeOffset timestamp );
    void      AddLink( in TelemetryActivityLink           link );
}
