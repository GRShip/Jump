using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PawnRagdoll : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawn Owner { get; private set; }
    public PlayerPawn Player { get; private set; }
    public bool Activity { get; private set; }
    
    private CharacterController cc;
    private Rigidbody[] rbs;
    private Collider[] cols;
    private Animator ani;
    public GameObject hip;
    
    private Transform[] pawnBones;
    private BoneTransform[] standUpFrontBones;
    private BoneTransform[] standUpBackBones;
    private BoneTransform[] ragdollBones;
    private bool standUpFront;
    [SerializeField]
    private float standUpTime = 0.5f;
    [SerializeField]
    private float ragdollTime = 3f;
    
    [SerializeField]
    private string standUpFrontName;
    [SerializeField]
    private string standUpFrontClipName;
    [SerializeField]
    private string standUpBackName;
    [SerializeField]
    private string standUpBackClipName;
    
    private void Awake() {
        Owner = GetComponentInParent<PlayerPawn>();
        ani = GetComponent<Animator>();
        cc = GetComponentInParent<CharacterController>();
        rbs = GetComponentsInChildren<Rigidbody>();
        cols = GetComponentsInChildren<Collider>();
        pawnBones = hip.GetComponentsInChildren<Transform>();
        
        standUpFrontBones = new BoneTransform[pawnBones.Length];
        standUpBackBones = new BoneTransform[pawnBones.Length];
        ragdollBones = new BoneTransform[pawnBones.Length];

        for (int i = 0; i < pawnBones.Length; i++) {
            standUpFrontBones[i] = new BoneTransform();
            standUpBackBones[i] = new BoneTransform();
            ragdollBones[i] = new BoneTransform();
        }
    }

    private void Start() {
        PopulateStartBoneTransforms(standUpFrontClipName, standUpFrontBones);
        PopulateStartBoneTransforms(standUpBackClipName, standUpBackBones);
        SetRagdollState(false);
    }
    
    public void StartRagdoll(Vector3 force) {
        rbs[0].AddForce(force, ForceMode.Impulse);
        StopAllCoroutines();
        StartCoroutine(RagdollAction(ragdollTime));
    }
    
    private IEnumerator RagdollAction(float time) {
        float timer = 0f;
        while (true) {
            if (timer > time) {
                if (rbs[0].linearVelocity.sqrMagnitude < 1f) {
                    break;
                }
            }
            else {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        
        standUpFront = hip.transform.forward.y > 0;
        AlignRoatationToHips();
        AlignPositionToHips();
        PopulateBoneTransforms(ragdollBones);
        
        StartCoroutine(LerpToStandUp(standUpTime));
    }

    private IEnumerator LerpToStandUp(float time) {
        float timer = 0f;
        float percent = 0f;

        while (percent < 1) {
            timer += Time.deltaTime;
            percent = timer / time;
            
            BoneTransform[] standUpBones = GetStandUpBones();
            for (int i = 0; i < pawnBones.Length; i++) {
                pawnBones[i].localPosition = Vector3.Lerp(ragdollBones[i].Position, standUpBones[i].Position, percent);
                pawnBones[i].localRotation = Quaternion.Lerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, percent);
            }
            yield return null;
        }
        
        (Owner as PlayerPawn)?.PlayerRagdollFinish();
        ani.Play(GetStandUpName(), 0, 0);
        yield return null;
        while (true) {
            if (ani.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpName()) == false) {
                break;
            }
            yield return null;
        }
        (Owner as PlayerPawn)?.PlayerStandUpFinish();
    }
    
    private void AlignRoatationToHips() {
        Vector3 originalPosition = hip.transform.position;
        Quaternion originalRotation = hip.transform.rotation;
        Vector3 desiredDirection = hip.transform.right; //방향
        if (standUpFront == false) {
            desiredDirection *= -1;
        }
        desiredDirection.y = 0;
        desiredDirection.Normalize();
        
        Quaternion fromToRotation = Quaternion.FromToRotation(Vector3.forward, desiredDirection);
        cc.gameObject.transform.rotation = fromToRotation;
        
        hip.transform.position = originalPosition;
        hip.transform.rotation = originalRotation;
    }
    
    private void AlignPositionToHips() {
        Vector3 originalHipsPosition = hip.transform.position;
        cc.gameObject.transform.position = hip.transform.position;

        Vector3 offset = GetStandUpBones()[0].Position;
        offset.y = 0;
        offset = transform.rotation * offset;
        cc.gameObject.transform.position += -offset;
        
        if (Physics.Raycast(cc.gameObject.transform.position, Vector3.down, out RaycastHit hit)) {
            cc.gameObject.transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        hip.transform.position = originalHipsPosition;
    }
    
    private void PopulateBoneTransforms(BoneTransform[] bonesarray) {
        for (int i = 0; i < bonesarray.Length; i++) {
            bonesarray[i].Position = pawnBones[i].transform.localPosition;
            bonesarray[i].Rotation = pawnBones[i].transform.localRotation;
        }
    }

    private void PopulateStartBoneTransforms(string name, BoneTransform[] bonesarray) {
        Vector3 positionSample = transform.position;
        Quaternion rotationSample = transform.rotation;
        foreach (AnimationClip clip in ani.runtimeAnimatorController.animationClips) {
            if (clip.name == name) {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransforms(bonesarray);
                break;
            }
        }
        transform.position = positionSample;
        transform.rotation = rotationSample;
    }

    private string GetStandUpName() {
        return standUpFront ? standUpFrontName : standUpBackName;
    }

    private BoneTransform[] GetStandUpBones() {
        return standUpFront ? standUpFrontBones : standUpBackBones;
    }
    
    public void SetRagdollState(bool state) {
        Vector3 velocity = Vector3.zero;
        if (cc) {
            cc.enabled = !state;
            velocity = cc.velocity;
        }
        
        if (ani) {
            ani.enabled = !state;
        }

        foreach (Rigidbody rb in rbs) {
            if (state == false) {
                rb.linearVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = !state;
        }

        foreach (Collider col in cols) {
            col.isTrigger = !state;
        }
        
        Activity = state;
    }
    
    public void DeActive() {
        SetRagdollState(true);
    }
    public void Active() {
        SetRagdollState(false);
    }
}
