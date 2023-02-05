using System;
using System.Collections.Generic;

using UnityEngine;

public enum EncounterType
{
    None,
    Water,
    Fertilizer,
    Block,
    Time
}

[Serializable]
public class EncounterEventData
{
    public EncounterType Type;
    public uint EffectValue;
}

public sealed class EncounterManager : MonoBehaviour
{
    [SerializeField] private GameObject _tilesRoot;
    [SerializeField] private GameObject _dirtTilePrefab;
    [SerializeField] private Transform _waterDisplay;
    [SerializeField] private Transform _fertilizerDisplay;
    [SerializeField] private Transform _timeDisplay;

    private readonly List<TerrainTile> _existedTiles = new List<TerrainTile>();
    private ResourceTracker _resourceTracker;
    private float _yCameraDelta;
    public event Action OnRootCrash;

    public void PrepareAll
    (
        ResourceTracker resourceTracker, 
        CameraManager cameraManager, 
        LevelMapGenerator levelMapGenerator,
        GamePlayPanel panel
    )
    {
        _resourceTracker = resourceTracker;

        const float INITIAL_Y_OFFSET = -6.56f;
        const float Y_OFFSET_PER_TILE = -10.8f;
        _existedTiles.Add(CreateNewTerrainTile());
        cameraManager.OnPositionYChanged += yChangeAmount =>
        {
            _yCameraDelta += yChangeAmount;
            if (_yCameraDelta + Y_OFFSET_PER_TILE < NextTilePositionY())
            {
                _existedTiles.Add(CreateNewTerrainTile());
            }
        };

        TerrainTile CreateNewTerrainTile()
        {
            var terrainTile = new TerrainTile
            (
                _tilesRoot.transform, _dirtTilePrefab, NextTilePositionY(),
                levelMapGenerator
            );
            terrainTile.Enable(OnTriggerEvent);
            return terrainTile;
        }

        float NextTilePositionY()
        {
            float value = INITIAL_Y_OFFSET + _existedTiles.Count * Y_OFFSET_PER_TILE;
            return value;
        }

        void OnTriggerEvent(EncounterEventData data)
        {
            panel.Collided = true;
            switch (data.Type)
            {
                case EncounterType.Water:
                    _resourceTracker.IncreaseWater(data.EffectValue);
                    break;
                case EncounterType.Fertilizer:
                    _resourceTracker.IncreaseFertilizer(data.EffectValue);
                    break;
                case EncounterType.Block:
                    OnRootCrash?.Invoke();
                    break;
                case EncounterType.Time:
                    _resourceTracker.IncreaseTime(data.EffectValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            panel.Collided = false;
        }
    }

    [Serializable]
    public class EncounterObjectViewData
    {
        public EncounterObject Object;
        public EncounterEventData Data;
    }
}