using System;
using System.Linq;
using Oculus.Interaction.Input;
using UnityEngine;

public class HandsManager : MonoBehaviour
{
    public enum HandSide
    {
        Right,
        Left
    }

    [SerializeField] private Hand rightHand;
    [SerializeField] private Hand leftHand;
    
    [SerializeField] private OVRCameraRig cameraRig;
    private Transform _centerEyeTransform;

    // Start is called before the first frame update
    private void Start()
    {
        _centerEyeTransform = cameraRig.centerEyeAnchor;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public bool IsRightHandTracked()
    {
        return rightHand.IsConnected && rightHand.GetData().IsTracked;
    }

    public bool IsLeftHandTracked()
    {
        return leftHand.IsConnected && leftHand.GetData().IsTracked;
    }

    public bool AreHandsTracked()
    {
        return IsLeftHandTracked() && IsRightHandTracked();
    }

    public Pose[] GetJoinPosesFromWrist(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Right:
            {
                rightHand.GetJointPosesFromWrist(out var jointPosesFromWrist);
                return jointPosesFromWrist.ToArray();
            }
            case HandSide.Left:
            {
                leftHand.GetJointPosesFromWrist(out var jointPosesFromWrist);
                return jointPosesFromWrist.ToArray();
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(handSide), handSide, "Valore HandSide non previsto!");
        }
    }

    public Pose GetRootPose(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Right:
            {
                rightHand.GetRootPose(out var rootPose);
                return rootPose;
            }
            case HandSide.Left:
            {
                leftHand.GetRootPose(out var rootPose);
                return rootPose;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(handSide), handSide, "Valore HandSide non previsto!");
        }
    }

    public (float rightHeight, float leftHeight) HandsHeightFromCenterEye()
    {
        var rightHandPose = GetRootPose(HandSide.Right);
        var leftHandPose = GetRootPose(HandSide.Left);
        
        var rightHandY = rightHandPose.position.y;
        var leftHandY = leftHandPose.position.y;
                
        var rightHeight = rightHandY - _centerEyeTransform.position.y;
        var leftHeight = leftHandY - _centerEyeTransform.position.y;
        
        return (rightHeight, leftHeight);
    }
}