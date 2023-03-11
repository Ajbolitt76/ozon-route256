using Grpc.Core;
using Moq;

namespace Ozon.Route256.Five.OrderService.UnitTests.Grpc;

public class GrpcUtils
{
    public static AsyncServerStreamingCall<TResponse> CreateAsyncServerStreamingCall<TResponse>(
        IEnumerable<TResponse> response,
        Metadata? metadata = null,
        Action? dispose = null)
        => new(
            new IEnumerableStreamReader<TResponse>(response),
            Task.FromResult(metadata ?? new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            dispose ?? (() => { }));

    public static AsyncServerStreamingCall<TResponse> CreateAsyncServerStreamingCall<TResponse>(
        IAsyncEnumerable<TResponse> response,
        Metadata? metadata = null,
        Action? dispose = null,
        Status? status = null,
        CancellationToken cancellationToken = default)
        => new(
            new IAsyncEnumerableStreamReader<TResponse>(response, cancellationToken),
            Task.FromResult(metadata ?? new Metadata()),
            () => status ?? Status.DefaultSuccess,
            () => new Metadata(),
            dispose ?? (() => { }));

    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(
        TResponse response,
        Exception? exception = null,
        Metadata? metadata = null,
        Action? dispose = null,
        Status? status = null,
        CancellationToken cancellationToken = default)
        => new(
            exception != null
                ? Task.FromException<TResponse>(exception)
                : Task.FromResult(response),
            Task.FromResult(metadata ?? new Metadata()),
            () => status ?? Status.DefaultSuccess,
            () => new Metadata(),
            dispose ?? (() => { }));

    private class IEnumerableStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public T Current => _enumerator.Current;

        public IEnumerableStreamReader(IEnumerable<T> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_enumerator.MoveNext());
        }
    }

    private class IAsyncEnumerableStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IAsyncEnumerator<T> _enumerator;

        public T Current => _enumerator.Current;

        private readonly CancellationTokenSource _tcs = new();

        public IAsyncEnumerableStreamReader(IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _tcs.Cancel());
            _enumerator = enumerable.GetAsyncEnumerator(_tcs.Token);
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            await using var _ = cancellationToken.Register(() => _tcs.Cancel());
            return await _enumerator.MoveNextAsync();
        }
    }
}