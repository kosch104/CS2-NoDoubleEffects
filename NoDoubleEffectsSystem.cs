using System.Collections.Generic;
using Game;
using Game.Buildings;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace NoDoubleEffects;

public partial class NoDoubleEffectsSystem : GameSystemBase
{
    public static NoDoubleEffectsSystem Instance;
    private PrefabSystem m_PrefabSystem;
    private EntityQuery signatureBuildingsQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        Instance = this;
        Enabled = true;

        m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        EntityQueryDesc signatureBuildingsQueryDesc = new EntityQueryDesc
        {
            All =
            [
                ComponentType.ReadOnly<Signature>(),
            ],
            Any =
            [

            ]
        };

        signatureBuildingsQuery = GetEntityQuery(signatureBuildingsQueryDesc);
        // Loop through all signature buildings
    }

    protected override void OnUpdate()
    {
    }

    public void UpdateSignatureBuildings()
    {
        var entities = signatureBuildingsQuery.ToEntityArray(Allocator.Temp);
        List<string> signatureBuildingNames = new List<string>();

        // WARNING: The order of entites is not always deterministic, so it might not be the oldest building that gets found first!
        foreach (Entity entity in entities)
        {
            if (EntityManager.HasComponent<PrefabRef>(entity))
            {
                var prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);
                if (m_PrefabSystem.TryGetPrefab(prefabRef, out BuildingPrefab prefab))
                {
                    if (prefab != null)
                    {
                        // If a buildings is in this list, their effects will be removed because they would be double
                        if (signatureBuildingNames.Contains(prefab.name))
                        {
                            // Remove the component
                            if (EntityManager.HasComponent<CityEffectProvider>(entity))
                                EntityManager.RemoveComponent<CityEffectProvider>(entity);
                            if (EntityManager.HasComponent<LocalEffectProvider>(entity))
                                EntityManager.RemoveComponent<LocalEffectProvider>(entity);
                        }
                        else
                        {
                            // Add components, for example if the original building was removed, we need to find another "original" building
                            if (!EntityManager.HasComponent<CityEffectProvider>(entity))
                                EntityManager.AddComponent<CityEffectProvider>(entity);
                            if (!EntityManager.HasComponent<LocalEffectProvider>(entity))
                                EntityManager.AddComponent<LocalEffectProvider>(entity);

                            signatureBuildingNames.Add(prefab.name);
                        }
                    }

                }
            }
        }
    }
}