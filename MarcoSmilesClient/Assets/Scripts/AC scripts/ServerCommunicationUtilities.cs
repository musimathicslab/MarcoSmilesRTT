using System;
using System.Collections.Generic;

[Serializable]
public class RequestWrapper
{
    public List<HandWrapper> LeftHandWrappers;
    public List<HandWrapper> RightHandWrappers;
    public int Note;
    public bool isRibattuta;


    public RequestWrapper()
    {
        LeftHandWrappers = new List<HandWrapper>();
        RightHandWrappers = new List<HandWrapper>();
        isRibattuta = false;
    }

    public RequestWrapper(bool isRibattuta)
    {
        LeftHandWrappers = new List<HandWrapper>();
        RightHandWrappers = new List<HandWrapper>();
        this.isRibattuta = isRibattuta;
    }

    public RequestWrapper(int note)
    {
        LeftHandWrappers = new List<HandWrapper>();
        RightHandWrappers = new List<HandWrapper>();
        Note = note;
        isRibattuta = false;
    }
}