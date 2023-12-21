using GameNetcodeStuff;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MelonLoader.MelonLogger;
using UnityEngine.InputSystem;
using UnityEngine;

namespace CameraPlus
{
    internal class CameraPatch
    {
        public static HarmonyLib.Harmony instance = new("Cyconi");
        public static void StartPatch() 
        {
            try
            {
                instance.Patch(typeof(PlayerControllerB).GetMethod("Awake", AccessTools.all),
                    postfix: new HarmonyMethod(typeof(CameraPatch).GetMethod(nameof(Awake), BindingFlags.NonPublic | BindingFlags.Static)));

                CLog.L("Awake Patched!");
            }
            catch (Exception) { CLog.L("Failed to Patch Awake"); }
            try
            {
                instance.Patch(typeof(PlayerControllerB).GetMethod("Update", AccessTools.all),
                    prefix: new HarmonyMethod(typeof(CameraPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static)),
                    postfix: new HarmonyMethod(typeof(CameraPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static)));

                CLog.L("Update Patched!");
            }
            catch (Exception) { CLog.L("Failed to Patch Update"); }
        }
        static float targetFov = 90f;
        static float originalFov = 90f;
        static float currentFov = 90f;
        static float regularFov = 90f;
        static float zoomModifier = .5f;
        static Vector3 visorScale;
        static int beforeItemSlot;
        private static void Awake() // runs x the amount of times there are players
        {
            visorScale = new Vector3(0f, 0f, 0f);
        }
        private static void Prefix(PlayerControllerB __instance)
        {
            beforeItemSlot = __instance.currentItemSlot;
            currentFov = __instance.gameplayCamera.fieldOfView;
            __instance.localVisor.localScale = visorScale;
        }
        internal static void Postfix(PlayerControllerB __instance)
        {
            var mouse = Mouse.current;

            try
            {
                if (__instance.inTerminalMenu || __instance.IsInspectingItem)
                    regularFov = 60;
                else if (__instance.isSprinting)
                    regularFov = originalFov * 1.125f;
                else 
                    regularFov = originalFov;

                if (mouse.rightButton.isPressed)
                {
                    float mouseWheel = mouse.scroll.ReadValue().y;
                    if (mouseWheel > 0f)
                        zoomModifier -= 0.01f;
                    else if (mouseWheel < 0f)
                        zoomModifier += 0.01f;
                    zoomModifier = Mathf.Clamp(zoomModifier, 0.25f, 0.75f);

                    __instance.gameplayCamera.fieldOfView = Mathf.Lerp(currentFov, regularFov * zoomModifier, Time.deltaTime * 10f);
                }
                else
                    __instance.gameplayCamera.fieldOfView = Mathf.Lerp(currentFov, regularFov, Time.deltaTime * 10f);
            }
            catch { }
        }
    }
}
