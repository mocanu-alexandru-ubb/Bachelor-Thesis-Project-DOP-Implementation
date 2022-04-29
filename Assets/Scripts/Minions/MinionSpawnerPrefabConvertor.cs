using System;
using Unity.Entities;
using UnityEngine;

public class MinionSpawnerPrefabConvertor : MonoBehaviour, IConvertGameObjectToEntity
{
    public static Entity minionToSpawn;
    public static Entity bulletToSpawn;
    public static Entity bulletDesctructionToSpawn;

    [SerializeField]
    private GameObject minionPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject bulletDestructionPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using (BlobAssetStore blobAssetStore = new BlobAssetStore())
        {
            minionToSpawn = GameObjectConversionUtility.ConvertGameObjectHierarchy(minionPrefab, 
                GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
            bulletToSpawn = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, 
                GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
            bulletDesctructionToSpawn = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletDestructionPrefab, 
                GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
        }
    }
}
