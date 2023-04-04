using System.Data;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class BucketDbConnection : DbConnection
{
    private readonly DbConnection _dbConnection;

    public BucketDbConnection(
        DbConnection dbConnection,
        int bucketId)
    {
        _dbConnection = dbConnection;
        BucketId = bucketId;
    }

    public int BucketId { get; }

    protected override DbTransaction BeginDbTransaction(
        IsolationLevel isolationLevel) => _dbConnection.BeginTransaction(isolationLevel);

    public override void ChangeDatabase(
        string databaseName) => _dbConnection.ChangeDatabase(databaseName);

    public override void Close() => _dbConnection.Close();

    public override void Open() => _dbConnection.Open();

    public override string ConnectionString
    {
        get => _dbConnection.ConnectionString;
        set => _dbConnection.ConnectionString = value;
    }

    public override string Database => _dbConnection.Database;
    public override ConnectionState State => _dbConnection.State;
    public override string DataSource => _dbConnection.DataSource;
    public override string ServerVersion => _dbConnection.ServerVersion;

    protected override ValueTask<DbTransaction> BeginDbTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
        => _dbConnection.BeginTransactionAsync(isolationLevel, cancellationToken);

    protected override DbCommand CreateDbCommand()
    {
        var command = _dbConnection.CreateCommand();
        return new BucketDbCommand(command, BucketId);
    }
}