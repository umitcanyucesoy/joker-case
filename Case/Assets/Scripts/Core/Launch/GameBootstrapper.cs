using Core.Camera;
using Core.Data;
using Core.Dice;
using Core.Grid;
using Core.Inventory;
using Core.Pool;
using Core.Tokens;
using Core.UI;
using Service;
using UnityEngine;

namespace Core.Launch
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Map Configuration")]
        [SerializeField] private MapData currentMapData;
        [SerializeField] private TileTypeRegistry tileTypeRegistry;

        [Header("Controllers")]
        [SerializeField] private TokenController tokenController;
        [SerializeField] private UIController uiController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private DiceController diceController;
        [SerializeField] private InventoryPanel inventoryPanel;
        
        [Header("Roots")]
        [SerializeField] private Transform tileRoot;

        private IGridService _gridService;
        private IInventoryService _inventoryService;
        private IPoolService _poolService;
        private ITokenController _tokenController;
        private ICameraController _cameraController;
        private IDiceController _diceController;

        private void Awake()
        {
            InitializeServices();
        }

        private void Start()
        {
            StartGame();
        }

        private void InitializeServices()
        {
            _poolService = new PoolService();
            ServiceLocator.Register(_poolService);
            
            _gridService = new GridService();
            ServiceLocator.Register(_gridService);

            _inventoryService = new InventoryService();
            ServiceLocator.Register(_inventoryService);

            _tokenController = tokenController;
            _cameraController = cameraController;
            _diceController = diceController;
        }

        private void StartGame()
        {
            _gridService.SetTypeRegistry(tileTypeRegistry);
            _gridService.BuildGrid(currentMapData, tileRoot);
            tokenController.Initialize(_cameraController);
            uiController.Init(_tokenController, _diceController);
            inventoryPanel.Initialize();
        }
    }
}