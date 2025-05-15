from datetime import datetime

import pandas as pd
import socketio
from flask import Flask, jsonify, request

from DQN.Double_DQN import Network
from utils.utils import load_action_space, read_request, toint
import os
from flask_socketio import SocketIO, emit
import shutil



app = Flask(__name__)
socketio = SocketIO(app)
# Mappa dei client connessi (per esempio, Client 1 e Client 2)
clients = {}


@app.route('/hello-world', methods=['GET'])
def hello_world():
    return jsonify({"message": "hello world!"})


@app.route('/hand-data', methods=['POST'])
def hand_data():
    try:
        global network
        predictions = []

        hand_data, note = read_request(request.json)
        print("Note received: " + str(note))

        # Predict whole batch
        for pose in hand_data:
            prediction = network.learn(pose, note)
            predictions.append(toint(prediction))

        # Get the most common prediction
        prediction = max(set(predictions), key=predictions.count)
        print("Predicted: " + str(prediction))

        # Live accuracy logic
        network.predicted_counter += 1
        if prediction == note:
            network.guessed_counter += 1

        # Bye bye
        return jsonify({
            "message": f"{prediction}",
            "accuracy": int(
                ((100 / network.predicted_counter) * network.guessed_counter) if network.predicted_counter > 0 else 0)
        })
    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500


@app.route('/et-train', methods=['GET'])
def et_train():
    try:
        csv_filename = "dati.csv"
        df = pd.read_csv(csv_filename)

        df_shuffled = df.sample(frac=1).reset_index(drop=True)

        for index, row in df_shuffled.iterrows():
            pose = row.iloc[:-1].tolist()
            note = row.iloc[-1]
            # print(f"row.iloc[:-1].tolist() {row}\n"
            #       f"pose {pose}\n"
            #       f"note {note}\n")

            network.learn(pose, note)

        return jsonify({
            "message": f"Fine training",
        })

    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500


@app.route('/et-train32', methods=['GET'])
def et_train32():
    for i in range(32):
        try:
            csv_filename = "dati.csv"
            df = pd.read_csv(csv_filename)

            df_shuffled = df.sample(frac=1).reset_index(drop=True)

            for index, row in df_shuffled.iterrows():
                pose = row.iloc[:-1].tolist()
                note = row.iloc[-1]
                # print(f"row.iloc[:-1].tolist() {row}\n"
                #       f"pose {pose}\n"
                #       f"note {note}\n")

                network.learn(pose, note)

            # return jsonify({
            #     "message": f"Fine training",
            #     })

        except Exception as e:
            print(e)
            return jsonify({
                "message": "Error processing request"}), 500

    return jsonify({
        "message": f"Fine training",
    })


@app.route('/et-hand-data', methods=['POST'])
def et_hand_data():
    try:
        csv_filename = "dati.csv"
        #csv_filename2 = "dati2.csv"  ###

        hand_data, note = read_request(request.json)
        #hand_data2 = hand_data.copy()  ###

        #hand_data *= 128
        print(f"note: {note}, hand_data: {hand_data}")

        df = pd.DataFrame(hand_data)
        df['label'] = note
        #df2 = pd.DataFrame(hand_data2)  ###
        #df2['label'] = note  ###

        file_exists = os.path.isfile(csv_filename)
        #file_exists2 = os.path.isfile(csv_filename2)  ###

        df.to_csv(csv_filename, index=False, header=not file_exists, mode='a')
        #df2.to_csv(csv_filename2, index=False, header=not file_exists2, mode='a')  ###

        return jsonify({
            "message": f"{note}",
        })
    except Exception as e:
        print(e)

#Training durante l'acquisizione-----------------------------------------------------------------------------
rtt_csv_filename = "rtt_hand_data.csv"
rttUsing_csv_filename = "rttu_hand_data.csv"
@app.route('/rtt-hand-data', methods=['POST'])
def rtt_hand_data():
    try:
        hand_data, note, _ = read_request(request.json)

        #hand_data *= 128
        #print(f"note: {note}, hand_data: {hand_data}")

        df = pd.DataFrame(hand_data)
        df['label'] = note

        file_exists = os.path.isfile(rtt_csv_filename)

        df.to_csv(rtt_csv_filename, index=False, header=not file_exists, mode='a')

        return jsonify({
            "message": f"{note}",
        })
    except Exception as e:
        print(e)
        return jsonify({
            "error": str(e)
        }), 500

@app.route('/rtt-train', methods=['GET'])
def rtt_train():
    if os.path.exists(rtt_csv_filename):
        shutil.copyfile(rtt_csv_filename, rttUsing_csv_filename)
    df = pd.read_csv(rttUsing_csv_filename)
    for i in range(16):
        try:

            df_shuffled = df.sample(frac=1).reset_index(drop=True)

            for index, row in df_shuffled.iterrows():
                pose = row.iloc[:-1].tolist()
                note = row.iloc[-1]
                # print(f"row.iloc[:-1].tolist() {row}\n"
                #       f"pose {pose}\n"
                #       f"note {note}\n")

                network.learn(pose, note)

            # return jsonify({
            #     "message": f"Fine training",
            #     })

        except Exception as e:
            print(e)
            return jsonify({
                "message": "Error processing request"}), 500

    return jsonify({
        "message": f"Fine training",
    })

