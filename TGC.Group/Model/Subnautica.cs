using System;
using System.Collections.Generic;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model.Utils;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Terrain;
using TGC.Core.Text;
using System.Drawing;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Group.Model.Items;
using TGC.Core.BoundingVolumes;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using TGC.Group.Model.Menus.PauseMenu;
using TGC.Core.Fog;
using TGC.Core.Geometry;
using TGC.Core.Sound;
using TGC.Core.Particle;
using TGC.Group.Model.Menus.PrincipalMenu;

namespace TGC.Group.Model
{
    public class Subnautica : TGCExample
    {
        #region SOUNDS
        private List<TgcStaticSound> sounds = new List<TgcStaticSound>();

        public TgcStaticSound OnHitPlayerSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound OutOfOxygenSound { get; private set; } = new TgcStaticSound();
        public List<TgcStaticSound> OnHitNpcSounds { get; private set; } = new List<TgcStaticSound>();
        public List<TgcStaticSound> CraftingSounds { get; private set; } = new List<TgcStaticSound>();
        public TgcStaticSound OpenCraftingMenu { get; private set; } = new TgcStaticSound();
        public TgcStaticSound EatingSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound EquipItemSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound UnderwaterAmbience { get; private set; } = new TgcStaticSound();
        public TgcStaticSound SurfaceAmbience { get; private set; } = new TgcStaticSound();
        public TgcStaticSound CraftingFailSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound BreathingSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound BoublesSound { get; private set; } = new TgcStaticSound();
        public TgcStaticSound CoralpickupSound { get; private set; } = new TgcStaticSound();
        #endregion

        #region MESHES
        private List<TgcMesh> playerMeshes;
        private List<TgcMesh> shipMeshes;
        #endregion

        #region RENDER
        private Effect gogleViewEffect;
        private Texture gogleViewTexture;
        private VertexBuffer fullQuadVertexBuffer;
        private Texture auxRenderTarget;
        private Surface auxDepthStencil;
        #endregion

        #region SETTINGS

        private TGCBox lightBox;
        private TGCVector3 skyBoxDimensions = new TGCVector3(65000, 20000, 65000);
        public SueloDelMar SueloDelMar { get; private set; }
        public List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();
        private TgcSkyBox skyBox;
        private TgcFog fog;
        public QuadTree ScenesQuadTree = new QuadTree();
        private List<GameObject> removedObjects = new List<GameObject>();
        private float time = 0f;
        private readonly float waterY = 0f;
        private readonly float floorY = -7000;
        private float escapeDelay = 0;
        private bool playerWasSubmerged = false;
        private ParticleEmitter bubbleParticleEmitter;
        private float timeSinceLastBubblesReposition = 0f;
        private float bubblesRepositionCooldown = 2f;

        public bool MouseEnabled { get; private set; } = false;
        public bool FocusInGame { get; private set; } = true; // Variable para saber si estoy jugando o en menu
        public TGCVector3 LightPosition { get; private set; }
        #endregion

        #region OBJECTS
        public List<StaticObject> StaticSceneObjects { get; private set; } = new List<StaticObject>();
        public List<GameObject> NonStaticSceneObjects { get; private set; } = new List<GameObject>();
        public List<GameObject> SceneObjects => StaticSceneObjects.Concat(NonStaticSceneObjects).ToList();
        public Player Player { get; private set; }
        public Ship Ship { get; private set; }

        private SpawnManager spawnManager;
        #endregion

        #region HUD

        private TgcText2D healthTextBox = new TgcText2D();
        private TgcText2D oxygenTextBox = new TgcText2D();
        private bool showCrosshair = true;
        private TgcText2D crossHair = new TgcText2D();
        private TgcText2D gameTimer = new TgcText2D();

        private PauseMenu pauseMenu;
        private PrincipalMenu principalMenu;
        public bool InPrincipalMenu { get; private set; }

        #endregion


