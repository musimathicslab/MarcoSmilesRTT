using System.Collections;
using System.Collections.Generic;
using HGS.Tone;
using UnityEngine;
using Utilities;

public class SynthManager : MonoBehaviour
{
    private static ToneSynth _synth;
    private static Coroutine _triggerCoroutine;
    private Dictionary<MarcoNote.NoteEnum, Coroutine> _triggerCoroutines;


    // Start is called before the first frame update
    private void Start()
    {
        _synth = GetComponent<ToneSynth>();
        _synth.SetInstrument(MidiInstrumentCode.Acoustic_Grand_Piano);

        _triggerCoroutines = new Dictionary<MarcoNote.NoteEnum, Coroutine>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void PlayNote(MarcoNote key, int pitch)
    {
        if (key.Value == MarcoNote.NoteEnum.Pause)
        {
            _synth.TriggerReleaseAll(true);
            return;
        }

        var note = ToneNote.Parse($"{key.ToInternational()}{pitch}");

        if (_triggerCoroutines.TryGetValue(key.Value, out var existingCoroutine))
            if (existingCoroutine != null)
                StopCoroutine(existingCoroutine);

        _triggerCoroutines[key.Value] = StartCoroutine(TriggerAttackAndReleaseCoroutine(note, 100, 1.0f, key.Value));
    }

    private IEnumerator TriggerAttackAndReleaseCoroutine(ToneNote note, int velocity, float duration,
        MarcoNote.NoteEnum noteEnum)
    {
        _synth.TriggerAttack(note, velocity);
        yield return new WaitForSeconds(duration);
        _synth.TriggerRelease(note);

        if (_triggerCoroutines.ContainsKey(noteEnum)) _triggerCoroutines.Remove(noteEnum);
    }

    public void StopAllNotes()
    {
        _synth.TriggerReleaseAll(true);
        foreach (var coroutine in _triggerCoroutines.Values)
            if (coroutine != null)
                StopCoroutine(coroutine);

        _triggerCoroutines.Clear();
    }
}