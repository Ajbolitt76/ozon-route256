using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Grpc.Core;

namespace Ozon.Route256.Five.OrderService.UnitTests.Grpc;

public class GrpcServerStreamPublisher<T>
{
    private Channel<T> _packetChannel;

    private CancellationTokenSource _cts = new();

    public GrpcServerStreamPublisher(Metadata? metadata = null)
    {
        _packetChannel = Channel.CreateUnbounded<T>();
        Call = GrpcUtils.CreateAsyncServerStreamingCall(_packetChannel.Reader.ReadAllAsync(_cts.Token), cancellationToken: _cts.Token);
    }

    public ChannelWriter<T> PacketWriter => _packetChannel.Writer;

    public int QueueLength => _packetChannel.Reader.Count;
    
    public AsyncServerStreamingCall<T> Call { get; }
}