        public Subnautica(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        #region TGC_EXAMPLE

        public override void Init()
        {
            

            LightPosition = new TGCVector3(0,80000, -3 * skyBoxDimensions.X);
            lightBox = TGCBox.fromSize(TGCVector3.One * 500, Color.Red);
            lightBox.Transform = TGCMatrix.Translation(LightPosition);

            InitFog();
            InitMainMeshes();
            InitHUD();
            LoadMainScene();
            ManageFocus();
            spawnManager = new SpawnManager(this);
            SetCamera();

            //DirectSound.ListenerTracking = Player.Meshes[0]; // Alguno de los meshes del player, cumplen con la interfaz ITransformObject

            LightPosition = new TGCVector3(0, 8000, -3 * heightMaps[0].XZRadius);
            lightBox = TGCBox.fromSize(TGCVector3.One * 500, Color.Red);
            lightBox.Transform = TGCMatrix.Translation(LightPosition);

            ScenesQuadTree.create(StaticSceneObjects, new TgcBoundingAxisAlignBox(SueloDelMar.centre - new TGCVector3(SueloDelMar.XZRadius, 3000, SueloDelMar.XZRadius), SueloDelMar.centre + new TGCVector3(SueloDelMar.XZRadius, 5000, SueloDelMar.XZRadius)));
            ScenesQuadTree.createDebugQuadTreeMeshes();

            InitFullQuadVB();
            InitAuxRenderTarget();
            InitGogleViewEffectResources();
            InitSounds();
            InitBubbleEmitter();

            InPrincipalMenu = true;
            FocusInGame = false;
            MouseEnable();
        }

        public override void Update()
        {
            PreUpdate();
            escapeDelay += ElapsedTime;

            if (!InPrincipalMenu && Input.keyDown(Key.Escape) && escapeDelay > 0.5f) { // uso el delay porque no me funciona el keyUp o keyPressed
                escapeDelay = 0;
                FocusInGame = !FocusInGame;
                ManageFocus();
            }

            if (FocusInGame || InPrincipalMenu)    // Si no se está en modo gameplay, desactivar el update de todo
            {
                principalMenu.Update(ElapsedTime);

                UpdateInstantiatedObjects();
                spawnManager.Update();


                // Actualizo el frustum para que solo tome hasta la fog distance asi no manda a renderizar items del quadtree que estén por detras
                var projectionMatrixFog = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio, D3DDevice.Instance.ZNearPlaneDistance, FastMath.Abs(fog.EndDistance));
                Frustum.updateVolume(TGCMatrix.FromMatrix(D3DDevice.Instance.Device.Transform.View), projectionMatrixFog);
                ScenesQuadTree.UpdateVisibleObjects(Frustum);


                // Todos los objetos (estaticos y no estaticos)
                foreach (GameObject o in SceneObjects)
                    o.Update();

                // HeightMaps
                foreach (HeightMapTextured hm in heightMaps)
                    hm.Update();

                PlayAmbienceSound();

                // Muevo el centro del skybox para que sea inalcanzable
                skyBox.Center = new TGCVector3(Camera.Position.X, 0, Camera.Position.Z);


            }

            UpdateHUD();
            UpdateParticleEmitter();

            time += ElapsedTime;


            PostUpdate();
        }

        public override void Render()
        {
            //PreRender();
            ClearTextures();
            var screenRenderTargetSurface = D3DDevice.Instance.Device.GetRenderTarget(0);
            var screenDepthStencil = D3DDevice.Instance.Device.DepthStencilSurface;

            var surf = auxRenderTarget.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, surf);
            D3DDevice.Instance.Device.DepthStencilSurface = auxDepthStencil;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, fog.Color, 1.0f, 0);

            RenderMainScene();


            surf.Dispose();
            // Para guardar una imagen
            //TextureLoader.Save(ShadersDir + "main_scene.bmp", ImageFileFormat.Bmp, auxRenderTarget);

