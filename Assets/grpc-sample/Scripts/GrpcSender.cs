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
        public string text = "wait reply...";

        private readonly SampleServiceClient client;
        private AsyncDuplexStreamingCall<SampleRequest, SampleResponse> stream;

        GrpcSender() {
            Channel channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
            client = new SampleServiceClient(channel);
        }

        // Start is called before the first frame update
        void Start() {
            stream = client.Stream();
            ReciveStream();
        }

        void Update() {
            //Transform();
            SendStream();
        }

        void OnDestroy() {
            Debug.Log("OnDestroy");
            stream.Dispose();
        }

        private void Transform() {
            Task.Run(() => {
                var reply = client.Transform(new SampleRequest { Message = "aaaa" });
                Debug.Log("reply: " + reply.Message);
                text = "reply: " + reply.Message;
            });
        }

        private void ReciveStream() {
            Task.Run(async () => {
                while (await stream.ResponseStream.MoveNext(CancellationToken.None)) {
                    Debug.Log("recive: " + stream.ResponseStream.Current.Message);
                    text = "recive: " + stream.ResponseStream.Current.Message;
                }
            });
        }

        private void SendStream() {
            Task.Run(async () => {
                var req = new SampleRequest { Message = "TestContent" };
                await stream.RequestStream.WriteAsync(req);
            });
        }
    }
}
