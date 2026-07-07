using HarmonyLib;
using TootTallyCore.Graphics;
using TootTallySettings;
using UnityEngine;
using UnityEngine.UI;

namespace JudgementCounter
{
    public static class JudgementCounterManager
    {
        private static GameObject _uiholder;
        private static Text _judgmentLabels;

        // Splitting counters into 5 text objects
        private static Text[] _counterRows = new Text[5];

        [HarmonyPatch(typeof(GameController), nameof(GameController.doScoreText))]
        [HarmonyPostfix]
        private static void DoScorePatch(GameController __instance)
        {
            if (!Plugin.Instance.ModuleConfigEnabled.Value || _counterRows[0] == null) return;

            UpdateCounterText(0, __instance.scores_A); // Perfecto
            UpdateCounterText(1, __instance.scores_B); // Nice
            UpdateCounterText(2, __instance.scores_C); // OK
            UpdateCounterText(3, __instance.scores_D); // Meh
            UpdateCounterText(4, __instance.scores_F); // Nasty
        }


        private static void UpdateCounterText(int index, int scoreValue)
        {
            string scoreString = scoreValue.ToString();
            _counterRows[index].text = scoreString;

            if (scoreString.Length >= 4)
            {
                _counterRows[index].fontSize = 12; 
            }
            else
            {
                _counterRows[index].fontSize = 13;
            }
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        [HarmonyPostfix]
        private static void OnGameControllerStart(GameController __instance)
        {
            _uiholder = GameObject.Find("GameplayCanvas/UIHolder");
            if (_uiholder == null) return;

            // Display Counter
            float lineSpacingOffset = 13f;

            var (xAnchorLabel, yAnchorLabel, xAnchorCounter, yAnchorCounter) = Plugin.Instance.DisplayPosition.Value switch
            {
                DisplayPosition.BottomLeft => (0.0175f, 0.01f, 0.03f, 0.116f),   // Shift down for Bottom Left
                _ => (0.0175f, 0.89f, 0.03f, 0.996f)                             // TopLeft or any other unsupported values
            };

            string pColor = ColorUtility.ToHtmlStringRGBA(Plugin.Instance.ColorPerfect.Value);
            string nColor = ColorUtility.ToHtmlStringRGBA(Plugin.Instance.ColorNice.Value);
            string oColor = ColorUtility.ToHtmlStringRGBA(Plugin.Instance.ColorOk.Value);
            string mColor = ColorUtility.ToHtmlStringRGBA(Plugin.Instance.ColorMeh.Value);
            string xColor = ColorUtility.ToHtmlStringRGBA(Plugin.Instance.ColorNasty.Value);

            // Label setup

            _judgmentLabels = GameObject.Instantiate(__instance.ui_score, _uiholder.transform);
            _judgmentLabels.name = "JudgmentLabels";
            _judgmentLabels.supportRichText = true;
            _judgmentLabels.fontSize = 13;
            _judgmentLabels.alignment = TextAnchor.UpperLeft;
            _judgmentLabels.horizontalOverflow = HorizontalWrapMode.Overflow;

            _judgmentLabels.text =
                $"<color=#{pColor}>P</color>\n" +
                $"<color=#{nColor}>N</color>\n" +
                $"<color=#{oColor}>O</color>\n" +
                $"<color=#{mColor}>M</color>\n" +
                $"<color=#{xColor}>X</color>";

            RectTransform lRect = _judgmentLabels.GetComponent<RectTransform>();
            lRect.anchorMax = new Vector2(xAnchorLabel, yAnchorLabel);
            lRect.anchorMin = new Vector2(xAnchorLabel, yAnchorLabel);
            lRect.pivot = Vector2.one * .5f;
            lRect.anchoredPosition = Vector2.zero;
            lRect.sizeDelta = new Vector2(50f, 100f);

            //Instantiate individual counters vertically
            for (int i = 0; i < 5; i++)
            {
                _counterRows[i] = GameObject.Instantiate(__instance.ui_score, _uiholder.transform);
                _counterRows[i].name = $"JudgmentCounter_{i}";
                _counterRows[i].fontSize = 13;
                _counterRows[i].alignment = TextAnchor.MiddleLeft;
                _counterRows[i].text = "0";

                _counterRows[i].horizontalOverflow = HorizontalWrapMode.Overflow;
                _counterRows[i].verticalOverflow = VerticalWrapMode.Overflow;
                _counterRows[i].resizeTextForBestFit = false;

                RectTransform cRect = _counterRows[i].GetComponent<RectTransform>();
                cRect.anchorMax = new Vector2(xAnchorCounter, yAnchorCounter);
                cRect.anchorMin = new Vector2(xAnchorCounter, yAnchorCounter);
                cRect.pivot = Vector2.one * .5f;

                cRect.anchoredPosition = new Vector2(0f, -(i * lineSpacingOffset));
                cRect.sizeDelta = new Vector2(40f, 15f);
            }
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.Update))]
        [HarmonyPostfix]
        private static void Postfix(GameController __instance)
        {
            if (__instance.musictrack_status == GameController.track_status.ended)
            {
                if (_judgmentLabels != null) GameObject.Destroy(_judgmentLabels.gameObject);

                for (int i = 0; i < 5; i++)
                {
                    if (_counterRows[i] != null) GameObject.Destroy(_counterRows[i].gameObject);
                    _counterRows[i] = null;
                }

                _judgmentLabels = null;
                _uiholder = null;
            }
        }

        public enum DisplayPosition
        {
            BottomLeft,
            TopLeft,
        }
    }
}