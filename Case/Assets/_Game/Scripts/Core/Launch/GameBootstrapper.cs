using _Game.Scripts.Core.UI;
using Core.Camera;
using Core.Data;
using Core.Dice;
using Core.Grid;
using Core.Inventory;
using Core.Particles;
using Core.Pool;
using Core.Sound;
using Core.Tokens;
using Save;
using Service;
using UnityEngine;

namespace Core.Launch
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Map Configuration")]
        [SerializeField] private MapData currentMapData;
        [SerializeField] private TileTypeRegistry tileTypeRegistry;

        [Header("Save Configuration")]
        [SerializeField] private SaveData saveData;

        [Header("Particle Configuration")]
        [SerializeField] private ParticleData particleData;

        [Header("Sound Configuration")]
        [SerializeField] private SoundData soundData;

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
        private IParticleService _particleService;
        private IPoolService _poolService;
        private ISaveService _saveService;
        private ISoundService _soundService;
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
            _saveService = new SaveService();
            ServiceLocator.Register(_saveService);

            _poolService = new PoolService();
            ServiceLocator.Register(_poolService);
            
            _gridService = new GridService();
            ServiceLocator.Register(_gridService);

            _inventoryService = new InventoryService(_saveService, saveData);
            ServiceLocator.Register(_inventoryService);

            _particleService = new ParticleService(_poolService, particleData);
            ServiceLocator.Register(_particleService);

            _soundService = new SoundService(soundData);
            ServiceLocator.Register(_soundService);

            _tokenController = tokenController;
            _cameraController = cameraController;
            _diceController = diceController;
        }

        private void StartGame()
        {
            _particleService.Prewarm();
            _gridService.SetTypeRegistry(tileTypeRegistry);
            _gridService.BuildGrid(currentMapData, tileRoot);
            tokenController.Initialize(_cameraController);
            uiController.Init(_tokenController, _diceController);
            inventoryPanel.Initialize(_cameraController);
        }
    }
}