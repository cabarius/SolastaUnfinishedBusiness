﻿using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class TranslationsDisplay
{
    internal static void DisplayTranslations()
    {
        UI.Label();
        UI.Label(Gui.Format("ModUi/&Campaigns"));
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&TargetLanguage"), UI.Width((float)120));

            var intValue = Array.IndexOf(TranslatorContext.AvailableLanguages, Main.Settings.SelectedLanguageCode);

            if (UI.SelectionGrid(
                    ref intValue,
                    TranslatorContext.AvailableLanguages,
                    TranslatorContext.AvailableLanguages.Length,
                    3, UI.Width((float)300)))
            {
                Main.Settings.SelectedLanguageCode = TranslatorContext.AvailableLanguages[intValue];
            }
        }

        UI.Label();

        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        foreach (var userCampaign in userCampaignPoolService.AllCampaigns
                     .Where(x => !x.TechnicalInfo.StartsWith(TranslatorContext.TranslatorBehaviour.UbTranslationTag))
                     .OrderBy(x => x.Author)
                     .ThenBy(x => x.Title))
        {
            var exportName = userCampaign.Title;

            using (UI.HorizontalScope())
            {
                string buttonLabel;

                UI.Label(
                    userCampaign.Author.Substring(0, Math.Min(16, userCampaign.Author.Length)).Bold().Orange(),
                    UI.Width((float)120));
                UI.Label(userCampaign.Title.Bold().Italic(), UI.Width((float)300));

                if (TranslatorContext.TranslatorBehaviour.CurrentExports.TryGetValue(exportName, out var status))
                {
                    buttonLabel = Gui.Format("ModUi/&TranslateCancel", status.LanguageCode.ToUpper(),
                        $"{status.PercentageComplete:00.0%}").Bold().Khaki();
                }
                else
                {
                    buttonLabel = Gui.Localize("ModUi/&Translate");
                }

                UI.ActionButton(buttonLabel, () =>
                    {
                        if (status == null)
                        {
                            TranslatorContext.TranslatorBehaviour.TranslateUserCampaign(
                                Main.Settings.SelectedLanguageCode, userCampaign.Title, userCampaign);
                        }
                        else
                        {
                            TranslatorContext.TranslatorBehaviour.Cancel(userCampaign.Title);
                        }
                    },
                    UI.Width((float)200));
            }
        }

        UI.Label();
    }
}
