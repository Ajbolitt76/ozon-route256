using System.Data;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class BucketDbCommand : DbCommand
{
    private readonly DbCommand _dbCommand;

    public BucketDbCommand(
        DbCommand dbCommand,
        int bucketId)
    {
        _dbCommand = dbCommand;
        BucketId = bucketId;
    }

    public int BucketId { get; }

    public override void Cancel() => _dbCommand.Cancel();

    public override int ExecuteNonQuery() => _dbCommand.ExecuteNonQuery();

    public override object? ExecuteScalar() => _dbCommand.ExecuteScalar();

    public override void Prepare() => _dbCommand.Prepare();

    public override string CommandText
    {
        get => _dbCommand.CommandText;
        set
        {
            var command = value?.Replace("__bucket__", BucketNamingConvention.GetBucketSchema(BucketId));
            _dbCommand.CommandText = command;
        }
    }

    public override int CommandTimeout
    {
        get => _dbCommand.CommandTimeout;
        set => _dbCommand.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
        get => _dbCommand.CommandType;
        set => _dbCommand.CommandType = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => _dbCommand.UpdatedRowSource;
        set => _dbCommand.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
        get => _dbCommand.Connection;
        set => _dbCommand.Connection = value;
    }

    protected override DbParameterCollection DbParameterCollection => _dbCommand.Parameters;

    protected override DbTransaction? DbTransaction
    {
        get => _dbCommand.Transaction;
        set => _dbCommand.Transaction = value;
    }

    public override bool DesignTimeVisible
    {
        get => _dbCommand.DesignTimeVisible;
        set => _dbCommand.DesignTimeVisible = value;
    }

    protected override DbParameter CreateDbParameter() => _dbCommand.CreateParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => _dbCommand.ExecuteReader(behavior);
}