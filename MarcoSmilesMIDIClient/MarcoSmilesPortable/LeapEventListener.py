import Leap
import math
import sys
from predict import load_utils, make_prediction
import pandas as pd
from midiUtils import sendMidi

# https://developer-archive.leapmotion.com/documentation/python/devguide/Leap_Controllers.html

# Last note played
last_note_played = None

class LeapEventListener(Leap.Listener):
    def on_connect(self, controller):
        print("Leap Motion connected")

    def on_disconnect(self, controller):
        print("Leap Motion disconnected")

    def on_frame(self, controller):
        global last_note_played

        frame = controller.frame()

        if not frame.hands.is_empty:
            features_values_left = []
            features_values_right = []

            for hand in frame.hands:
                if len(frame.hands) == 2:
                    if hand.is_left:
                        features_values_left = get_features(features_values_left, hand)
                    elif hand.is_right:
                        features_values_right = get_features(features_values_right, hand)

            features_values = features_values_left + features_values_right
            if len(features_values) != 0:
                # Predict
                features_values = pd.Series(features_values)
                prediction = make_prediction(features_values)

                # Send Midi Event
                sendMidi(prediction, last_note_played)

                #print(prediction)

                # Update the last note played
                last_note_played  = prediction


# This function take in input a hand and compute two values: FF and NFA
def get_features(features_values, hand):
    for i in range(0, 5):
        if i == 0:
            features_values.append(get_FF(hand.fingers[i], True))
        else:
            features_values.append(get_FF(hand.fingers[i]))

    for i in range(0, 4):
        features_values.append(get_NFA(hand.fingers[i], hand.fingers[i + 1]))

    return features_values


def get_FF(f, is_thumb=False):
    if is_thumb:
        return grads(math.degrees(f.bone(1).direction.angle_to(f.bone(3).direction)))
    else:
        return grads(math.degrees(f.bone(0).direction.angle_to(f.bone(3).direction)))


def get_NFA(f1, f2):
    return grads(math.degrees(f1.direction.angle_to(f2.direction)))


def grads(num):
    return num * 180.0 / math.pi


def main():

    # Load all files used in prediction phase
    load_utils()

    # Create a listener and controller for Leap Motion
    listener = LeapEventListener()
    controller = Leap.Controller()

    # Have the sample listener receive events from the controller
    controller.add_listener(listener)

    # Keep this process running until Enter is pressed
    print("Press Enter to quit...")
    try:
        sys.stdin.readline()
    except KeyboardInterrupt:
        pass
    finally:
        # Remove the sample listener when done
        controller.remove_listener(listener)


if __name__ == "__main__":
    main()