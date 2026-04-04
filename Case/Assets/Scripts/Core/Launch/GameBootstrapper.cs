using System;
using Core.Data;
using Core.Grid;
using Service;
using UnityEngine;

namespace Core.Launch
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] private MapData currentMapData;
       
        private IGridService _gridService;

        private void Awake()
        {
            InitializeServices();
            
        }

        private void Start()
        {
            BuildGrid();
        }

        private void InitializeServices()
        {
            _gridService = new GridService();
            ServiceLocator.Register(_gridService);
        }

        private void BuildGrid()
        {
            _gridService = ServiceLocator.Get<IGridService>();
            _gridService.BuildGrid(currentMapData);
        }
    }
}