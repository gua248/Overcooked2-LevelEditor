using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using InControl;


namespace LevelEditor
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-99)]
    public class Debugger : MonoBehaviour
    {
        [SerializeField] Key debugKeyPickup = Key.Space;
        [SerializeField] Key debugKeyInteract = Key.G;
        [SerializeField] Key debugKeyDash = Key.F;
        [SerializeField] Key debugKeyEmote = Key.E;
        [SerializeField] Key debugKeyShift = Key.H;

        [SerializeField] private FlowControllerBase flowController;

        private void Awake()
        {
            //Debug.Log("Debug.Awake." + (Application.isPlaying ? "Play" : "Edit"));
            if (Application.isPlaying && Application.isEditor)
            {
                KeyboardBindings defaultKeyboardBindings = (KeyboardBindings)typeof(PCPadInputProvider)
                    .GetField("m_DefaultKeyboardBindings", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(null);
                defaultKeyboardBindings.m_CombinedKeyboard.m_ButtonBindings[ControlPadInput.Button.A] = new List<Key> { debugKeyPickup };
                defaultKeyboardBindings.m_CombinedKeyboard.m_ButtonBindings[ControlPadInput.Button.X] = new List<Key> { debugKeyInteract };
                defaultKeyboardBindings.m_CombinedKeyboard.m_ButtonBindings[ControlPadInput.Button.B] = new List<Key> { debugKeyDash };
                defaultKeyboardBindings.m_CombinedKeyboard.m_ButtonBindings[ControlPadInput.Button.Y] = new List<Key> { debugKeyEmote };
                defaultKeyboardBindings.m_CombinedKeyboard.m_ButtonBindings[ControlPadInput.Button.LB] = new List<Key> { debugKeyShift };
                PCPadInputProvider.RestoreDefaultBindings();
            }

            if (flowController != null)
            {
                int singleplayerChopTimeMultiplier = (Application.isPlaying && Application.isEditor) ? 1 : 5;
                flowController.m_gameConfig.SingleplayerChopTimeMultiplier = singleplayerChopTimeMultiplier;
            }
        }

        private void Start()
        {
            //Debug.Log("Debug.Start." + (Application.isPlaying ? "Play" : "Edit"));
        }

        private void OnEnable()
        {
            //Debug.Log("Debug.OnEnable." + (Application.isPlaying ? "Play" : "Edit"));
            if (!Application.isPlaying)
            {
                //var audio = PseudoPrefabManager.GetAssetBundle(pseudo.bundleName).LoadAsset<AudioClip>(pseudo.assetPath);
                //audioSource.clip = audio;
                //audioSource.Play();
                //Debug.Log(Application.persistentDataPath);
            }
        }

        private void OnDisable()
        {
            //Debug.Log("Debug.OnDisable." + (Application.isPlaying ? "Play" : "Edit"));
            if (!Application.isPlaying)
            {
                //audioSource.Stop();
            }
        }

        private void OnDestroy()
        {
            //Debug.Log("Debug.OnDestroy." + (Application.isPlaying ? "Play" : "Edit"));
        }
    }
}
