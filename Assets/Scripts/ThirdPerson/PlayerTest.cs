using UnityEngine;

public class PlayerTest : MonoBehaviour {
    private Rigidbody[] rbs;
    private Animator ani;
    private CharacterController cc;
    
    private PlayerState state = PlayerState.Idle;
    private float timeWakeup;

    [SerializeField]
    private string standupName;
    [SerializeField]
    private string standupClipName;
    
    [SerializeField]
    private string standupBackName;
    [SerializeField]
    private string standupBackClipName;
 
    [SerializeField] private float timeReset;
    private float timeResetTimer;
    
    private Transform hipsBone;
    private BoneTransform[] standBones;
    private BoneTransform[] standBackBones;
    private BoneTransform[] ragdollBones;
    private Transform[] bones;
    private bool standBack;
    
    private void Awake() {
        rbs = GetComponentsInChildren<Rigidbody>();
        cc = GetComponentInParent<CharacterController>();
        ani = GetComponentInParent<Animator>();
    }

    private void Start() {
        hipsBone = ani.GetBoneTransform(HumanBodyBones.Hips);
        bones = hipsBone.GetComponentsInChildren<Transform>();
        standBones = new BoneTransform[bones.Length];
        standBackBones = new BoneTransform[bones.Length];
        ragdollBones = new BoneTransform[bones.Length];

        for (int i = 0; i < bones.Length; i++) {
            standBones[i] = new BoneTransform();
            standBackBones[i] = new BoneTransform();
            ragdollBones[i] = new BoneTransform();
        }
        
        PopulateStartBoneTransforms(standupClipName, standBones);
        PopulateStartBoneTransforms(standupBackClipName, standBackBones);
        DisableRagdoll();
    }

    private void Update() {
        switch (state) {
            case PlayerState.Idle:
                WalkingBehaviour();
                break;
            case PlayerState.Ragdoll:
                RagdollBehaviour();
                break;
            case PlayerState.Standup:
                StandingBehaviour();
                break;
            //case PlayerState.LerpBones:
                ResetBehaviour();
                break;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitpoint) {
        EnableRagdoll();
        
        //Rigidbody hitrb = rbs.OrderBy(rb => Vector3.Distance(rb.position, hitpoint)).First();
        Rigidbody hitrb = FindHitRigidbody(hitpoint);
        hitrb.AddForceAtPosition(force, hitpoint, ForceMode.Impulse);
        state = PlayerState.Ragdoll;
        timeWakeup = 5f;
    }

    private Rigidbody FindHitRigidbody(Vector3 point) {
        Rigidbody closetRigidbody = null;
        float closestDistance = 0;

        foreach (var rb in rbs) {
            float distance = Vector3.Distance(rb.position, point);
            if (closetRigidbody == null || distance < closestDistance) {
                closetRigidbody = rb;
                closestDistance = distance;
            }
        }
        
        return closetRigidbody;
    }
    
    private void DisableRagdoll() {
        foreach (var rb in rbs) {
            rb.isKinematic = true;
        }
        ani.enabled = true;
        cc.enabled = true;
    }
    
    private void EnableRagdoll() {
        foreach (var rb in rbs) {
            rb.isKinematic = false;
        }
        
        ani.enabled = false;
        cc.enabled = false;
    }

    private void WalkingBehaviour() {
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();
        
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);
    }

    private void RagdollBehaviour() {
        timeWakeup += -Time.deltaTime;

        if (timeWakeup <= 0) {
            standBack = hipsBone.forward.y < 0;
            AlignRoatationToHips();
            AlignPositionToHips();
            
            PopulateBoneTransforms(ragdollBones);
            //state = PlayerState.LerpBones;
            timeResetTimer = 0f;
        }
    }

    private void StandingBehaviour() {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName(GetStandBackName()) == false) {
            state = PlayerState.Idle;
        }
}
    
    private void ResetBehaviour() {
        timeResetTimer += Time.deltaTime;
        float ResetPercentage = timeResetTimer / timeReset;

        BoneTransform[] standupBones = GetStandBackBones();
        
        for (int i = 0; i < bones.Length; i++) {
            bones[i].localPosition = Vector3.Lerp(ragdollBones[i].Position, standupBones[i].Position, ResetPercentage);
            bones[i].localRotation = Quaternion.Lerp(ragdollBones[i].Rotation, standupBones[i].Rotation, ResetPercentage);
        }
        if (ResetPercentage >= 1) {
            state = PlayerState.Standup;
            DisableRagdoll();
            
            ani.Play(GetStandBackName(), 0, 0);
        }
    }

    private void AlignRoatationToHips() {
        Vector3 originalPosition = hipsBone.position;
        Quaternion originalRotation = hipsBone.rotation;
        Vector3 desiredDirection = hipsBone.up;
        if (standBack == false) {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0;
        desiredDirection.Normalize();
        
        Quaternion fromToRotation = Quaternion.FromToRotation(Vector3.forward, desiredDirection);
        transform.rotation = fromToRotation;
        
        hipsBone.position = originalPosition;
        hipsBone.rotation = originalRotation;
    }
    
    private void AlignPositionToHips() {
        Vector3 originalHipsPosition = hipsBone.position;
        transform.position = originalHipsPosition;

        Vector3 offset = GetStandBackBones()[0].Position;
        offset.y = 0;
        offset = transform.rotation * offset;
        transform.position += -offset;
        
        
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit)) {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            
        }
        
        hipsBone.position = originalHipsPosition;
    }
    
    private void PopulateBoneTransforms(BoneTransform[] bonesarray) {
        for (int i = 0; i < bonesarray.Length; i++) {
            bonesarray[i].Position = bones[i].transform.localPosition;
            bonesarray[i].Rotation = bones[i].transform.localRotation;
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

    private string GetStandBackName() {
        return standBack == false ? standupName : standupBackName;
    }

    private BoneTransform[] GetStandBackBones() {
        return standBack == false ? standBones : standBackBones;
    }
}
