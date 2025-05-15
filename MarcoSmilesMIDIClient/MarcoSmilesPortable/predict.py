import pickle
import os
import numpy as np

model = None
lbl_notes = None
min_values = None
max_values = None

def clean_utils_dir():
    utils_folder = "utils"
    files_to_maintain = ["min&max_values_dataset_out_1H.txt", "lbl_notes.txt", "model_1H.pkl"]

    # Get file list from utils folder
    files = os.listdir(utils_folder)

    # Delete unwanted
    for file in files:
        if file not in files_to_maintain:
            file_path = os.path.join(utils_folder, file)
            os.remove(file_path)


def load_utils():

    # Remove unwanted files from Exported dataset
    clean_utils_dir()

    global model, lbl_notes, min_values, max_values

    model_file = 'utils/model_1H.pkl'
    with open(model_file, 'rb') as f:
        model = pickle.load(f)

    lbl_notes_file = 'utils/lbl_notes.txt'
    with open(lbl_notes_file, 'r') as f:
        lines = f.readlines()
        # clean the labels from new line characters
        lbl_notes = [int(line.strip()) for line in lines]

    min_values, max_values = get_min_max_from_file()


def get_min_max_from_file():
    min_max_file = 'utils/min&max_values_dataset_out_1H.txt'
    with open(min_max_file, 'r') as f:
        lines = f.readlines()
        # clean \n
        lines = [(line.strip()) for line in lines]
        min_values = list(map(float, lines[0].strip().split(' ')))
        max_values = list(map(float, lines[1].strip().split(' ')))

        return min_values, max_values


def compute_min_max(features, min_values, max_values):
    scaled_features = []
    for f_v, min_v, max_v in zip(features, min_values, max_values):
        scaled_features.append((f_v - min_v) / (max_v - min_v))

    return scaled_features


def make_prediction(new_data_x):
    global model, lbl_notes, min_values, max_values

    # Normalize data
    new_data_scaled = compute_min_max(new_data_x, min_values, max_values)
    # print(new_data_scaled)

    new_data_reshaped = np.array(new_data_scaled).reshape(1, -1)
    # print(new_data_reshaped)

    # Input note encoded -> [0,4,1,2,3,5,7,6,8] = [Do,Mi, Do#, Re, Re#, ...]
    # Output model->[0,1,2,3,4,5,6,7]
    prediction = model.predict(new_data_reshaped)
    decoded_prediction = lbl_notes[prediction[0]]

    return decoded_prediction

