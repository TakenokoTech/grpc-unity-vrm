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

        public VrmAnimationCreater vrmAnimationCreater;
        private IGrpcSendable GrpcSendable { get { return vrmAnimationCreater as IGrpcSendable; } }
        public VrmAnimationCreater[] attachAnime;

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
            Transform();
            //SendStream();
        }

        void OnDestroy() {
            Debug.Log("OnDestroy");
            stream.Dispose();
        }

        private void Transform() {
            Task.Run(() => {
                var reply = client.Transform(new SampleRequest { Message = GrpcSendable.GetAnime().Stringify() });
                // Debug.Log("reply: " + reply.Message);
                var a = GrpcSendable.GetAnime().ToJson(reply.Message);
                foreach (var ani in attachAnime) ani.SetAnime(a);
            });
        }

        private void ReciveStream() {
            Task.Run(async () => {
                while (await stream.ResponseStream.MoveNext(CancellationToken.None)) {
                    // Debug.Log("recive: " + stream.ResponseStream.Current.Message);
                    text = stream.ResponseStream.Current.Message;
                }
            });
        }

        private void SendStream() {
            Task.Run(async () => {
                var req = new SampleRequest { Message = TimeUttil.NowTime() };
                await stream.RequestStream.WriteAsync(req);
            });
        }
    }

    public interface IGrpcSendable {
        VrmAnimationJson GetAnime();
        void SetAnime(VrmAnimationJson anime);
    }
}