            D3DDevice.Instance.Device.SetRenderTarget(0, screenRenderTargetSurface);
            D3DDevice.Instance.Device.DepthStencilSurface = screenDepthStencil;

            RenderPostProcess();
            lightBox.Render();

            D3DDevice.Instance.Device.Present();
        }

        public override void Dispose()
        {
            DisposeHUD();
            spawnManager.Dispose();

            foreach (GameObject o in SceneObjects)
                o.Dispose();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Dispose();

            lightBox.Dispose();

            ItemDatabase.Instance.Dispose();
            skyBox.Dispose();

            fullQuadVertexBuffer.Dispose();
            auxRenderTarget.Dispose();
            auxDepthStencil.Dispose();
            gogleViewEffect.Dispose();
            gogleViewTexture.Dispose();
            DisposeSounds();
        }

        #endregion

        #region INTERFACE

        public void InstanceObject(GameObject obj)
        {
            NonStaticSceneObjects.Add(obj);
        }

        public void DestroyObject(GameObject obj)
        {
            removedObjects.Add(obj);
        }

        public void InstanceStaticSceneObject(StaticObject obj)
        {
            StaticSceneObjects.Add(obj);
        }

        // heightmaps: 0 = piso, 1 = agua
        public float WaterLevelToWorldHeight(float waterLevel) => heightMaps[1].CalcularAltura(Player.Position.X, Player.Position.Z) + waterLevel;

        public float FloorLevelToWorldHeight(float floorLevel) => heightMaps[0].CalcularAltura(Player.Position.X, Player.Position.Z) + floorLevel;

        public void MouseDisable()
        {
            MouseEnabled = false;
            int deviceWidth = D3DDevice.Instance.Width;
            int deviceHeight = D3DDevice.Instance.Height;
            Cursor.Clip = new Rectangle(deviceWidth / 2, deviceHeight / 2, 1, 1); // El cursor se queda quieto en un punto y permite que se pueda mover la camara infinitamente
            Cursor.Hide();

            showCrosshair = true;
        }

        public void MouseEnable()
        {
            MouseEnabled = true;
            Cursor.Clip = new Rectangle(); // libero el mouse
            Cursor.Show();

            showCrosshair = false;
        }

        #endregion

        #region PRIVATE_METHODS

        private void ManageFocus()
        {
            if (FocusInGame)
            {
                MouseDisable();
            }
            else
            {
                Player.CloseInventory();
                Ship.CloseCraftingMenu();
                MouseEnable();
            }
        }

        private void SetCamera()
        {
            // Cambio el farPlane
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 10f).ToMatrix();

            Camera = new FPSCamera(this, Player, Player.RelativeEyePosition);
        }

        private void LoadMainScene()
        {
            // Genero el terreno

            LoadSkybox();

            Player = new Player(this, "player", playerMeshes);
            InstanceObject(Player);

            Ship = new Ship(this, "main_ship", shipMeshes);
            InstanceObject(Ship);

            LoadTerrain();
        }

        private void LoadTerrain()
        {
            SueloDelMar = new SueloDelMar(this, "SeaFloor", new TGCVector3(0, floorY, 0), MediaDir + "Terrain\\" + "HMFondo-x128.jpg", MediaDir + "Terrain\\" + "sand.jpg", ShadersDir + "SeaFloorShader.fx", 500f, 20f);
            heightMaps.Add(SueloDelMar);
            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, waterY, 0), MediaDir + "Terrain\\" + "HeightMapPlanox1024.jpg", MediaDir + "Skybox\\down.jpg", ShadersDir + "WaterShader.fx", 62.5f, 1f));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
            }
        }

        private void InitHUD()
        {
            int deviceWidth = D3DDevice.Instance.Width;
            int deviceHeight = D3DDevice.Instance.Height;

            healthTextBox.Text = "✚ 100";
            healthTextBox.Color = Color.DarkOrange;
            healthTextBox.Position = new Point((int)FastMath.Floor(0.05f * deviceWidth), (int)FastMath.Floor(0.9f * deviceHeight));
            healthTextBox.Size = new Size(600, 200);
            healthTextBox.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
            healthTextBox.Align = TgcText2D.TextAlign.LEFT;

            oxygenTextBox.Text = "◴ 100";
            oxygenTextBox.Color = Color.DarkOrange;
            oxygenTextBox.Position = new Point((int)FastMath.Floor(0.12f * deviceWidth), (int)FastMath.Floor(0.9f * deviceHeight));
            oxygenTextBox.Size = new Size(600, 200);
            oxygenTextBox.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
            oxygenTextBox.Align = TgcText2D.TextAlign.LEFT;

            crossHair.Text = "+";
            crossHair.Color = Color.White;
            crossHair.Position = new Point(deviceWidth / 2 - 8, deviceHeight / 2 - 40);
            crossHair.Size = new Size(600, 200);
            crossHair.changeFont(new Font("TimesNewRoman", 25, FontStyle.Regular));
            crossHair.Align = TgcText2D.TextAlign.LEFT;

            gameTimer.Text = "Time: ";
            gameTimer.Color = Color.DarkOrange;
            gameTimer.Position = new Point((int)FastMath.Floor(0.05f * deviceWidth), (int)FastMath.Floor(0.85f * deviceHeight));
            gameTimer.Size = new Size(300, 600);
            gameTimer.changeFont(new Font("TimesNewRoman", 12, FontStyle.Regular));
            gameTimer.Align = TgcText2D.TextAlign.LEFT;

            pauseMenu = new PauseMenu(this);
            principalMenu = new PrincipalMenu(this);
        }

        private void UpdateHUD()
        {
            healthTextBox.Text = "✚ " + Player.Health;
            oxygenTextBox.Text = "◴ " + Player.Oxygen;
            gameTimer.Text = "Time: " + FastMath.Floor(time) + " sec";
        }

        private void RenderHUD()
        {
            principalMenu.Render();
            if (InPrincipalMenu)
                return;
            healthTextBox.render();
            oxygenTextBox.render();
            pauseMenu.Render();

            if (showCrosshair)
                crossHair.render();

            gameTimer.render();
        }

        private void DisposeHUD()
        {
            pauseMenu.Dispose();
            principalMenu.Dispose();
            healthTextBox.Dispose();
            oxygenTextBox.Dispose();
            gameTimer.Dispose();
        }

        private void InitMainMeshes()
        {
            var loader = new TgcSceneLoader();

            playerMeshes = loader.loadSceneFromFile(MediaDir + "Player\\Player-TgcScene.xml").Meshes;
            shipMeshes = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\ship-TgcScene.xml").Meshes;
        }

        private void LoadSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Up * (skyBoxDimensions.Y / 11f);
            skyBox.Size = new TGCVector3(skyBoxDimensions);

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, MediaDir + "Skybox\\up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, MediaDir + "Skybox\\down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, MediaDir + "Skybox\\left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, MediaDir + "Skybox\\right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, MediaDir + "Skybox\\front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, MediaDir + "Skybox\\back.jpg");

            skyBox.Init();

            var effect = TGCShaders.Instance.LoadEffect(ShadersDir + "SkyboxShader.fx");
            effect.SetValue("ColorFog", fog.Color.ToArgb());
            effect.SetValue("WaterLevel", waterY);

            foreach (var face in skyBox.Faces)
            {
               
                face.Effect = effect;
                face.Technique = "Default";
            }

            skyBox.AlphaBlendEnable = true;
        }

        private void UpdateInstantiatedObjects()
        {
            StaticSceneObjects.RemoveAll(obj => removedObjects.Contains(obj));
            NonStaticSceneObjects.RemoveAll(obj => removedObjects.Contains(obj));
            removedObjects.Clear();
        }

        private void InitFullQuadVB()
        {
            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };

            fullQuadVertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            fullQuadVertexBuffer.SetData(vertices, 0, LockFlags.None);
        }

        private void InitAuxRenderTarget()
        {
            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            auxRenderTarget = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            //Creamos un DepthStencil que debe ser compatible con nuestra definicion de renderTarget2D.
            auxDepthStencil = D3DDevice.Instance.Device.CreateDepthStencilSurface(D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);
        }

        private void RenderMainScene()
        {
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();

            skyBox.Render();

            foreach (GameObject o in SceneObjects)
                o.Render();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Render();

            //ScenesQuadTree.RenderDebugBoxes();

            RenderBubbles();

            D3DDevice.Instance.Device.EndScene();
        }

        private void RenderPostProcess()
        {
            D3DDevice.Instance.Device.BeginScene();

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, fullQuadVertexBuffer, 0);

            gogleViewEffect.SetValue("mainSceneTexture", auxRenderTarget);

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, fog.Color, 1.0f, 0);

            if (Player.IsSubmerged)
            {
                gogleViewEffect.Technique = "GogleView";
            }
            else
            {
                gogleViewEffect.Technique = "NoGogles";
            }

            gogleViewEffect.Begin(FX.None);
            gogleViewEffect.BeginPass(0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            gogleViewEffect.EndPass();
            gogleViewEffect.End();

            
            RenderHUD();

            RenderFPS();
            RenderAxis();

            D3DDevice.Instance.Device.EndScene();
        }

        private void InitGogleViewEffectResources()
        {
            gogleViewEffect = TGCShaders.Instance.LoadEffect(ShadersDir + "Varios.fx");

            gogleViewTexture = TgcTexture.createTexture(MediaDir + "gogleView.png").D3dTexture;
            gogleViewEffect.SetValue("gogleViewTexture", gogleViewTexture);
        }

        private void InitFog()
        {
            fog = new TgcFog();
            fog.StartDistance = 3500f;
            fog.EndDistance = 10000f;
            fog.Color = Color.FromArgb(255, 11, 36, 74);
        }

        private void InitSounds()
        {
            OnHitPlayerSound.loadSound(MediaDir + "//Sounds//GolpeAPlayer.wav", DirectSound.DsDevice);
            sounds.Add(OnHitPlayerSound);

            OutOfOxygenSound.loadSound(MediaDir + "//Sounds//SinOxigeno.wav", DirectSound.DsDevice);
            sounds.Add(OutOfOxygenSound);

            OpenCraftingMenu.loadSound(MediaDir + "//Sounds//AbrirMenuCrafteo.wav", DirectSound.DsDevice);
            sounds.Add(OpenCraftingMenu);

            EatingSound.loadSound(MediaDir + "//Sounds//Comer.wav", DirectSound.DsDevice);
            sounds.Add(EatingSound);

            EquipItemSound.loadSound(MediaDir + "//Sounds//EquiparItem.wav", DirectSound.DsDevice);
            sounds.Add(EquipItemSound);

            UnderwaterAmbience.loadSound(MediaDir + "//Sounds//AmbienteBajoElAgua.wav", DirectSound.DsDevice);
            sounds.Add(UnderwaterAmbience);

            SurfaceAmbience.loadSound(MediaDir + "//Sounds//AmbienteFueraDelAgua.wav", DirectSound.DsDevice);
            sounds.Add(SurfaceAmbience);

            CraftingFailSound.loadSound(MediaDir + "//Sounds//ErrorConstruccion.wav", DirectSound.DsDevice);
            sounds.Add(CraftingFailSound);

            BreathingSound.loadSound(MediaDir + "//Sounds//RespiracionProfunda.wav", DirectSound.DsDevice);
            sounds.Add(BreathingSound);

            BoublesSound.loadSound(MediaDir + "//Sounds//Burbujas.wav", DirectSound.DsDevice);
            sounds.Add(BoublesSound);

            CoralpickupSound.loadSound(MediaDir + "//Sounds//RecolectarCoral.wav", DirectSound.DsDevice);
            sounds.Add(CoralpickupSound);


            TgcStaticSound hit1 = new TgcStaticSound();
            hit1.loadSound(MediaDir + "//Sounds//Golpe1.wav", DirectSound.DsDevice);

            TgcStaticSound hit2 = new TgcStaticSound();
            hit2.loadSound(MediaDir + "//Sounds//Golpe2.wav", DirectSound.DsDevice);

            OnHitNpcSounds.Add(hit1);
            OnHitNpcSounds.Add(hit2);

            sounds.AddRange(OnHitNpcSounds);


            TgcStaticSound crafting1 = new TgcStaticSound();
            crafting1.loadSound(MediaDir + "//Sounds//Construir1.wav", DirectSound.DsDevice);

            TgcStaticSound crafting2 = new TgcStaticSound();
            crafting2.loadSound(MediaDir + "//Sounds//Construir2.wav", DirectSound.DsDevice);

            CraftingSounds.Add(crafting1);
            CraftingSounds.Add(crafting2);

            sounds.AddRange(CraftingSounds);
        }

        private void DisposeSounds()
        {
            sounds.ForEach(s => s.dispose());
        }

        private void PlayAmbienceSound()
        {
            if (Player.IsSubmerged)
            {
                playerWasSubmerged = true;
                SurfaceAmbience.stop();
                BreathingSound.stop();
                UnderwaterAmbience.play(true);
            }
            else
            {
                UnderwaterAmbience.stop();
                SurfaceAmbience.play(true);

                if (playerWasSubmerged)
                {
                    BreathingSound.play();
                    playerWasSubmerged = false;
                }
            }
        }

        private void UpdateParticleEmitter()
        {
            timeSinceLastBubblesReposition += ElapsedTime;

            if (timeSinceLastBubblesReposition >= bubblesRepositionCooldown)
            {
                int relativeXPosition = MathExtended.GetRandomNumberBetween(-600, 600);
                int relativeYPosition = MathExtended.GetRandomNumberBetween(-100, 300);
                int relativeZPosition = MathExtended.GetRandomNumberBetween(700, 2000);

                TGCVector3 particlesRelativePosition = Player.RelativeRightDirection * relativeXPosition + Player.RelativeUpDirection * relativeYPosition + Player.LookDirection * relativeZPosition;
                bubbleParticleEmitter.Position = Player.Position + particlesRelativePosition;

                timeSinceLastBubblesReposition = 0f;
            }
        }

        private void RenderBubbles()
        {
            if(Player.IsSubmerged)
                bubbleParticleEmitter.render(ElapsedTime);
        }

        private void InitBubbleEmitter()
        {
            bubbleParticleEmitter = new ParticleEmitter(MediaDir + "//Sprites//burbujas.png", 10);
            bubbleParticleEmitter.MinSizeParticle = 1;
            bubbleParticleEmitter.MaxSizeParticle = 5;
            bubbleParticleEmitter.ParticleTimeToLive = 3;
            bubbleParticleEmitter.CreationFrecuency = 0.5f;
            bubbleParticleEmitter.Dispersion = 10;
            bubbleParticleEmitter.Speed = TGCVector3.One * 5;
        }
        #endregion

        #region PUBLIC_METHODS

        public void loadEffectWithFogValues(Effect effect)
        {
            effect.SetValue("ColorFog", fog.Color.ToArgb());
            //effect.SetValue("CameraPos", TGCVector3.TGCVector3ToFloat4Array(Camera.Position));
            effect.SetValue("StartFogDistance", fog.StartDistance);
            effect.SetValue("EndFogDistance", fog.EndDistance);
        }

        public bool startGame()
        {
            InPrincipalMenu = false;
            FocusInGame = true;
            ManageFocus();
            return true;
        }
        #endregion
    }
}
