using System.ComponentModel.DataAnnotations;
using Blazorise;
using Syncfusion.Blazor.PivotView;



namespace Jakar.Telemetry.Server.Data.Tables;


[Table(TABLE_NAME)]
public sealed record ApplicationRecord( [StringLength(512, MinimumLength = 10)] string ApplicationName, RecordID<UserRecord>? CreatedBy, RecordID<ApplicationRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<ApplicationRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<ApplicationRecord>
{
    public const string TABLE_NAME = "applications";

    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<ApplicationRecord>.Default.WithColumn<string>(nameof(ApplicationName), length: 512)
                                                                                                                  .With_CreatedBy()
                                                                                                                  .Build();

    public static string TableName => TABLE_NAME;


    public static MigrationRecord CreateTable( ulong migrationID ) => MigrationRecord.Create<ApplicationRecord>(migrationID,
                                                                                                                $"create {TABLE_NAME} table",
                                                                                                                $"""
                                                                                                                 CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                                                                                                 (
                                                                                                                 {nameof(ApplicationName).SqlColumnName()}  bigint       PRIMARY KEY,  
                                                                                                                 {nameof(CreatedBy).SqlColumnName()} uuid NULL, FOREIGN KEY( {nameof(CreatedBy).SqlColumnName()}) REFERENCES {UserRecord.TABLE_NAME.SqlColumnName()}(id) ON DELETE SET NULL,
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
    public override ValueTask         Export( NpgsqlBinaryExporter exporter,        CancellationToken        token )                                        => default;
    public static   ApplicationRecord Create( string               applicationName, in RecordID<UserRecord>? createdBy, in RecordID<ApplicationRecord> id ) => new(applicationName, createdBy, id, DateTimeOffset.UtcNow);
    public static ApplicationRecord Create( NpgsqlDataReader reader )
    {
        string                      applicationName = reader.GetFieldValue<string>(nameof(ApplicationName));
        DateTimeOffset              dateCreated     = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?             lastModified    = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>?       ownerUserID     = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<ApplicationRecord> id              = RecordID<ApplicationRecord>.ID(reader);
        ApplicationRecord           record          = new(applicationName, ownerUserID, id, dateCreated, lastModified);
        record.Validate();
        return record;
    }
}
