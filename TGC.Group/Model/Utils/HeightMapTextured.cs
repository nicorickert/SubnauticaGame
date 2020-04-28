using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

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

        private string currentHeightmap;
        private float currentScaleXZ;
        private float currentScaleY;
        private string currentTexture;
        private Texture terrainTexture;
        private int totalVertices;
        private VertexBuffer vbTerrain;
        private TGCVector3 centre;
        public string name;

        public Subnautica GameInstance { get; private set; }

        public HeightMapTextured(Subnautica gameInstance, string name, TGCVector3 centreP, string heightMap, string texture)
        {
            GameInstance = gameInstance;
            centre = centreP;
            currentHeightmap = heightMap;
            currentTexture = texture;
        }

        public HeightMapTextured(Subnautica gameInstance, string name, string heightMap, string texture)
        :this(gameInstance, name, TGCVector3.Empty, heightMap, texture) { }

        public void Init()
        {
            //Modifiers para variar escala del mapa
            currentScaleXZ = 300f;
            currentScaleY = 10f;
            createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

            //UserVars para cantidad de vertices
        }

        public void Update()
        {
        }

        /// <summary>
        ///     Crea y carga el VertexBuffer en base a una textura de Heightmap
        /// </summary>
        private void createHeightMapMesh(Device d3dDevice, string path, float scaleXZ, float scaleY)
        {
            //parsear bitmap y cargar matriz de alturas
            var heightmap = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmap.GetLength(0) - 1) * (heightmap.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];

            // Ancho (x) y alto (z) total del heightmap (imagen)
            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            // Vector para mover los vertices respecto del centro
            var vectorCenter = centre - new TGCVector3(width / 2 * scaleXZ, 0 , height / 2 * scaleXZ);

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < width - 1; i++)
            {
                for (var j = 0; j < height - 1; j++)
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

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            //Llenar todo el VertexBuffer con el array temporal
            vbTerrain.SetData(data, 0, LockFlags.None);
        }

        /// <summary>
        ///     Cargar textura
        /// </summary>
        private void loadTerrainTexture(Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
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
         * float[,] GenerateHeights()
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
            //PreRender();
            //DrawText.drawText("Camera pos: " + TGCVector3.PrintTGCVector3(Camara.Position), 5, 20, Color.Red);
            //DrawText.drawText("Camera LookAt: " + TGCVector3.PrintTGCVector3(Camara.LookAt), 5, 40, Color.Red);

            // CrearHeightMap
            
            //createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            // Cargo textura del terreno
            //loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);

            //PostRender();
        }

        public void Dispose()
        {
            vbTerrain.Dispose();
            terrainTexture.Dispose();
        }
    }
}
 