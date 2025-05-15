using System.Collections.Generic;
using System.Linq;
using SelectNotes;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class KeySelection : MonoBehaviour
{
    private static List<GameObject> _keys;

    private static GameObject _firstSelectedKey;
    private static GameObject _lastSelectedKey;


    public static List<GameObject> selectedKeys = new();

    public static List<Note> selectedNotes;

    private void Awake()
    {
        _keys = GetComponentsInChildren<Toggle>()
            .Select(t => t.gameObject)
            .Where(t => t.CompareTag("Key")
            ).ToList();

        foreach (var key in _keys)
        {
            //Debug.Log($"[KeySelection] Key: {key.name}");
            var keyToggle = key.GetComponent<Toggle>();
            keyToggle.onValueChanged.AddListener(_ => SelectKey(keyToggle));
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SelectKey(Toggle keyToggle)
    {
        Debug.Log($"[KeySelection] Selected key: {keyToggle.name}, {keyToggle.isOn}");

        // In case we have not yet selected or have selected both, just reset and show the first selected
        if ((_firstSelectedKey is null && _lastSelectedKey is null) ||
            (_firstSelectedKey is not null && _lastSelectedKey is not null))
        {
            ResetKeys();
            _firstSelectedKey = keyToggle.gameObject;
            _firstSelectedKey.GetComponent<Image>().color = GetSelectedColor(keyToggle.name);
        }
        else if (_firstSelectedKey is not null && _lastSelectedKey is null && keyToggle.gameObject != _firstSelectedKey)
        {
            _lastSelectedKey = keyToggle.gameObject;
            ComputeSelectedKeys();
            CreateSelectedNotes();
        }
    }

    private void ComputeSelectedKeys()
    {
        if (_firstSelectedKey != null && _lastSelectedKey != null)
        {
            var firstSelectedKeyPositionX = _firstSelectedKey.GetComponent<Transform>().position.x;
            var lastSelectedKeyPositionX = _lastSelectedKey.GetComponent<Transform>().position.x;
            foreach (var key in _keys)
            {
                var keyPositionX = key.GetComponent<Transform>().position.x;
                if ((keyPositionX >= firstSelectedKeyPositionX && keyPositionX <= lastSelectedKeyPositionX) ||
                    (keyPositionX <= firstSelectedKeyPositionX && keyPositionX >= lastSelectedKeyPositionX))
                {
                    key.GetComponent<Image>().color = GetSelectedColor(key.name);
                    selectedKeys.Add(key);
                }
            }
        }
    }

    public void CreateSelectedNotes()
    {
        selectedNotes = new List<Note>();
        foreach (var key in selectedKeys)
        {
            var note = NoteMapper.KeyToNote(key.name);
            selectedNotes.Add(note);
        }

        selectedNotes.Add(new Note(Note.NoteNameEnum.PAUSE, Note.OctaveEnum.PAUSE));
        selectedNotes = selectedNotes.OrderBy(note => note.Octave).ThenBy(note => note.NoteName).ToList();

        foreach (var (note, i) in selectedNotes.Select((e, i) => (e, i)))
            Debug.Log($"[KeySelection] in selectedNotes at {i}: {note.NoteName}, {note.Octave}");
    }

    private void ResetKeys()
    {
        foreach (var key in _keys) key.GetComponent<Image>().color = GetResetColor(key.name);
        selectedKeys.Clear();
        _firstSelectedKey = null;
        _lastSelectedKey = null;
    }

    private static Color GetSelectedColor(string keyName)
    {
        Color color;
        if (keyName.Contains("#"))
            ColorUtility.TryParseHtmlString("#2D5B2A", out color);
        else
            ColorUtility.TryParseHtmlString("#3F903A", out color);

        return color;
    }

    private static Color GetResetColor(string keyName)
    {
        Color color;
        if (keyName.Contains("#"))
            ColorUtility.TryParseHtmlString("#0E161B", out color);
        else
            ColorUtility.TryParseHtmlString("#DEE3E9", out color);

        return color;
    }
}