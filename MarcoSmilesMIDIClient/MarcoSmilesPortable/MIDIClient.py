import requests
import time
import socketio
import midiUtils as MIDI

pause_value=9999
note_converter=[]
start_note="_"
total_notes=0
previous_note=total_notes-1
diesis= True


# Crea un client SocketIO
sio = socketio.Client()

NOTE_TO_SEMITONE = {
    "DO": 0, "DO#": 1, "RE": 2, "RE#": 3, "MI": 4, "FA": 5, "FA#": 6,
    "SOL": 7, "SOL#": 8, "LA": 9, "LA#": 10, "SI": 11
}


# Generate converter
def generate_converter(start_note: str, total_notes: int, diesis_in: bool):
    # Seleziona la lista di note in base a diesis_in
    note_list = [n for n in NOTE_TO_SEMITONE.keys()] if diesis_in else [n for n in NOTE_TO_SEMITONE.keys() if
                                                                        "#" not in n]
    #start_note=start_note[:-1]
    note_list.sort(key=lambda x: NOTE_TO_SEMITONE[x])
    print("lengh of start notes", len(start_note))
    for char in start_note:
        print(char, " - ")
    #print("HERE IN CONVERTER: note name ",start_note[:-1])
    #print(" octave  ", int(start_note[-1]))
    #note_name, octave = start_note[:-1], int(start_note[-1])
    note_name = start_note
    octave = 4

    # Find start note index
    if note_name not in note_list:
        raise ValueError(f"Nota iniziale {start_note} non valida")

    start_index = note_list.index(note_name)

    converter = []

    for i in range(total_notes):
        index = (start_index + i) % len(note_list)  # Ciclo attraverso le note
        current_octave = octave + ((start_index + i) // len(note_list))  # Cambio ottava se necessario
        converter.append([NOTE_TO_SEMITONE[note_list[index]], current_octave])  # Indice MIDI e ottava

    return converter




# Evento di connessione
@sio.event
def connect():
    print("Connesso al server")

# Evento per ricevere la predizione dal server
@sio.event
def start_note(data):
    global diesis
    global note_converter
    global start_note

    start_note=data['start_note']
    print(f"START NOTE:", start_note)

    start_note= start_note[1:]
    start_note = start_note[:-1]
    total_notes = data['total_notes']
    print(f"START NOTE:", start_note)
    print(f"total note:", total_notes)

    note_converter=generate_converter(start_note,total_notes-2,diesis)
    note_converter.append([pause_value, 0])

    print(note_converter)

# Evento di disconnessione
@sio.event
def disconnect():
    print("Disconnesso dal server")

# Connetti al server Flask usando WebSocket e passando client_id nella URL
def connect_to_server():
    sio.connect("http://localhost:5005?client_id=client2", transports=['websocket'])




@sio.event
def get_prediction(data):
    global previous_note
    #prediction will contain a number between 0 and total notes, where total notes-1 = pause
    nota = data['prediction']
    isRibattuta = data['isRibattuta']

    print(f"Messaggio ricevuto dal server: {nota}, isRibattuta {isRibattuta}")
    check =0
    if(nota=="_"):
        MIDI.sendMidiUPDATED(note_converter[total_notes-1][0],note_converter[previous_note][0],note_converter[nota][1],pause_value,isRibattuta)
        #print(note_converter[int(nota)])

    if(nota!='_'):
        print("MIDI NOTE ", nota)
        #def sendMidi(current_note, previous_note, octave=1):
        MIDI.sendMidiUPDATED(note_converter[nota][0],note_converter[previous_note][0],note_converter[nota][1],pause_value,isRibattuta)
        previous_note = nota
        





'''
def note_to_midi(note, note_index: int) -> int:
    global total_notes
    print("TOTAL NOTES ", total_notes)
    if (note_index != (total_notes - 1)):
        # Separare la parte alfabetica (RE) e il numero dell'ottava (4)
        for i, char in enumerate(note):
            if char.isdigit():
                note_name = note[:i]  # Es: "RE"
                octave = int(note[i:])  # Es: 4
                break
        else:
            raise ValueError("Formato nota non valido")

        # Calcola il numero MIDI
        scale_id = NOTE_TO_SEMITONE[note_name] + note_index
    else:
        midi_number= 9999999

    return midi_number

'''

if __name__ == "__main__":

    '''
    #TESTING
    start_note="RE4"
    #Means 6 notes + pause
    total_notes=7
    diesis=False

    note_converter=generate_converter(start_note,total_notes-1,diesis);
    #Adding the pause
    note_converter.append([pause_value,0])
    previous_note=total_notes-1
    print("note converter:", note_converter)
    for k in range (4):
        for i in range (total_notes):
            print(i)
            MIDI.sendMidiUPDATED(note_converter[i][0], note_converter[previous_note][0], note_converter[i][1],pause_value)
            previous_note = i
            print(previous_note)
            time.sleep(0.3)
            MIDI.sendMidiUPDATED(note_converter[i][0], note_converter[previous_note][0], note_converter[i][1], pause_value)
            time.sleep(0.1)
            MIDI.sendMidiUPDATED(note_converter[i][0], note_converter[previous_note][0], note_converter[i][1], pause_value)
            
            print(previous_note)
            time.sleep(0.5)

    note_converter=[]
    '''
    # Avvia la connessione al server
    connect_to_server()

    # Mantieni il client in esecuzione per ricevere eventi
    sio.wait()

    

