using System;
using Oculus.Interaction.Input;

public enum HandSide
{
    Left,
    Right
}

public enum UsefulJointIds
{
    HandThumb1 = HandJointId.HandThumb1,
    HandThumb2 = HandJointId.HandThumb2,
    HandThumb3 = HandJointId.HandThumb3,
    HandIndex1 = HandJointId.HandIndex1,
    HandIndex2 = HandJointId.HandIndex2,
    HandIndex3 = HandJointId.HandIndex3,
    HandMiddle1 = HandJointId.HandMiddle1,
    HandMiddle2 = HandJointId.HandMiddle2,
    HandMiddle3 = HandJointId.HandMiddle3,
    HandRing1 = HandJointId.HandRing1,
    HandRing2 = HandJointId.HandRing2,
    HandRing3 = HandJointId.HandRing3,
    HandPinky1 = HandJointId.HandPinky1,
    HandPinky2 = HandJointId.HandPinky2,
    HandPinky3 = HandJointId.HandPinky3
}

[Serializable]
public class HandCoordinates
{
    public float PositionX;
    public float PositionY;
    public float PositionZ;
}

[Serializable]
public class HandWrapper
{
    public HandCoordinates HandThumb1;
    public HandCoordinates HandThumb2;
    public HandCoordinates HandThumb3;
    public HandCoordinates HandIndex1;
    public HandCoordinates HandIndex2;
    public HandCoordinates HandIndex3;
    public HandCoordinates HandMiddle1;
    public HandCoordinates HandMiddle2;
    public HandCoordinates HandMiddle3;
    public HandCoordinates HandRing1;
    public HandCoordinates HandRing2;
    public HandCoordinates HandRing3;
    public HandCoordinates HandPinky1;
    public HandCoordinates HandPinky2;
    public HandCoordinates HandPinky3;

    public object
        this[string propertyName] // Got it on SO, let us access class instance's fields by string stored in a variable. Mindblowing :o 
    {
        get
        {
            var myType = typeof(HandWrapper);
            var myPropInfo = myType.GetField(propertyName);
            return myPropInfo.GetValue(this);
        }
        set
        {
            var myType = typeof(HandWrapper);
            var myPropInfo = myType.GetField(propertyName);
            myPropInfo.SetValue(this, value);
        }
    }
}

[Serializable]
public class HandsWrapper
{
    public HandWrapper LeftHand;
    public HandWrapper RightHand;
}