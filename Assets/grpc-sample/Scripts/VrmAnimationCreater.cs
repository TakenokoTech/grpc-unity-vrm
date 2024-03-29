﻿using grpc;
using UnityEngine;
using static VrmAnimationJson;

public class VrmAnimationCreater : MonoBehaviour, IGrpcSendable {

    enum VrmAnimationCreaterType { SEND, RECEIVE }
    [SerializeField] private VrmAnimationCreaterType type;
    [SerializeField] private string characterName;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform rootBone;

    private Vector3 defRootPos;
    private HumanPose humanPose = new HumanPose();
    private VrmAnimationJson anime = new VrmAnimationJson();

    // Start is called before the first frame update
    void Start() {
        defRootPos = animator.GetBoneTransform(0).localPosition;
        LoadBone();
    }

    // Update is called once per frame
    void Update() {
        HumanPoseHandler humanPoseHandler = new HumanPoseHandler(animator.avatar, rootBone);
        humanPoseHandler.GetHumanPose(ref humanPose);
        switch (type) {
            case VrmAnimationCreaterType.SEND: SendBone(); break;
            case VrmAnimationCreaterType.RECEIVE: ReceiveBone(); break;
        }

    }

    private void LoadBone() {
        for (int i = 0; i <= 54; i++) {
            Transform bone = animator.GetBoneTransform((HumanBodyBones)i);
            this.anime.vrmAnimation.Add(new VrmAnimation());
            this.anime.vrmAnimation[i].keys.Add(new Key());
        }
    }

    private void SendBone() {
        for (int i = 0; i <= 54; i++) {
            Transform bone = animator.GetBoneTransform((HumanBodyBones)i);
            if (bone == null) continue;

            float[] pos = new float[3] { bone.localPosition.x, bone.localPosition.y, bone.localPosition.z };
            float[] rot = new float[4] { bone.localRotation.x, bone.localRotation.y, bone.localRotation.z, bone.localRotation.w };
            float[] scl = new float[3] { bone.localScale.x, bone.localScale.y, bone.localScale.z };
            this.anime.vrmAnimation[i].name = i.ToString();
            this.anime.vrmAnimation[i].bone = bone.name;
            this.anime.vrmAnimation[i].keys[0] = new Key(pos, rot, scl, System.DateTime.Now.ToBinary());
        }

        Transform root = animator.GetBoneTransform(0);
        this.anime.rootPosition = new float[3] { defRootPos.x - root.localPosition.x, defRootPos.y - root.localPosition.y, defRootPos.z - root.localPosition.z };
    }

    private void ReceiveBone() {
        for (int i = 0; i <= 54; i++) {
            Transform bone = animator.GetBoneTransform((HumanBodyBones)i);
            Key key = this.anime.vrmAnimation[i].keys[0];
            if (bone == null || key == null) continue;

            //if (i == 0 && key.pos != null && key.pos.Length == 3) bone.localPosition = new Vector3(key.pos[0], key.pos[1], key.pos[2]);
            if (key.rot != null && key.rot.Length == 4) bone.localRotation = new Quaternion(key.rot[0], key.rot[1], key.rot[2], key.rot[3]);
            // if (key.scl != null && key.scl.Length == 3) bone.localScale = new Vector3(key.scl[0], key.scl[1], key.scl[2]);
        }

        Transform root = animator.GetBoneTransform(0);
        root.localPosition = new Vector3(defRootPos.x - anime.rootPosition[0], defRootPos.y - anime.rootPosition[1], defRootPos.z - anime.rootPosition[2]);
    }

    public VrmAnimationJson GetAnime() {
        return anime;
    }

    public void SetAnime(VrmAnimationJson ani) {
        this.anime = ani;
    }
}
