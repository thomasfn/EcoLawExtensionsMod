using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace Eco.Mods.LawExtensions.CivicsImpExp
{
    public class SmartTaxGameValueMigration : ICivicsImpExpMigratorV1
    {
        private static readonly IReadOnlyDictionary<string, string> typeRemaps = new Dictionary<string, string>
        {
            { "Eco.Mods.SmartTax.CitizenPopulation", "Eco.Mods.LawExtensions.CitizenPopulation" },
            { "Eco.Mods.SmartTax.GovernmentAccountHolding", "Eco.Mods.LawExtensions.GovernmentAccountHolding" },
            { "Eco.Mods.SmartTax.SkillRate", "Eco.Mods.LawExtensions.SkillRate" },
        };

        private IEnumerable<JObject> EnumerateDeep(JObject rootObj)
        {
            yield return rootObj;
            foreach (var property in rootObj.Properties())
            {
                if (property.Value is JObject nestedObj)
                {
                    var deepObjs = EnumerateDeep(nestedObj);
                    foreach (var deepObj in deepObjs)
                    {
                        yield return deepObj;
                    }
                }
                else if (property.Value is JArray nestedArr)
                {
                    foreach (var element in nestedArr)
                    {
                        if (element is JObject elementObj)
                        {
                            var deepObjs = EnumerateDeep(elementObj);
                            foreach (var deepObj in deepObjs)
                            {
                                yield return deepObj;
                            }
                        }
                    }
                }
            }
        }

        public bool ShouldMigrate(JObject obj)
        {
            foreach (var nestedObj in EnumerateDeep(obj))
            {
                var type = nestedObj.Value<string>("type");
                if (!string.IsNullOrEmpty(type) && typeRemaps.ContainsKey(type))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ApplyMigration(JObject obj, IList<string> migrationReport)
        {
            bool didPerformMigration = false;
            foreach (var nestedObj in EnumerateDeep(obj))
            {
                var type = nestedObj.Value<string>("type");
                if (!string.IsNullOrEmpty(type) && typeRemaps.TryGetValue(type, out var remapType))
                {
                    nestedObj["type"] = remapType;
                    migrationReport.Add($"Changed a '{type}' to a '{remapType}'");
                    didPerformMigration = true;
                }
            }
            return didPerformMigration;
        }
    }
}
