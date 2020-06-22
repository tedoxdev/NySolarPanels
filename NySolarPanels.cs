namespace Eco.Mods.TechTree
{
    // [DoNotLocalize]
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
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
	
	[Serialized]
	[RequireComponent(typeof(OnOffComponent))]  
	[RequireComponent(typeof(PropertyAuthComponent))]
	[RequireComponent(typeof(PowerGridComponent))]              
    [RequireComponent(typeof(PowerGeneratorComponent))]
	[RequireComponent(typeof(HousingComponent))]                  
    [RequireComponent(typeof(SolidGroundComponent))] 
    public partial class NySolarPanelsObject : 
        WorldObject,    
        IRepresentsItem
    {
        public override LocString DisplayName { get { return Localizer.DoStr("NySolarPanels"); } } 

        public virtual Type RepresentedItemType { get { return typeof(NySolarPanelsItem); } } 

        static NySolarPanelsObject()
        {
            WorldObject.AddOccupancy<NySolarPanelsObject>(new List<BlockOccupancy>(){
				new BlockOccupancy(new Vector3i(0, 0, 0)),
				new BlockOccupancy(new Vector3i(0, 0, 1)),
				new BlockOccupancy(new Vector3i(0, 0, -1)),
            });
        }
		
		protected override void Initialize()
        {             
            this.GetComponent<PowerGridComponent>().Initialize(30, new ElectricPower());        
            this.GetComponent<PowerGeneratorComponent>().Initialize(500);                       
            this.GetComponent<HousingComponent>().Set(SolarGeneratorItem.HousingVal);                                
        }

        public override void Destroy()
        {
            base.Destroy();
        }
       
    }

    [Serialized]
    public partial class NySolarPanelsItem :
        WorldObjectItem<NySolarPanelsObject> 
    {
        public override LocString DisplayName { get { return Localizer.DoStr("NySolarPanels"); } } 
        public override LocString DisplayDescription  { get { return Localizer.DoStr("Solar panels"); } }

        static NySolarPanelsItem()
        {
            
        }
		
		[TooltipChildren] public HousingValue HousingTooltip { get { return HousingVal; } }
        [TooltipChildren] public static HousingValue HousingVal { get { return new HousingValue() 
                                                {
                                                    Category = "Industrial",
                                                    TypeForRoomLimit = "", 
        };}}
        
        [Tooltip(8)] private LocString PowerProductionTooltip  { get { return new LocString(string.Format(Localizer.DoStr("Produces: {0}w"), Text.Info(500))); } } 
        
    }
	
	[RequiresSkill(typeof(ElectronicsSkill), 0)]      
    public partial class NySolarPanelsRecipe : Recipe
    {
        public NySolarPanelsRecipe()
        {
            this.Products = new CraftingElement[]
            {
                new CraftingElement<NySolarPanelsItem>(),
            };

            this.Ingredients = new CraftingElement[]
            {
                new CraftingElement<SteelItem>(typeof(ElectronicsSkill), 25, ElectronicsSkill.MultiplicativeStrategy, typeof(ElectronicsLavishResourcesTalent)),
                new CraftingElement<CopperWiringItem>(typeof(ElectronicsSkill), 40, ElectronicsSkill.MultiplicativeStrategy, typeof(ElectronicsLavishResourcesTalent)),
                new CraftingElement<CircuitItem>(typeof(ElectronicsSkill), 10, ElectronicsSkill.MultiplicativeStrategy, typeof(ElectronicsLavishResourcesTalent)),          
            };
            this.ExperienceOnCraft = 10;  
            this.CraftMinutes = CreateCraftTimeValue(typeof(NySolarPanelsRecipe), Item.Get<NySolarPanelsItem>().UILink(), 40, typeof(ElectronicsSkill), typeof(ElectronicsFocusedSpeedTalent), typeof(ElectronicsParallelSpeedTalent));    
            this.Initialize(Localizer.DoStr("Solar Panels"), typeof(NySolarPanelsRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
	
}