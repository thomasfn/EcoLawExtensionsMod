using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Eco.Mods.LawExtensions
{
    using Core.Plugins.Interfaces;
    using Core.Utils;
    using Core.Utils.Threading;
    using Core.Serialization;
    using Core.Plugins;

    using Shared.Localization;
    using Shared.Utils;
    using Shared.Serialization;

    using Simulation.Time;

    using Gameplay.Items;
    using Gameplay.Components;
    using Gameplay.Objects;
    using Gameplay.PowerGrids;

    [Serialized]
    public class LawExtensionsData : Singleton<LawExtensionsData>, IStorage
    {
        public IPersistent StorageHandle { get; set; }

        public readonly PeriodicUpdateConfig UpdateTimer = new PeriodicUpdateConfig(true);

        public void InitializeRegistrars()
        {
            
        }

        public void Initialize()
        {
            this.UpdateTimer.Initialize(LawExtensionsPlugin.Obj, () => Math.Max(1.0, LawExtensionsPlugin.Obj.Config.TickInterval));
        }
    }

    [Localized]
    public class LawExtensionsConfig
    {
        [LocDescription("Seconds between each power grid law tick. Set to 0 to disable.")]
        public int TickInterval { get; set; } = 30;
    }

    [Localized, LocDisplayName(nameof(LawExtensionsPlugin)), Priority(PriorityAttribute.High)]
    public class LawExtensionsPlugin : Singleton<LawExtensionsPlugin>, IModKitPlugin, IInitializablePlugin, IThreadedPlugin, ISaveablePlugin, IContainsRegistrars, IConfigurablePlugin
    {
        public readonly PowerGridLawManager PowerGridLawManager;

        private readonly TagDefinition poweredTagDefinition = new TagDefinition("Powered") { AutoHighlight = false, ShowInEcopedia = false, ShowInFilter = true };

        static LawExtensionsPlugin()
        {
            var dynamicTagsField = typeof(TagManager).GetField("dynamicTags", BindingFlags.Static | BindingFlags.NonPublic);
            if (dynamicTagsField != null)
            {
                var oldValue = dynamicTagsField.GetValue(null) as string[];
                dynamicTagsField.SetValue(null, oldValue.Append("Powered").ToArray());
            }
            else
            {
                Logger.Error($"Failed to modify 'dynamicTags' field on TagManager (field not found)");
            }
        }

        [NotNull] private readonly LawExtensionsData data;
        [NotNull] private readonly RepeatableActionWorker powerGridLawTickWorker;

        public IPluginConfig PluginConfig => config;

        private PluginConfig<LawExtensionsConfig> config;
        public LawExtensionsConfig Config => config.Config;

        public LawExtensionsPlugin()
        {
            data = StorageManager.LoadOrCreate<LawExtensionsData>("LawExtensions");
            config = new PluginConfig<LawExtensionsConfig>("LawExtensions");
            PowerGridLawManager = new PowerGridLawManager();
            this.powerGridLawTickWorker = PeriodicWorkerFactory.Create(TimeSpan.FromSeconds(1), this.TryTickPowerGridLawManager);
        }

        public void Initialize(TimedTask timer)
        {
            data.Initialize();
            try
            {
                SetupPowerGridManagerDetour();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to setup power grid manager detour ({ex.Message}) - power related law triggers will not work!");
            }
            // Modded tags as filters for law trigger parameters don't seem to be supported for now (or maybe it's just my crazy way of making a tag...)
            // SetupPoweredTag();
        }
        public void InitializeRegistrars(TimedTask timer) => data.InitializeRegistrars();
        public string GetDisplayText() => string.Empty;
        public string GetStatus() => string.Empty;
        public string GetCategory() => Localizer.DoStr("Civics");
        public override string ToString() => Localizer.DoStr("LawExtensions");
        public void Run() => this.powerGridLawTickWorker.Start(ThreadPriorityTaskFactory.Lowest);
        public Task ShutdownAsync() => this.powerGridLawTickWorker.ShutdownAsync();
        public void SaveAll() => StorageManager.Obj.MarkDirty(data);

        public object GetEditObject() => Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public void OnEditObjectChanged(object o, string param)
        {
            this.SaveConfig();
        }

        private void SetupPowerGridManagerDetour()
        {
            var tickWorkerField = typeof(PowerGridManager).GetField("tickWorker", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception($"Failed to reflect PowerGridManager.tickWorker");
            var tickWorker = tickWorkerField.GetValue(PowerGridManager.Obj) as EventDrivenWorker ?? throw new Exception($"Failed to retrieve PowerGridManager.tickWorker");
            var repeatableActionField = typeof(EventDrivenWorker).GetField("repeatableAction", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception($"Failed to reflect EventDrivenWorker.repeatableAction");
            var oldRepeatableAction = repeatableActionField.GetValue(tickWorker) as Func<CancellationToken, Task<int>> ?? throw new Exception($"Failed to retrieve EventDrivenWorker.repeatableAction");
            Func<CancellationToken, Task<int>> newRepeatableAction = (token) =>
            {
                PowerGridLawManager.PowerGridManagerPreTick(PowerGridManager.Obj);
                return oldRepeatableAction(token).ContinueWith(t =>
                {
                    PowerGridLawManager.PowerGridManagerPostTick(PowerGridManager.Obj);
                    return t.Result;
                });
            };
            repeatableActionField.SetValue(tickWorker, newRepeatableAction);
        }

        private void SetupPoweredTag()
        {
            TagDefinition.Register(poweredTagDefinition);
            var poweredTag = TagManager.GetOrMake("Powered");
            TagManager.TypeToTags.AddToSet(typeof(PowerGridComponent), poweredTag);
            TagManager.TagToTypes.AddToSet(poweredTag, typeof(PowerGridComponent));
            var poweredWorldObjectTypes = new HashSet<Type>();
            foreach (var type in typeof(WorldObject).DerivedTypes())
            {
                if (type.Attributes<RequireComponentAttribute>().Any(x => x.ComponentType == typeof(PowerGridComponent)))
                {
                    TagManager.TypeToTags.AddToSet(type, poweredTag);
                    TagManager.TagToTypes.AddToSet(poweredTag, type);
                    poweredWorldObjectTypes.Add(type);
                }
            }
            foreach (var type in typeof(WorldObjectItem).DerivedTypes())
            {
                var worldObjectType = (Item.Get(type) as WorldObjectItem)?.WorldObjectType;
                if (worldObjectType == null) continue;
                if (poweredWorldObjectTypes.Contains(worldObjectType))
                {
                    TagManager.TypeToTags.AddToSet(type, poweredTag);
                    TagManager.TagToTypes.AddToSet(poweredTag, type);
                }
            }
        }

        private void TryTickPowerGridLawManager()
        {
            if (!this.data.UpdateTimer.DoUpdate || Config.TickInterval <= 0) { return; }
            PowerGridLawManager.Obj.DoLawTick(Config.TickInterval);
        }

    }
}
