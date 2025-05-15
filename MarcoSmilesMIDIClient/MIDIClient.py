import requests
import time
import socketio

start_note="_"
total_notes=0
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

    
    note_list.sort(key=lambda x: NOTE_TO_SEMITONE[x])

    
    note_name, octave = start_note[:-1], int(start_note[-1])

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


# Test della funzione
start_note = "RE4"
total_notes = 6

# Senza diesis
converter_no_diesis = generate_converter(start_note, total_notes, diesis_in=False)
print("Senza diesis:", converter_no_diesis)

# Con diesis
converter_with_diesis = generate_converter(start_note, total_notes, diesis_in=True)
print("Con diesis:", converter_with_diesis)


# Evento di connessione
@sio.event
def connect():
    print("Connesso al server")

# Evento per ricevere la predizione dal server
@sio.event
def start_note(data):
    global start_note
    start_note=data['start_note']
    
    start_note= start_note[1:]
    total_notes = data['total_notes']
    print(f"START NOTE:", start_note)
    print(f"total note:", total_notes)

# Evento di disconnessione
@sio.event
def disconnect():
    print("Disconnesso dal server")

# Connetti al server Flask usando WebSocket e passando client_id nella URL
def connect_to_server():
    sio.connect("http://localhost:5030?client_id=client2", transports=['websocket'])


NOTE_TO_SEMITONE = {}
OCTAVES = range(3, 6)  # Da DO3 a SI5



@sio.event
def get_prediction(data):
        nota =data['prediction']
        print(f"Messaggio ricevuto dal server: {nota}")
        if(nota!='_'):
            print("MIDI NOTE", nota, "  MIDI CODE - ",note_to_midi(start_note,nota));






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



if __name__ == "__main__":
    # Lista delle note con i diesis
    NOTE_INDEXES = ["DO", "DO#", "RE", "RE#", "MI", "FA", "FA#", "SOL", "SOL#", "LA", "LA#", "SI"]

    NOTE_TO_INDEX = {}

    # Generiamo le note per ogni ottava da 3 a 5
    for octave in range(3, 6):
        for index, note in enumerate(NOTE_INDEXES):
            key = f"{note}{octave}"  # Es. "DO3", "DO#3", "RE3", ...
            NOTE_TO_INDEX[key] = {"index": index, "octave": octave}

    # Stampa il dizionario generato
    for key, value in NOTE_TO_INDEX.items():
        print(f"{key}: {value}")

    # Avvia la connessione al server
    connect_to_server()

    # Mantieni il client in esecuzione per ricevere eventi
    sio.wait()

