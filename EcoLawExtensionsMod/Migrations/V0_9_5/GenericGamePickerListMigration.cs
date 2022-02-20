namespace Eco.Mods.LawExtensions.Migrations.V0_9_5
{
    using Eco.Core.Serialization.Migrations;
    using Eco.Core.Serialization.Migrations.Attributes;
    using Eco.Core.Serialization.Migrations.DataMigrations;
    using Eco.Gameplay.Civics.GameValues;
    using Eco.Gameplay.Objects;
    using Eco.Simulation.Types;

    ///<summary> Migrates all existing <see cref="GamePickerList"/>s to <see cref="GamePickerList{T}"/>, using <see cref="GamePickerList.MustDeriveType"/> as 'T'.</summary>
    [Migration(SinceVersion = 3.912f)]
    [MigrationType(typeof(GamePickerList<WorldObject>))]
    [MigrationType(typeof(GamePickerList<PlantSpecies>))]
    public class GenericGamePickerListMigration : AggregateMigration
    {
        public GenericGamePickerListMigration()
        {
            //                                        Full path to type affected by this migration                     Migrated member name
            this.MigrateToGamePickerList<PlantSpecies>("Eco.Mods.LawExtensions.DistanceToClosestPlant", "PlantType");
            this.MigrateToGamePickerList<WorldObject>("Eco.Mods.LawExtensions.DistanceToClosestWorldObject", "ObjectType");

        }

        // Migrates the type of a member with specified name, from specified class/type, to GamePickerList<T> with specified <T>.
        void MigrateToGamePickerList<T>(string typeName, string memberName)
        {
            string targetType = SchemaUtils.GetSchemaType(typeof(GamePickerList<T>));                              // Get a usable-by-schema string representing the type we're migrating the member to
            this.AddDataMigration(typeName, MigrateClass);                                                         // Add a migration to the scope, with an action as instruction

            void MigrateClass(DataMigration typeDM) => typeDM.MigrateMember(memberName, MigrateMemberType);        // Migration instructions for the class  --> Migrate member with specified name
            void MigrateMemberType(DataMigration memberDM) => memberDM.ChangeSchemaType(targetType);               // Migration instructions for the member --> Change type of member
        }
    }
}