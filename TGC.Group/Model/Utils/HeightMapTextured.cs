using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Core.Shaders;
using TGC.Core.Textures;

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
        protected CustomVertex.PositionNormalTextured[] vertices;
        protected int verticesWidth;
        protected int verticesHeight;
        protected int[,] heightmapData;

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
            currentScaleY = 20f;
            createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);


            if (currentEffect != null)
            {
                effect = TGCShaders.Instance.LoadEffect(currentEffect);
                effect.Technique = "Default";
                effect.SetValue("textureExample", terrainTexture.D3dTexture);
            }

        }

        public void Update()
        {
            if (effect != null)
            {
                time += GameInstance.ElapsedTime;
                effect.SetValue("time", time);
            }
        }

        /// <summary>
        ///     Crea y carga el VertexBuffer en base a una textura de Heightmap
        /// </summary>
        private void createHeightMapMesh(Device d3dDevice, string path, float scaleXZ, float scaleY)
        {
            //parsear bitmap y cargar matriz de alturas
            heightmapData = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmapData.GetLength(0) - 1) * (heightmapData.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            vertices = new CustomVertex.PositionNormalTextured[totalVertices];

            // Ancho (x) y alto (z) total del heightmap (imagen)
            verticesWidth = heightmapData.GetLength(0);
            verticesHeight = heightmapData.GetLength(1);

            // Vector para mover los vertices respecto del centro
            var vectorCenter = centre - new TGCVector3(verticesWidth / 2 * scaleXZ, 0 , verticesHeight / 2 * scaleXZ);

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < verticesWidth - 1; i++)
            {
                for (var j = 0; j < verticesHeight - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente y los desplazo para que el centro del hm este en el pasado por parametro
                    var v1 = new TGCVector3(i * scaleXZ, heightmapData[i, j] * scaleY, j * scaleXZ) + vectorCenter;
                    var v2 = new TGCVector3(i * scaleXZ, heightmapData[i, j + 1] * scaleY, (j + 1) * scaleXZ) + vectorCenter;
                    var v3 = new TGCVector3((i + 1) * scaleXZ, heightmapData[i + 1, j] * scaleY, j * scaleXZ) + vectorCenter;
                    var v4 = new TGCVector3((i + 1) * scaleXZ, heightmapData[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ) + vectorCenter;

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new TGCVector2(i / (float)heightmapData.GetLength(0), j / (float)heightmapData.GetLength(1));
                    var t2 = new TGCVector2(i / (float)heightmapData.GetLength(0), (j + 1) / (float)heightmapData.GetLength(1));
                    var t3 = new TGCVector2((i + 1) / (float)heightmapData.GetLength(0), j / (float)heightmapData.GetLength(1));
                    var t4 = new TGCVector2((i + 1) / (float)heightmapData.GetLength(0), (j + 1) / (float)heightmapData.GetLength(1));

                    // ACA METO LAS NORMALES

                    //Cargar triangulo 1
                    TGCVector3 n1 = TGCVector3.Cross(v2 - v1, v3 - v1);
                    n1.Normalize();
                    vertices[dataIdx] = new CustomVertex.PositionNormalTextured(v1, n1, t1.X, t1.Y);
                    vertices[dataIdx + 1] = new CustomVertex.PositionNormalTextured(v2, n1, t2.X, t2.Y);
                    vertices[dataIdx + 2] = new CustomVertex.PositionNormalTextured(v4, n1, t4.X, t4.Y);

                    //Cargar triangulo 2
                    TGCVector3 n2 = TGCVector3.Cross(v4 - v1, v3 - v1);
                    n2.Normalize();
                    vertices[dataIdx + 3] = new CustomVertex.PositionNormalTextured(v1, n2, t1.X, t1.Y);
                    vertices[dataIdx + 4] = new CustomVertex.PositionNormalTextured(v4, n2, t4.X, t4.Y);
                    vertices[dataIdx + 5] = new CustomVertex.PositionNormalTextured(v3, n2, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            //Llenar todo el VertexBuffer con el array temporal
            vbTerrain.SetData(vertices, 0, LockFlags.None);
        }

        /// <summary>
        ///     Cargar textura
        /// </summary>
        private void loadTerrainTexture(Device d3dDevice, string path)
        {
            terrainTexture = TgcTexture.createTexture(path);
            
            
            //Rotar e invertir textura
            //var b = (Bitmap)Image.FromFile(path);
            //b.RotateFlip(RotateFlipType.Rotate90FlipX);
            //terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
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

        public float CalcularAltura(float x, float z)
        {
            var largo = currentScaleXZ * 64;  // 64 es el ancho del bitmap
            var pos_i = 64f * (0.5f + x / largo);
            var pos_j = 64f * (0.5f + z / largo);

            var pi = (int)pos_i;
            var fracc_i = pos_i - pi;
            var pj = (int)pos_j;
            var fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else if (pi > 63)
                pi = 63;

            if (pj < 0)
                pj = 0;
            else if (pj > 63)
                pj = 63;

            var pi1 = pi + 1;
            var pj1 = pj + 1;
            if (pi1 > 63)
                pi1 = 63;
            if (pj1 > 63)
                pj1 = 63;

            // 2x2 percent closest filtering usual:
            var H0 = heightmapData[pi, pj] * currentScaleY;
            var H1 = heightmapData[pi1, pj] * currentScaleY;
            var H2 = heightmapData[pi, pj1] * currentScaleY;
            var H3 = heightmapData[pi1, pj1] * currentScaleY;
            var H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) + (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;
            // H tiene la altura en espacio de mesh
            return H + centre.Y;
        }

        public void Render()
        {
            var device = D3DDevice.Instance.Device;
            
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format; // PositionNormalTextured
            device.SetStreamSource(0, vbTerrain, 0);
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
        }

        public void Dispose()
        {
            vbTerrain.Dispose();
            terrainTexture.D3dTexture.Dispose();
            if (effect != null)
                effect.Dispose();
        }
    }
}
 