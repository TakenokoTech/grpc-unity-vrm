using Grpc.Core;
using Sample;
using System;
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
        private int count = 0;

        GrpcSender() {
            this.channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
            this.client = new SampleServiceClient(channel);
        }

        // Start is called before the first frame update
        void Start() {
            text = "wait reply...";
            // Transform();
            // ReciveStream();
        }

        void Update() {
            SendStream();
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

        private void SendStream() {
            Task nowait = Task.Run(async () => {
                using (var call = client.Stream()) {
                    try {
                        await call.RequestStream.WriteAsync(new SampleRequest { Message = "message" + count++ });
                    }
                    catch (Exception ex) {
                        Debug.Log(ex);
                    }
                }
            });
        }

        private void ReciveStream() {
            Task nowait = Task.Run(async () => {
                using (var call = client.Stream()) {
                    try {
                        Debug.Log("recive");
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;
                        await call.ResponseStream.MoveNext(token);
                        Debug.Log("recive: " + call.ResponseStream.Current);
                        //CancellationToken token = new CancellationToken();
                        //while (await call.ResponseStream.MoveNext(token).ConfigureAwait(true)) {
                        //    Debug.Log("recive: " + call.ResponseStream.Current);
                        //}
                    }
                    catch (Exception ex) {
                        Debug.Log(ex);
                    }
                }
            });
        }
    }
}
