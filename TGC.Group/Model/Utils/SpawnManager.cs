using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Utils
{
    public class SpawnManager
    {
        private Subnautica gameInstance;
        private List<TgcMesh> sharkMeshes;
        private List<TgcMesh> fishMeshes;

        private readonly int initialFishNumber = 300;
        private int totalFishCounter = 0;

        private readonly float fishSpawnCooldown = 100f;
        private float timeSinceLastFishSpawn = 0f;

        private readonly float sharkSpawnCooldown = 30f;
        private float timeSinceLastSharkSpawn = 0f;

        public SpawnManager(Subnautica gameInstance)
        {
            this.gameInstance = gameInstance;
            TgcSceneLoader loader = new TgcSceneLoader();

            fishMeshes = loader.loadSceneFromFile(gameInstance.MediaDir + "Aquatic\\Meshes\\fish-TgcScene.xml").Meshes;
            sharkMeshes = loader.loadSceneFromFile(gameInstance.MediaDir + "Aquatic\\Meshes\\shark-TgcScene.xml").Meshes;

            for (int i = 0; i < initialFishNumber; i++)
            {
                SpawnFish();
            }
        }

        public void Update()
        {
            timeSinceLastFishSpawn += gameInstance.ElapsedTime;
            timeSinceLastSharkSpawn += gameInstance.ElapsedTime;

            if (timeSinceLastFishSpawn >= fishSpawnCooldown)
                SpawnFish();

            if (timeSinceLastSharkSpawn >= sharkSpawnCooldown)
                SpawnShark();
        }

        public void Dispose()
        {
            fishMeshes.ForEach(mesh => mesh.Dispose());
            sharkMeshes.ForEach(mesh => mesh.Dispose());
        }

        private TGCVector3 RandomSpawnLocation()
        {
            Random random = new Random();
            TGCVector3 spawnLocation = TGCVector3.Empty;
            spawnLocation.X = MathExtended.GetRandomNumberBetween(-gameInstance.SueloDelMar.XZRadius, gameInstance.SueloDelMar.XZRadius);
            spawnLocation.Y = MathExtended.GetRandomNumberBetween(-gameInstance.SueloDelMar.XZRadius, gameInstance.SueloDelMar.XZRadius);
            spawnLocation.Z = MathExtended.GetRandomNumberBetween(-gameInstance.SueloDelMar.XZRadius, gameInstance.SueloDelMar.XZRadius);

            return spawnLocation;
        }

        private TGCVector3 RandomWaterSpawnLocation()
        {
            TGCVector3 spawnLocation = RandomSpawnLocation();
            spawnLocation.Y = MathExtended.GetRandomNumberBetween((int)gameInstance.FloorLevelToWorldHeight(gameInstance.SueloDelMar.YMax), (int)gameInstance.WaterLevelToWorldHeight(-100));
            return spawnLocation;
        }

        private void SpawnFish()
        {
            totalFishCounter++;

            string name = "fish" + totalFishCounter;
            gameInstance.InstanceObject(new Fish(gameInstance, name, InstantiateAllMeshes(fishMeshes, name), RandomWaterSpawnLocation()));
            timeSinceLastFishSpawn = 0f;
        }

        private void SpawnShark()
        {
            gameInstance.InstanceObject(new Shark(gameInstance, "shark", InstantiateAllMeshes(sharkMeshes, "shark"), RandomWaterSpawnLocation()));
            timeSinceLastSharkSpawn = 0f;
        }

        private List<TgcMesh>InstantiateAllMeshes(List<TgcMesh> meshes, string name) => Array.ConvertAll(meshes.ToArray(), mesh => mesh.createMeshInstance(name)).ToList();
    }
}
