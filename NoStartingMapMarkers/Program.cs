using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace NoStartingMapMarkers
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "No Starting Map Markers.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {

            var mapMarkerFormKey = Skyrim.Static.MapMarker.FormKey;

            foreach (var placedObjectGetter in state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache))
            {
                if (placedObjectGetter.Record.Base.FormKey == mapMarkerFormKey && placedObjectGetter.Record.MapMarker != null && placedObjectGetter.Record.MapMarker.Flags.HasFlag(MapMarker.Flag.Visible))
                {
                    IPlacedObject copiedPlacedObject = placedObjectGetter.GetOrAddAsOverride(state.PatchMod);
                    if (copiedPlacedObject.MapMarker != null) copiedPlacedObject.MapMarker.Flags &= ~MapMarker.Flag.Visible;
                }
            };

        }
    }
}
