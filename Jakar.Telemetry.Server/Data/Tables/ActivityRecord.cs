// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  22:06

using System.Diagnostics.CodeAnalysis;
using Serilog.Events;
using static Jakar.Extensions.Constants.Values;



namespace Jakar.Telemetry.Server.Data.Tables;


[Table(TABLE_NAME)]
public sealed record ActivityRecord( RecordID<UserRecord>? CreatedBy, RecordID<ActivityRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<ActivityRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<ActivityRecord>, ITelemetryActivity
{
    public const string TABLE_NAME = "activities";

    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<ActivityRecord>.Default.WithColumn<string>(nameof(OperationName))
                                                                                                               .WithColumn<string>(nameof(DisplayName))
                                                                                                               .WithColumn<string>(nameof(OperationName))
                                                                                                               .WithColumn<string>(nameof(StatusDescription))
                                                                                                               .WithColumn<string>(nameof(SpanID))
                                                                                                               .WithColumn<StatusCode>(nameof(Status))
                                                                                                               .WithColumn<ActivityKind>(nameof(Kind))
                                                                                                               .WithColumn<DateTimeOffset?>(nameof(EndTimeUtc))
                                                                                                               .WithColumn<DateTimeOffset?>(nameof(StartTimeUtc))
                                                                                                               .WithColumn<TimeSpan?>(nameof(Duration))
                                                                                                               .WithColumn<bool>(nameof(IsStopped))
                                                                                                               .WithColumn<string>(nameof(TraceID))
                                                                                                               .WithForeignKey<ActivityRecord>(nameof(Parent))
                                                                                                               .WithForeignKey<ApplicationRecord>(nameof(ApplicationID))
                                                                                                               .Build();

    public static   string                       TableName         => TABLE_NAME;
    public          ActivityKind                 Kind              { get; set; }
    public          string                       OperationName     { get; set; } = EMPTY;
    public          TelemetryActivityContext     Context           => new(OperationName, TraceID, ActivitySpanID.Parse(SpanID));
    public          string                       DisplayName       { get; set; } = EMPTY;
    public          TimeSpan?                    Duration          { get; set; }
    public          DateTimeOffset?              EndTimeUtc        { get; set; }
    public          bool                         IsStopped         { get; set; }
    public required ActivityTraceID              TraceID           { get; init; }
    public          string                       SpanID            { get; set; } = EMPTY;
    public          DateTimeOffset?              StartTimeUtc      { get; set; }
    public          StatusCode?                  Status            { get; set; }
    public          string?                      StatusDescription { get; set; }
    public          RecordID<ActivityRecord>?    Parent            { get; set; }
    public required RecordID<ApplicationRecord>? ApplicationID     { get; init; }


    [SetsRequiredMembers] public ActivityRecord( TelemetryActivity log, RecordID<ApplicationRecord>? applicationID, RecordID<ActivityRecord>? parent, RecordID<UserRecord>? createdBy = null ) : this(createdBy, RecordID<ActivityRecord>.New(), DateTimeOffset.UtcNow)
    {
        ApplicationID     = applicationID;
        Parent            = parent;
        TraceID           = log.Context.TraceID;
        SpanID            = log.Collate(true);
        OperationName     = log.OperationName;
        StartTimeUtc      = log.StartTimeUtc;
        EndTimeUtc        = log.EndTimeUtc;
        Status            = log.Status;
        StatusDescription = log.StatusDescription;
        Duration          = log.Duration;
        IsStopped         = log.IsStopped;
        Kind              = log.Kind;
    }
    public static MigrationRecord CreateTable( ulong migrationID ) =>
        MigrationRecord.Create<ActivityRecord>(migrationID,
                                               $"create {TABLE_NAME} table",
                                               $"""
                                                CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                                (
                                                {nameof(TraceID).SqlColumnName()} char( 32 ) NOT NULL,
                                                {nameof(SpanID).SqlColumnName()} varchar( 4096 ) NOT NULL,
                                                {nameof(StartTimeUtc).SqlColumnName()} timestamptz NULL,
                                                {nameof(EndTimeUtc).SqlColumnName()} timestamptz NULL,
                                                {nameof(Duration).SqlColumnName()} timestamptz NULL,
                                                {nameof(ApplicationID).SqlColumnName()} uuid NULL FOREIGN KEY( {nameof(CreatedBy).SqlColumnName()}) REFERENCES {ApplicationRecord.TABLE_NAME.SqlColumnName()}(id) ON DELETE SET NULL,
                                                {nameof(CreatedBy).SqlColumnName()} uuid NULL FOREIGN KEY( {nameof(CreatedBy).SqlColumnName()}) REFERENCES {UserRecord.TABLE_NAME.SqlColumnName()}(id) ON DELETE SET NULL,
                                                {nameof(ID).SqlColumnName()} uuid NOT NULL PRIMARY KEY,
                                                {nameof(DateCreated).SqlColumnName()} timestamptz NOT NULL DEFAULT SYSUTCDATETIME(),
                                                {nameof(LastModified).SqlColumnName()} timestamptz NULL,
                                                {nameof(AdditionalData).SqlColumnName()} json NULL
                                                );

                                                CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                BEFORE INSERT OR UPDATE ON {TABLE_NAME}
                                                FOR    EACH ROW  EXECUTE FUNCTION {
                                                    nameof(MigrationRecord.SetLastModified)
                                                       .SqlColumnName()
                                                }
                                                ();
                                                """);
    public static ActivityRecord Create( NpgsqlDataReader reader )
    {
        ActivityRecord record = new() { };
        record.Validate();
        return record;
    }
    public override ValueTask Export( NpgsqlBinaryExporter exporter, CancellationToken token ) => default;
    public          void      Dispose() { }


    public void SetStatus( StatusCode? code, string? description = null )
    {
        Status            = code;
        StatusDescription = description;
    }
}
