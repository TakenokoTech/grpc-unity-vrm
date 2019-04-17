using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

[Serializable]
public class VrmAnimationJson {

    public List<VrmAnimation> vrmAnimation = new List<VrmAnimation>();

    [Serializable]
    public class VrmAnimation {
        public string name = "";
        public string bone = "";
        public List<Key> keys = new List<Key>();
    }

    [Serializable]
    public class Key {
        public float[] pos;
        public float[] rot;
        public float[] scl;
        public long time;

        public Key(float[] pos, float[] rot, float[] scl, long time) {
            this.pos = pos;
            this.rot = rot;
            this.scl = scl;
            this.time = time;
        }

        public Key() {
        }
    }

    /// GZIP圧縮
    public byte[] ComporessGZIP() {
        using (var memoryStream = new MemoryStream()) {
            using (var inflateStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
                using (var writer = new StreamWriter(inflateStream)) {
                    writer.Write(JsonUtility.ToJson(this));
                }
            }
            return memoryStream.ToArray();
        }
    }

    /// GZIP解凍
    public VrmAnimationJson DecomporessGZIP(byte[] bytes) {
        using (var memoryStream = new MemoryStream(bytes)) {
            using (var deflateStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
                using (var reader = new StreamReader(deflateStream)) {
                    string json = reader.ReadToEnd();
                    return JsonUtility.FromJson<VrmAnimationJson>(json);
                }
            }
        }
    }

    // JSON化
    public string Stringify() {
        return JsonUtility.ToJson(this);
    }

    // JSON化
    public VrmAnimationJson ToJson(string json) {
        return JsonUtility.FromJson<VrmAnimationJson>(json);
    }
}
