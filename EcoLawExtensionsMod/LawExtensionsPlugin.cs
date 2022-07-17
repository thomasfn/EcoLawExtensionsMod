using System;
using System.Diagnostics.CodeAnalysis;

namespace Eco.Mods.LawExtensions
{
    using Core.Plugins.Interfaces;
    using Core.Utils;
    using Core.Serialization;
    using Core.Plugins;

    using Shared.Localization;
    using Shared.Utils;
    using Shared.Serialization;

    using Gameplay.Systems.Messaging.Chat.Commands;

    [Serialized]
    public class LawExtensionsData : Singleton<LawExtensionsData>, IStorage
    {
        public IPersistent StorageHandle { get; set; }

        public void InitializeRegistrars()
        {
            
        }

        public void Initialize()
        {
            
        }
    }

    [Localized]
    public class LawExtensionsConfig
    {
        
    }

    [Localized, LocDisplayName(nameof(LawExtensionsPlugin)), Priority(PriorityAttribute.High)]
    public class LawExtensionsPlugin : Singleton<LawExtensionsPlugin>, IModKitPlugin, IInitializablePlugin, ISaveablePlugin, IContainsRegistrars, IConfigurablePlugin
    {
        [NotNull] private readonly LawExtensionsData data;

        public IPluginConfig PluginConfig => config;

        private PluginConfig<LawExtensionsConfig> config;
        public LawExtensionsConfig Config => config.Config;

        public LawExtensionsPlugin()
        {
            data = StorageManager.LoadOrCreate<LawExtensionsData>("LawExtensions");
            config = new PluginConfig<LawExtensionsConfig>("LawExtensions");
        }

        public void Initialize(TimedTask timer) => data.Initialize();
        public void InitializeRegistrars(TimedTask timer) => data.InitializeRegistrars();
        public string GetDisplayText() => string.Empty;
        public string GetStatus() => string.Empty;
        public string GetCategory() => Localizer.DoStr("Civics");
        public override string ToString() => Localizer.DoStr("LawExtensions");
        public void SaveAll() => StorageManager.Obj.MarkDirty(data);

        public object GetEditObject() => Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public void OnEditObjectChanged(object o, string param)
        {
            this.SaveConfig();
        }
    }
}