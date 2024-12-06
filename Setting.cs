using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace NoDoubleEffects
{
    [FileLocation(nameof(NoDoubleEffects))]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public Setting(IMod mod) : base(mod)
        {
        }

        public bool Button
        {
            set => NoDoubleEffectsSystem.Instance.UpdateSignatureBuildings();
        }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "No Double Effects" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "Remove Double Effects" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)),
                    $"Remove the signature effect from all signature buildings, except the first one"
                },
            };
        }

        public void Unload()
        {
        }
    }
}