using Grpc.Core;
using Sample;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Sample.SampleService;

namespace grpc {
    public class GrpcSender : MonoBehaviour {


        public string ip = "127.0.0.1";
        public string port = "9999";
        public string text;

        private readonly Channel channel;
        private readonly SampleServiceClient client;

        GrpcSender() {
            this.channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
            this.client = new SampleServiceClient(channel);
        }

        // Start is called before the first frame update
        async void Start() {
            text = "wait reply...";
            // await ReciveStreamAsync();
        }

        void Update() {
            Transform();
        }

        private void OnDestroy() {
            Debug.Log("OnDestroy");
            this.channel.ShutdownAsync().Wait();
        }

        private void Transform() {
            var reply = client.Transform(new SampleRequest { Message = "aaaa" });
            Debug.Log("reply: " + reply.Message);
            text = "reply: " + reply.Message;
        }

        private async Task ReciveStreamAsync() {
            using (var call = client.Stream()) {
                Task nowait = Task.Run(async () => {
                    while (await call.ResponseStream.MoveNext(CancellationToken.None)) {
                        // Debug.Log("recive: " + call.ResponseStream.Current.Message);
                        text = "recive: " + call.ResponseStream.Current.Message;
                    }
                });

                for (int i = 1; i <= 100000; i++) {
                    if (i > 10000) i = 0;
                    var req = new SampleRequest { Message = "TestContent: " + i };
                    await call.RequestStream.WriteAsync(req);
                }
                await call.RequestStream.CompleteAsync();
            }
        }
    }
}
