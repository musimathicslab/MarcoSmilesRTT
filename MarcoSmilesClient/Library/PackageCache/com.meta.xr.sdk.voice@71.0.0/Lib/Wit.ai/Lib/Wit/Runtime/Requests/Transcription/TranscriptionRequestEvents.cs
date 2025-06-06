/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.Voice
{
    /// <summary>
    /// A unity event that returns a string
    /// </summary>
    [Serializable]
    public class TranscriptionRequestEvent : UnityEvent<string> {}

    /// <summary>
    /// A unity event that returns a string for the client user id and an additional string for the transcription
    /// </summary>
    [Serializable]
    public class UserTranscriptionRequestEvent : UnityEvent<string, string> {}

    [Serializable]
    public class TranscriptionRequestEvents<TUnityEvent>
        : VoiceRequestEvents<TUnityEvent>
        where TUnityEvent : UnityEventBase
    {
        /// <summary>
        /// Called every time audio input changes states
        /// </summary>
        public TUnityEvent OnAudioInputStateChange => _onAudioInputStateChange;
        [Header("Audio Events")] [Tooltip("Called every time audio input changes states.")]
        [SerializeField] private TUnityEvent _onAudioInputStateChange = Activator.CreateInstance<TUnityEvent>();

        /// <summary>
        /// Called when audio is activated for this audio transcription request
        /// </summary>
        public TUnityEvent OnAudioActivation => _onAudioActivation;
        [Tooltip("Called every time audio input changes states.")]
        [SerializeField] private TUnityEvent _onAudioActivation = Activator.CreateInstance<TUnityEvent>();
        /// <summary>
        /// Called when audio is being listened to for this request
        /// </summary>
        public TUnityEvent OnStartListening => _onStartListening;
        [Tooltip("Called when audio is being listened to for this request.")]
        [SerializeField] private TUnityEvent _onStartListening = Activator.CreateInstance<TUnityEvent>();

        /// <summary>
        /// Called when audio is deactivated for this audio transcription request
        /// </summary>
        public TUnityEvent OnAudioDeactivation => _onAudioDeactivation;
        [Tooltip("Called when audio is no longer being listened to for this request.")]
        [SerializeField] private TUnityEvent _onAudioDeactivation = Activator.CreateInstance<TUnityEvent>();
        /// <summary>
        /// Called when audio is no longer being listened to for this request
        /// </summary>
        public TUnityEvent OnStopListening => _onStopListening;
        [Tooltip("Called when audio is no longer being listened to for this request.")]
        [SerializeField] private TUnityEvent _onStopListening = Activator.CreateInstance<TUnityEvent>();

        /// <summary>
        /// Called on request transcription while audio is still being analyzed
        /// </summary>
        public TranscriptionRequestEvent OnPartialTranscription => _onPartialTranscription;
        [Header("Transcription Events")] [Tooltip("Called on request transcription while audio is still being analyzed.")]
        [SerializeField] private TranscriptionRequestEvent _onPartialTranscription = Activator.CreateInstance<TranscriptionRequestEvent>();
        /// <summary>
        /// Called on request transcription when audio has been completely transferred
        /// </summary>
        public TranscriptionRequestEvent OnFullTranscription => _onFullTranscription;
        [Tooltip("Called on request transcription when audio has been completely transferred.")]
        [SerializeField] private TranscriptionRequestEvent _onFullTranscription = Activator.CreateInstance<TranscriptionRequestEvent>();


        [Tooltip("Called on request transcription while audio is still being analyzed.  Also returns client user id as first parameter")]
        [SerializeField] private UserTranscriptionRequestEvent _onUserPartialTranscription = Activator.CreateInstance<UserTranscriptionRequestEvent>();
        public UserTranscriptionRequestEvent OnUserPartialTranscription => _onUserPartialTranscription;

        [Tooltip("Called on request transcription when audio has been completely transferred.  Also returns client user id as first parameter")]
        [SerializeField] private UserTranscriptionRequestEvent _onUserFullTranscription = Activator.CreateInstance<UserTranscriptionRequestEvent>();
        public UserTranscriptionRequestEvent OnUserFullTranscription => _onUserFullTranscription;
    }
}
