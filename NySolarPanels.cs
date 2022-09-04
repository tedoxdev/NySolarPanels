
namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Minimap;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Pipes.LiquidComponents;
    using Eco.Gameplay.Pipes.Gases;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using Eco.Shared.Items;
    using Eco.Gameplay.Pipes;
    using Eco.World.Blocks;
    using Eco.Gameplay.Housing.PropertyValues;
    using static Eco.Gameplay.Housing.PropertyValues.HomeFurnishingValue;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(PowerGeneratorComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    [PowerGenerator(typeof(ElectricPower))]
    public partial class NySolarPanelsObject : WorldObject, IRepresentsItem
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Solar Panels"); } }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public virtual Type RepresentedItemType { get { return typeof(NySolarPanelsItem); } }

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<PowerGridComponent>().Initialize(30, new ElectricPower());
            this.GetComponent<PowerGeneratorComponent>().Initialize(500);
            this.GetComponent<HousingComponent>().HomeValue = NySolarPanelsItem.homeValue;
            this.ModsPostInitialize();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Solar Panels")]
    [Ecopedia("Crafted Objects", "Power Generation", createAsSubPage: true, display: InPageTooltip.DynamicTooltip)]
    public partial class NySolarPanelsItem : WorldObjectItem<NySolarPanelsObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Modern Solar Panels");

        public override DirectionAxisFlags RequiresSurfaceOnSides { get;} = 0 | DirectionAxisFlags.Down;

        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new HomeFurnishingValue()
        {
            Category                 = RoomCategory.Industrial,
            TypeForRoomLimit         = Localizer.DoStr(""),
        };

        [Tooltip(8)] private LocString PowerProductionTooltip  => Localizer.Do($"Produces: {Text.Info(500)}w of {new ElectricPower().Name} power");
    }

    [RequiresSkill(typeof(ElectronicsSkill), 1)]
    public partial class NySolarPanelsRecipe : RecipeFamily
    {
        public NySolarPanelsRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "SolarPanels",  //noloc
                Localizer.DoStr("Solar Panels"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(SteelBarItem), 25, typeof(ElectronicsSkill), typeof(ElectronicsLavishResourcesTalent)),
                    new IngredientElement(typeof(CopperWiringItem), 40, typeof(ElectronicsSkill), typeof(ElectronicsLavishResourcesTalent)),
                    new IngredientElement(typeof(BasicCircuitItem), 10, typeof(ElectronicsSkill), typeof(ElectronicsLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<NySolarPanelsItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 10;
            this.LaborInCalories = CreateLaborInCaloriesValue(2000, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(NySolarPanelsRecipe), 40, typeof(ElectronicsSkill), typeof(ElectronicsFocusedSpeedTalent), typeof(ElectronicsParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Solar Panels"), typeof(NySolarPanelsRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}
