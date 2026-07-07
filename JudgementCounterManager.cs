using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace JudgementCounter
{
    public static class JudgementCounterManager
    {
        private static GameObject _uiholder;
        private static Text _judgmentLabels;
        private static Text _judgmentCounters;

        [HarmonyPatch(typeof(GameController), nameof(GameController.doScoreText))]
        [HarmonyPostfix]
        private static void DoScorePatch(GameController __instance)
        {
            if (_judgmentCounters == null) return;

            _judgmentCounters.text = $"{__instance.scores_A}\n" +
                                     $"{__instance.scores_B}\n" +
                                     $"{__instance.scores_C}\n" +
                                     $"{__instance.scores_D}\n" +
                                     $"{__instance.scores_F}";
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        [HarmonyPostfix]
        private static void OnGameControllerStart(GameController __instance)
        {
            _uiholder = GameObject.Find("GameplayCanvas/UIHolder");
            if (_uiholder == null) return;

            //Fetch Hex from Config
            string pColor = GetValidHex(Plugin.Instance.ColorPerfect.Value, "00BFFF");
            string nColor = GetValidHex(Plugin.Instance.ColorNice.Value, "00FF00");
            string oColor = GetValidHex(Plugin.Instance.ColorOk.Value, "FFFF00");
            string mColor = GetValidHex(Plugin.Instance.ColorMeh.Value, "FF8800");
            string xColor = GetValidHex(Plugin.Instance.ColorNasty.Value, "FF0000");

            // Display Counter
            float xAnchorLabel = 0.02f;
            float xAnchorCounter = 0.04f;
            float yAnchor = 0.89f; // Default to Top Left

            if (Plugin.Instance.DisplayPosition.Value == "Bottom Left")
            {
                yAnchor = 0.01f; // Shift down for Bottom Left
            }

            // Instantiate labels
            _judgmentLabels = GameObject.Instantiate(__instance.ui_score, _uiholder.transform);
            _judgmentLabels.name = "JudgmentLabels";
            _judgmentLabels.supportRichText = true;
            _judgmentLabels.fontSize = 13;
            _judgmentLabels.alignment = TextAnchor.UpperLeft;
            _judgmentLabels.text = $"<color=#{pColor}>P</color>\n<color=#{nColor}>N</color>\n<color=#{oColor}>O</color>\n<color=#{mColor}>M</color>\n<color=#{xColor}>X</color>";

            RectTransform lRect = _judgmentLabels.GetComponent<RectTransform>();
            lRect.anchorMax = new Vector2(xAnchorLabel, yAnchor);
            lRect.anchorMin = new Vector2(xAnchorLabel, yAnchor);
            lRect.pivot = new Vector2(0.5f, 0.5f);
            lRect.anchoredPosition = Vector2.zero;
            lRect.sizeDelta = new Vector2(50f, 100f);

            // Instantiate counters
            _judgmentCounters = GameObject.Instantiate(__instance.ui_score, _uiholder.transform);
            _judgmentCounters.name = "JudgmentCounters";
            _judgmentCounters.fontSize = 13;
            _judgmentCounters.alignment = TextAnchor.UpperLeft;
            _judgmentCounters.text = "0\n0\n0\n0\n0";

            RectTransform cRect = _judgmentCounters.GetComponent<RectTransform>();
            cRect.anchorMax = new Vector2(xAnchorCounter, yAnchor);
            cRect.anchorMin = new Vector2(xAnchorCounter, yAnchor);
            cRect.pivot = new Vector2(0.5f, 0.5f);
            cRect.anchoredPosition = Vector2.zero;
            cRect.sizeDelta = new Vector2(50f, 100f);
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.Update))]
        [HarmonyPostfix]
        private static void Postfix(GameController __instance)
        {
            if (__instance.musictrack_status == GameController.track_status.ended)
            {
                if (_judgmentLabels != null) GameObject.Destroy(_judgmentLabels.gameObject);
                if (_judgmentCounters != null) GameObject.Destroy(_judgmentCounters.gameObject);
                _judgmentLabels = null;
                _judgmentCounters = null;
                _uiholder = null;
            }
        }

        private static string GetValidHex(string input, string fallback)
        {
            if (string.IsNullOrEmpty(input)) return fallback;

            string cleanInput = input.Trim().Replace("#", "");
            if (ColorUtility.TryParseHtmlString("#" + cleanInput, out _))
            {
                return cleanInput;
            }
            return fallback;
        }
    }
}