@app.route('/rtt-save-model', methods=['GET'])
def rtt_training():
    try:
        timestamp = datetime.now().strftime('%Y-%m-%d_%H-%M-%S')
        nuovo_nome = f'rtt_dati_{timestamp}.csv'
        os.rename(rtt_csv_filename, nuovo_nome)

        global network
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "Network saved"}), 200
    except Exception as e:
        print(e)
        return jsonify({"message": "Error saving network"}), 500

@app.route('/rtt-new-model', methods=['POST'])
def rtt_new_model():
    try:
        if os.path.exists(rtt_csv_filename):
            os.remove(rtt_csv_filename)
        if os.path.exists(rttUsing_csv_filename):
            os.remove(rttUsing_csv_filename)
        global network
        output_dimension: int = request.json.get('output_dimension', None)
        network = Network(
            action_space_shape=output_dimension) if output_dimension is not None else Network()
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "New model created"})

    except Exception as e:
        print(e)
        return jsonify({"message": "Error creating new model"}), 500

#--------------------------------------------------------

@app.route('/hand-data-play-mode', methods=['POST'])
def hand_data_play_mode():
    try:
        global network
        predictions = []

        hand_data, _ = read_request(request.json)

        # Predict whole batch
        for pose in hand_data:
            prediction = network.agent.get_action(pose, training_mode=False)
            predictions.append(toint(prediction))

        # Get the most common prediction
        prediction = max(set(predictions), key=predictions.count)
        print("Predicted: " + str(prediction) + " with confidence: " + str(
            predictions.count(prediction) / len(predictions)))

        # Check if prediction has at least 65% confidence
        if predictions.count(prediction) / len(predictions) < 0.65:
            prediction = "_"

        # Bye bye
        return jsonify({
            "message": f"{prediction}"
        })
    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500

@app.route('/et-hand-data-play-mode', methods=['POST'])
def et_hand_data_play_mode():
    try:
        global actual_note
        global network
        predictions = []

        hand_data, _, isRibattuta = read_request(request.json)

        # Predict whole batch
        for pose in hand_data:
            if pose is None:
                if 'client2' in clients:
                    socketio.emit('get_prediction', {'prediction': "_"}, room=clients['client2'])
                return jsonify({
                    "message": f"{'_'}"
                })
            prediction = network.agent.get_action(pose, training_mode=False)
            predictions.append(toint(prediction))

        # Get the most common prediction
        prediction = max(set(predictions), key=predictions.count)
        print("Predicted: " + str(prediction) + " with confidence: " + str(
            predictions.count(prediction) / len(predictions)))

        # Check if prediction has at least a specific confidence
        if predictions.count(prediction) / len(predictions) < 0.65:
            prediction = "_"

        actual_note = prediction
        # latest_prediction = str(prediction)

        # Invia la predizione a Client 2 (se connesso) tramite WebSocket
        if 'client2' in clients:
            socketio.emit('get_prediction', {'prediction': prediction, 'isRibattuta': isRibattuta}, room=clients['client2'])

        # Bye bye
        return jsonify({
            "message": f"{prediction}"
        })
    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500

@app.route('/save-model', methods=['GET'])
def end_training():
    try:
        csv_filename = "dati.csv"
        timestamp = datetime.now().strftime('%Y-%m-%d_%H-%M-%S')
        nuovo_nome = f'dati_{timestamp}.csv'
        os.rename(csv_filename, nuovo_nome)

        global network
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "Network saved"}), 200
    except Exception as e:
        print(e)
        return jsonify({"message": "Error saving network"}), 500


@app.route('/new-model', methods=['POST'])
def new_model():
    try:
        global network
        output_dimension: int = request.json.get('output_dimension', None)
        network = Network(
            action_space_shape=output_dimension) if output_dimension is not None else Network()
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "New model created"})

    except Exception as e:
        print(e)
        return jsonify({"message": "Error creating new model"}), 500

@app.route('/start-note', methods=['POST'])
def start_note():
    try:
        global action_space_shape_dim
        global start_note
        start_note = request.json.get('start_note', None)
        # Invia la predizione a Client 2 (se connesso) tramite WebSocket
        if 'client2' in clients:
            socketio.emit('start_note', {'start_note': start_note, 'total_notes': action_space_shape_dim}, room=clients['client2'])


        print("Startnote is :",start_note)
        return jsonify({"message": "Note sent"})
    except Exception as e:
        print(e)
        print("ERROR in start note")
        return jsonify({"message": "Error sendind note"}), 500

#Gestione delle connessioni WebSocket
@socketio.on('connect')
def handle_connect():
    # Registrazione dei client connessi (se si tratta di Client 2, ad esempio)
    client_id = request.args.get('client_id')  # Puoi passare un parametro client_id per identificare i client
    if client_id:
        clients[client_id] = request.sid
        print(f"Client {client_id} connesso con SID: {request.sid}")
    else:
        print("Client connesso senza ID")

@socketio.on('disconnect')
def handle_disconnect():
    # Rimuovi i client disconnessi
    client_id = request.args.get('client_id')
    if client_id in clients:
        del clients[client_id]
    print(f"Client {client_id} disconnesso")


if __name__ == '__main__':
    action_space_shape_dim = load_action_space()
    network = Network(
        action_space_shape=action_space_shape_dim) if action_space_shape_dim is not None else Network()
    network.load_model()
    network.load_target_model()
    network.load_predicted_and_guessed()
    app.run(host='0.0.0.0', port=5005)
