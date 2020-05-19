using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo CrearHeightmapBasico:
    ///     Unidades Involucradas:
    ///     # Unidad 7 - Tecnicas de Optimizacion - Heightmap
    ///     Crea un terreno en base a una textura de Heightmap.
    ///     Aplica sobre el terreno una textura para dar color (DiffuseMap).
    ///     Se parsea la textura y se crea un VertexBuffer en base las distintas
    ///     alturas de la imagen.
    ///     Ver el ejemplo EjemploSimpleTerrain para aprender como realizar esto mismo
    ///     pero en forma mas simple con la herramienta TgcSimpleTerrain
    ///     Autor: Matias Leone, Leandro Barbagallo
    /// </summary>
    public class HeightMapTextured
    {
        protected Effect effect;
        protected string currentEffect;
        protected string currentHeightmap;
        protected float currentScaleXZ;
        protected float currentScaleY;
        protected string currentTexture;
        protected TgcTexture terrainTexture;
        protected int totalVertices;
        protected VertexBuffer vbTerrain;
        protected TGCVector3 centre;
        public string name;
        public float time;
        protected TgcSceneLoader.DiffuseMapVertex[] vertices;
        protected int verticesWidth;
        protected int verticesHeight;
        protected TgcMesh mesh;

        public Subnautica GameInstance { get; private set; }

        public HeightMapTextured(Subnautica gameInstance, string name, TGCVector3 centreP, string heightMap, string texture, string effect)
        {
            GameInstance = gameInstance;
            centre = centreP;
            currentHeightmap = heightMap;
            currentTexture = texture;
            currentEffect = effect;
        }

        public HeightMapTextured(Subnautica gameInstance, string name, string heightMap, string texture, string effect)
            :this(gameInstance, name, TGCVector3.Empty, heightMap, texture, effect) { }

        public virtual void Init()
        {

            time = 0;
            //Modifiers para variar escala del mapa
            currentScaleXZ = 1000f;
            currentScaleY = 25f;
            createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

            if (currentEffect != null)
            {
                effect = TGCShaders.Instance.LoadEffect(currentEffect);
                //mesh.Effect = effect;
                //mesh.Technique = "Default";
            }
            // agrego la textura al mesh
            mesh.DiffuseMaps = new[] { terrainTexture.Clone() };
            mesh.Materials = new[] { D3DDevice.DEFAULT_MATERIAL };
            mesh.createBoundingBox();
            mesh.Enabled = true;
        }

        public void Update()
        {
            if (mesh.Effect != null)
            {
                time += GameInstance.ElapsedTime;
                //mesh.Effect.SetValue("time", time);
            }
        }

        /// <summary>
        ///     Crea y carga el VertexBuffer en base a una textura de Heightmap
        /// </summary>
        private void createHeightMapMesh(Device d3dDevice, string path, float scaleXZ, float scaleY)
        {
            //parsear bitmap y cargar matriz de alturas
            var heightmap = loadHeightMap(path);

            //consigo el total de vertices
            totalVertices = 2 * 3 * (heightmap.GetLength(0) - 1) * (heightmap.GetLength(1) - 1);

            //Crear array temporal de vertices
            var dataIdx = 0;
            vertices = new TgcSceneLoader.DiffuseMapVertex[totalVertices];

            // Ancho (x) y alto (z) total del heightmap (imagen)
            verticesWidth = heightmap.GetLength(0);
            verticesHeight = heightmap.GetLength(1);

            // Vector para mover los vertices respecto del centro
            var vectorCenter = centre - new TGCVector3(verticesWidth / 2 * scaleXZ, 0 , verticesHeight / 2 * scaleXZ);

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < verticesWidth - 1; i++)
            {
                for (var j = 0; j < verticesHeight - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente y los desplazo para que el centro del hm este en el pasado por parametro
                    var v1 = new TGCVector3(i * scaleXZ, heightmap[i, j] * scaleY, j * scaleXZ) + vectorCenter;
                    var v2 = new TGCVector3(i * scaleXZ, heightmap[i, j + 1] * scaleY, (j + 1) * scaleXZ) + vectorCenter;
                    var v3 = new TGCVector3((i + 1) * scaleXZ, heightmap[i + 1, j] * scaleY, j * scaleXZ) + vectorCenter;
                    var v4 = new TGCVector3((i + 1) * scaleXZ, heightmap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ) + vectorCenter;

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new TGCVector2(i / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t2 = new TGCVector2(i / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));
                    var t3 = new TGCVector2((i + 1) / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t4 = new TGCVector2((i + 1) / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));

                    // ACA METO LAS NORMALES

                    //Cargar triangulo 1
                    TGCVector3 n1 = TGCVector3.Cross(v2 - v1, v3 - v1);
                    n1.Normalize();
                    vertices[dataIdx]     = crearDiffuseMapVertex(v1, n1, t1);
                    vertices[dataIdx + 1] = crearDiffuseMapVertex(v2, n1, t2);
                    vertices[dataIdx + 2] = crearDiffuseMapVertex(v4, n1, t4);

                    //Cargar triangulo 2
                    TGCVector3 n2 = TGCVector3.Cross(v4 - v1, v3 - v1);
                    n2.Normalize();
                    vertices[dataIdx + 3] = crearDiffuseMapVertex(v1, n2, t1);
                    vertices[dataIdx + 4] = crearDiffuseMapVertex(v4, n2, t4);
                    vertices[dataIdx + 5] = crearDiffuseMapVertex(v3, n2, t3);

                    dataIdx += 6;
                }
            }


            //Llenar todo el VertexBuffer con el array temporal
            Mesh tempMesh = new Mesh(totalVertices / 3, totalVertices, MeshFlags.Managed, TgcSceneLoader.DiffuseMapVertexElements, d3dDevice);

            tempMesh.SetVertexBufferData(vertices, LockFlags.None);
            
            mesh = new TgcMesh(tempMesh, "heightmap", TgcMesh.MeshRenderType.DIFFUSE_MAP);
        }

        protected TgcSceneLoader.DiffuseMapVertex crearDiffuseMapVertex(TGCVector3 position, TGCVector3 normal, TGCVector2 texture)
        {
            TgcSceneLoader.DiffuseMapVertex v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = position;
            v.Normal = normal;
            v.Color = 1;
            v.Tu = texture.X;
            v.Tv = texture.Y;

            return v;
        }

        /// <summary>
        ///     Cargar textura
        /// </summary>
        private void loadTerrainTexture(Device d3dDevice, string path)
        {
            terrainTexture = TgcTexture.createTexture(path);
        }

        /// <summary>
        ///     Cargar Bitmap y obtener el valor en escala de gris de Y
        ///     para cada coordenada (x,z)
        /// </summary>
        private int[,] loadHeightMap(string path)  // ACA SE PUEDE HACER PARA AUTOGENERARLO MAS ADELANTE
        {
            //Cargar bitmap desde el FileSystem
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width;
            var height = bitmap.Size.Height;
            var heightmap = new int[width, height];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    //Obtener color
                    //(j, i) invertido para primero barrer filas y despues columnas
                    var pixel = bitmap.GetPixel(j, i);
                    

                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }
            }

            return heightmap;
        }

        /* TODO: PARA PODER GENERAR "ALEATORIO"
         * float[,] loadHeightMap()
            {
                float[,] heights = new float[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        heights[x, y] = calculateHeight(x, y);
                    }
                }
                return heights;
            }

            // Me devuelve la altura dependiendo de la coordenada y el perlinNoise
            float calculateHeight(int i, int j)
            {
                float iCoord = (float)i / width * currentScaleXZ;
                float jCoord = (float)j / height * currentScaleXZ;

                return Math.PerlinNoise(iCoord, jCoord);
            }
            */

        public void Render()
        {
            mesh.Render();
            mesh.BoundingBox.Render();

            /*
            var device = D3DDevice.Instance.Device;
            
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format; // PositionNormalTextured
            device.SetStreamSource(0, mesh, 0);
            //Render terrain

            if (effect != null && !effect.Disposed)
            {
                // Habilito el canal alpha
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;

                TGCShaders.Instance.SetShaderMatrixIdentity(effect);
                var numPasses = effect.Begin(0);
                for (var n = 0; n < numPasses; n++)
                {
                    effect.BeginPass(n);
                    D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
                    effect.EndPass();
                }
                effect.End();

                device.RenderState.AlphaTestEnable = false;
                device.RenderState.AlphaBlendEnable = false;

            } else
            {
                device.SetTexture(0, terrainTexture.D3dTexture);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
            }
            */
        }

        public void Dispose()
        {
            mesh.Dispose();
        }
    }
}
 