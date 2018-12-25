using UnityEngine;
using System.Collections.Generic;

using ProceduralNoiseProject;

namespace SimpleProceduralTerrainProject
{
    [ExecuteInEditMode]
    public class TerrainGeneratorNew : MonoBehaviour
    {
        public bool reset;
        public int m_seed = 0;
        private List<GameObject> tiles = new List<GameObject>();
        
        //noise settings
        [Header("Terrain settings")]
        public float m_treeFrq = 0.005f;
        public float m_detailFrq = 0.01f;
        public float waterLevel;
        public Vector4 groundSmall, groundBig = new Vector4(0.01f,5,1,.1f);


        //Terrain settings
        public int m_tilesX = 2; //Number of terrain tiles on the x axis
        public int m_tilesZ = 2; //Number of terrain tiles on the z axis
        public float m_pixelMapError = 6.0f; //A lower pixel error will draw terrain at a higher Level of detail but will be slower
        public float m_baseMapDist = 1000.0f; //The distance at which the low res base map will be drawn. Decrease to increase performance

        //Terrain data settings
        public int m_heightMapSize = 513; //Higher number will create more detailed height maps
        public int m_alphaMapSize = 1024; //This is the control map that controls how the splat textures will be blended
        public int m_terrainSize = 2048;
        public int m_terrainHeight = 512;
        public int m_detailMapSize = 512; //Resolutions of detail (Grass) layers
         //Prototypes
        [Header("Prototype")]
        public Material customMaterial;
        public float soilSize = 10.0f;
        public float grassSize = 2.0f;
        public Texture2D soilTexture, grassTexture;
        public detail detailMaster;
        [Range(0,5)] public float detailSpread = 2;
        public int detailBillboards = 0;
        public detail[] details;
        public GameObject[] terrainTrees;
        public GameObject[] trees;
        //Tree settings
        public Vector2 m_treeScale = new Vector2(2,2);
        public int m_treeSpacing = 32; //spacing between trees
        public float m_treeDistance = 2000.0f; //The distance at which trees will no longer be drawn
        public float m_treeBillboardDistance = 400.0f; //The distance at which trees meshes will turn into tree billboards
        public float m_treeCrossFadeLength = 20.0f; //As trees turn to billboards there transform is rotated to match the meshes, a higher number will make this transition smoother
        public int m_treeMaximumFullLODCount = 400; //The maximum number of trees that will be drawn in a certain area. 

        //Detail settings
        public int m_detailObjectDistance = 400; //The distance at which details will no longer be drawn
        public float m_detailObjectDensity = 4.0f; //Creates more dense details within patch
        public int m_detailResolutionPerPatch = 32; //The size of detail patch. A higher number may reduce draw calls as details will be batch in larger patches
        public float m_wavingGrassStrength = 0.4f;
        public float m_wavingGrassAmount = 0.2f;
        public float m_wavingGrassSpeed = 0.4f;
        public Color m_wavingGrassTint = Color.white;
        public Color m_grassHealthyColor = Color.white;
        public Color m_grassDryColor = Color.white;
        //Private
        
        private FractalNoise noiseSmall, noiseBig, m_mountainNoise, m_treeNoise, m_detailNoise;
        private Terrain[,] m_terrain;
        private SplatPrototype[] m_splatPrototypes;
        private TreePrototype[] m_treeProtoTypes;
        private DetailPrototype[] m_detailProtoTypes;
        private Vector2 m_offset;
        public float radial;
        public float radialFalloff;
        public Vector2 underlaySize;
        
        private void Update() {
            for (int i = 0; i < details.Length; i++)
            {
                Shader.SetGlobalTexture("_Detail"+i, details[i].texture);
            }
            Shader.SetGlobalFloat("_DetailSpread", detailSpread);

            if (reset){
                reset=false;
                foreach (GameObject t in tiles) {
				    DestroyImmediate(t);
			    }
                tiles.Clear();
                Generate();
                Debug.Log("Generation Complete");
            }
        }
        [System.Serializable]
        public struct detail{
            public Texture2D texture;
            public float minWidth,minHeight,maxWidth,maxHeight;
        }

        public void Generate(){
            noiseSmall = new FractalNoise(new PerlinNoise(m_seed, groundSmall.x), Mathf.FloorToInt(groundSmall.y), groundSmall.z, groundSmall.w);
            noiseBig = new FractalNoise(new PerlinNoise(m_seed, groundBig.x), Mathf.FloorToInt(groundBig.y),groundBig.z, groundBig.w);
            
            m_treeNoise = new FractalNoise(new PerlinNoise(m_seed + 1, m_treeFrq), 6, 1.0f);
            m_detailNoise = new FractalNoise(new PerlinNoise(m_seed + 2, m_detailFrq), 6, 1.0f);

            m_heightMapSize = Mathf.ClosestPowerOfTwo(m_heightMapSize) + 1;
            m_alphaMapSize = Mathf.ClosestPowerOfTwo(m_alphaMapSize);
            m_detailMapSize = Mathf.ClosestPowerOfTwo(m_detailMapSize);

            if (m_detailResolutionPerPatch < 8)
                m_detailResolutionPerPatch = 8;

            float[,] htmap = new float[m_heightMapSize, m_heightMapSize];

            m_terrain = new Terrain[m_tilesX, m_tilesZ];

            //this will center terrain at origin
            m_offset = new Vector2(-m_terrainSize * m_tilesX * 0.5f, -m_terrainSize * m_tilesZ * 0.5f);

            CreateProtoTypes();
            //underlay
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = m_heightMapSize;
            terrainData.SetHeights(0, 0, new float[0,0]);
            terrainData.size = new Vector3(underlaySize.x, m_terrainHeight, underlaySize.y);
            terrainData.splatPrototypes = m_splatPrototypes;
            terrainData.treePrototypes = m_treeProtoTypes;
            terrainData.detailPrototypes = m_detailProtoTypes;
            addTextures(terrainData);
            GameObject o = Terrain.CreateTerrainGameObject(terrainData);
            o.name = "Underlay";
            o.transform.parent = transform;
            Terrain t = o.GetComponent<Terrain>();
            t.materialType=Terrain.MaterialType.Custom;
            t.materialTemplate = customMaterial;
            t.transform.position = transform.position- new Vector3(underlaySize.x/2, 1, underlaySize.y/2);
            FillDetailMap(t, -1,-1);
            tiles.Add(o);


            for (int x = 0; x < m_tilesX; x++)
            {
                for (int z = 0; z < m_tilesZ; z++)
                {
                    FillHeights(htmap, x, z);

                    terrainData = new TerrainData();

                    terrainData.heightmapResolution = m_heightMapSize;
                    terrainData.SetHeights(0, 0, htmap);
                    terrainData.size = new Vector3(m_terrainSize, m_terrainHeight, m_terrainSize);
                    terrainData.splatPrototypes = m_splatPrototypes;
                    terrainData.treePrototypes = m_treeProtoTypes;
                    terrainData.detailPrototypes = m_detailProtoTypes;

                    //FillAlphaMap(terrainData);
                    addTextures(terrainData);

                    o = Terrain.CreateTerrainGameObject(terrainData);
                    o.name = "TerrainTile "+x+"/"+z;
                    o.transform.parent = transform;
                    m_terrain[x, z] = o.GetComponent<Terrain>();
                    m_terrain[x, z].materialType=Terrain.MaterialType.Custom;
                    m_terrain[x, z].materialTemplate = customMaterial;
                    m_terrain[x, z].transform.position = transform.position+ new Vector3(m_terrainSize * x + m_offset.x, 0, m_terrainSize * z + m_offset.y);
                    m_terrain[x, z].heightmapPixelError = m_pixelMapError;
                    m_terrain[x, z].basemapDistance = m_baseMapDist;
                    m_terrain[x, z].castShadows = false;
                    tiles.Add(o);

                    FillTreeInstances(m_terrain[x, z], x, z);
                    FillDetailMap(m_terrain[x, z], x, z);

                }
            }
            
            //Set the neighbours of terrain to remove seams.
            for (int x = 0; x < m_tilesX; x++)
            {
                for (int z = 0; z < m_tilesZ; z++)
                {
                    Terrain right = null;
                    Terrain left = null;
                    Terrain bottom = null;
                    Terrain top = null;

                    if (x > 0) left = m_terrain[(x - 1), z];
                    if (x < m_tilesX - 1) right = m_terrain[(x + 1), z];

                    if (z > 0) bottom = m_terrain[x, (z - 1)];
                    if (z < m_tilesZ - 1) top = m_terrain[x, (z + 1)];

                    m_terrain[x, z].SetNeighbors(left, top, right, bottom);

                }
            }
        }

        void CreateProtoTypes()
        {


            m_treeProtoTypes = new TreePrototype[trees.Length];
            for (int i = 0; i < trees.Length; i++)
            {
                m_treeProtoTypes[i] = new TreePrototype();
                m_treeProtoTypes[i].prefab = trees[i];
            }

            m_detailProtoTypes = new DetailPrototype[details.Length];
            for (int i = 0; i < details.Length; i++)
            {
                Texture2D tex = new Texture2D(1,1,TextureFormat.ARGB32, false);
                Color c =new Color(i*.5f,i*.25f,i*.1f,i*.1f);
                tex.SetPixel(0,0,c);
                tex.Apply();
                DetailPrototype prot = new DetailPrototype();
                prot.prototypeTexture = details[i].texture;
                prot.renderMode = DetailRenderMode.Grass;
                prot.healthyColor = m_grassHealthyColor;
                prot.dryColor = m_grassDryColor;
                prot.minHeight = details[i].minHeight + detailMaster.minHeight;
                prot.minWidth = details[i].minWidth + detailMaster.minWidth;
                prot.maxHeight = details[i].maxHeight + detailMaster.maxHeight;
                prot.maxWidth = details[i].maxWidth + detailMaster.maxWidth;
                if (i+detailBillboards-details.Length>=0) prot.renderMode = DetailRenderMode.GrassBillboard;
                m_detailProtoTypes[i] = prot;
            }




        }

        void FillHeights(float[,] htmap, int tileX, int tileZ)
        {
            float ratio = (float)m_terrainSize / (float)m_heightMapSize;

            for (int x = 0; x < m_heightMapSize; x++)
            {
                for (int z = 0; z < m_heightMapSize; z++)
                {
                    float worldPosX = (x + tileX * (m_heightMapSize - 1)) * ratio;
                    float worldPosZ = (z + tileZ * (m_heightMapSize - 1)) * ratio;
                    float distance = new Vector3(worldPosX-m_terrainSize * m_tilesX/2,0,worldPosZ-m_terrainSize * m_tilesZ/2).magnitude;
                    htmap[z, x] = noiseBig.Amplitude + noiseSmall.Sample2D(worldPosX, worldPosZ);
                    htmap[z, x] += noiseSmall.Amplitude + noiseSmall.Sample2D(worldPosX, worldPosZ);
                    
                    htmap[z, x] *= Mathf.Clamp01((radial-distance)/radialFalloff);
                }

            }
        }

        void FillAlphaMap(TerrainData terrainData)
        {
            float[,,] map = new float[m_alphaMapSize, m_alphaMapSize, 2];

            for (int x = 0; x < m_alphaMapSize; x++)
            {
                for (int z = 0; z < m_alphaMapSize; z++)
                {
                    // Get the normalized terrain coordinate that
                    // corresponds to the the point.
                    float normX = x * 1.0f / (m_alphaMapSize - 1);
                    float normZ = z * 1.0f / (m_alphaMapSize - 1);

                    // Get the steepness value at the normalized coordinate.
                    float angle = terrainData.GetSteepness(normX, normZ);

                    // Steepness is given as an angle, 0..90 degrees. Divide
                    // by 90 to get an alpha blending value in the range 0..1.
                    float frac = angle / 90.0f;
                    map[z, x, 0] = frac;
                    map[z, x, 1] = 1.0f - frac;

                }
            }

            terrainData.alphamapResolution = m_alphaMapSize;
            terrainData.SetAlphamaps(0, 0, map);
        }

        

        void FillTreeInstances(Terrain terrain, int tileX, int tileZ)
        {
            Random.InitState(0);
            Transform parent = terrain.transform;
            for (int x = 0; x < m_terrainSize; x += m_treeSpacing)
            {
                for (int z = 0; z < m_terrainSize; z += m_treeSpacing)
                {

                    float unit = 1.0f / (m_terrainSize - 1);

                    float offsetX = Random.value * unit * m_treeSpacing;
                    float offsetZ = Random.value * unit * m_treeSpacing;

                    float normX = x * unit + offsetX;
                    float normZ = z * unit + offsetZ;

                    // Get the steepness value at the normalized coordinate.
                    float angle = terrain.terrainData.GetSteepness(normX, normZ);

                    // Steepness is given as an angle, 0..90 degrees. Divide
                    // by 90 to get an alpha blending value in the range 0..1.
                    float frac = angle / 90.0f;

                    float ht = terrain.terrainData.GetInterpolatedHeight(normX, normZ);

                    if (frac < 0.5f) //make sure tree are not on steep slopes
                    {
                        float worldPosX = x + tileX * (m_terrainSize - 1);
                        float worldPosZ = z + tileZ * (m_terrainSize - 1);

                        float noise = m_treeNoise.Sample2D(worldPosX, worldPosZ);
                        if (noise > 0.0f && ht < m_terrainHeight * 0.4f)
                        {
                            float distance = new Vector3(worldPosX-m_terrainSize * m_tilesX/2,0,worldPosZ-m_terrainSize * m_tilesZ/2).magnitude;
                            if (distance<radial){
                                if (Random.Range(0,trees.Length+terrainTrees.Length)<terrainTrees.Length){

                            
                                    TreeInstance temp = new TreeInstance();
                                    temp.position = new Vector3(normX, ht-1, normZ);
                                    temp.prototypeIndex = Random.Range(0, trees.Length);
                                    temp.widthScale = m_treeScale.x;
                                    temp.heightScale = m_treeScale.y;
                                    temp.color = Color.white;
                                    temp.lightmapColor = Color.white;
                                    //temp.rotation = Random.value * 2 * Mathf.PI;

                                    terrain.AddTreeInstance(temp);
                                }else{
                                        Transform o = Instantiate(trees[Random.Range(0,trees.Length)],parent).transform;
                                        o.position += new Vector3(x+tileX/2, ht-1, z+tileZ/2);
                                        o.parent = parent;
                                        o.rotation = Quaternion.Euler(0,Random.value*360,0);

                                }
                            }
                         
                        }
                    }

                }
            }

            terrain.treeDistance = m_treeDistance;
            terrain.treeBillboardDistance = m_treeBillboardDistance;
            terrain.treeCrossFadeLength = m_treeCrossFadeLength;
            terrain.treeMaximumFullLODCount = m_treeMaximumFullLODCount;

        }

        void FillDetailMap(Terrain terrain, int tileX, int tileZ)
        {
            //each layer is drawn separately so if you have a lot of layers your draw calls will increase 
            int[,,] detailMap = new int[details.Length,m_detailMapSize, m_detailMapSize];

            float ratio = (float)m_terrainSize / (float)m_detailMapSize;

            Random.InitState(0);

            for (int x = 0; x < m_detailMapSize; x++)
            {
                for (int z = 0; z < m_detailMapSize; z++)
                {
                    for (int i = 0; i < details.Length; i++)
                    {
                      detailMap[i,z,x] = 0;  
                    }
                    float unit = 1.0f / (m_detailMapSize - 1);

                    float normX = x * unit;
                    float normZ = z * unit;

                    // Get the steepness value at the normalized coordinate.
                    float angle = terrain.terrainData.GetSteepness(normX, normZ);

                    // Steepness is given as an angle, 0..90 degrees. Divide
                    // by 90 to get an alpha blending value in the range 0..1.
                    float frac = angle / 90.0f;

                    float ht = terrain.terrainData.GetInterpolatedHeight(normX, normZ);

                    if (frac < 0.5f && (ht>waterLevel || tileX<0))
                    {
                        float worldPosX = (x + tileX * (m_detailMapSize - 1)) * ratio;
                        float worldPosZ = (z + tileZ * (m_detailMapSize - 1)) * ratio;

                        float noise = m_detailNoise.Sample2D(worldPosX, worldPosZ);

                        if (noise > 0.0f)
                        {
                            detailMap[Random.Range(0,details.Length),x,z] = 1;
                        }
                    }

                }
            }

            terrain.terrainData.wavingGrassStrength = m_wavingGrassStrength;
            terrain.terrainData.wavingGrassAmount = m_wavingGrassAmount;
            terrain.terrainData.wavingGrassSpeed = m_wavingGrassSpeed;
            terrain.terrainData.wavingGrassTint = m_wavingGrassTint;
            terrain.detailObjectDensity = m_detailObjectDensity;
            terrain.detailObjectDistance = m_detailObjectDistance;
            terrain.terrainData.SetDetailResolution(m_detailMapSize, m_detailResolutionPerPatch);
            for (int i = 0; i < details.Length; i++)
            {
                terrain.terrainData.SetDetailLayer(0, 0, i, getMap(detailMap,i));
            }

        }
        int[,] getMap(int[,,] mapList, int index){
            int[,] map = new int[mapList.GetLength(1),mapList.GetLength(2)];
            for (int x = 0; x < mapList.GetLength(1); x++)
            {
                for (int y = 0; y < mapList.GetLength(2); y++)
                {
                    map[y,x]=mapList[index,x,y];
                }
            }
            return map;
        }


        private void addTextures(TerrainData terrainData)
        {
            var grassSplat = new SplatPrototype();
            grassSplat.tileSize = new Vector2(grassSize, grassSize);
            var rockSplat = new SplatPrototype();
            rockSplat.tileSize = new Vector2(soilSize, soilSize);

            grassSplat.texture = grassTexture;
            rockSplat.texture = soilTexture;

            terrainData.splatPrototypes = new SplatPrototype[]
                {
                    
                    rockSplat,
                    grassSplat
                };

            terrainData.RefreshPrototypes();

            var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];

            for (var zRes = 0; zRes < terrainData.alphamapHeight; zRes++)
            {
                for (var xRes = 0; xRes < terrainData.alphamapWidth; xRes++)
                {
                    var normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                    var normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);

                    var steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                    var steepnessNormalized = 1f-steepness / 90f;

                    splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;

                    float ht = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
                    float range = Mathf.Clamp01((ht-waterLevel)/4);
                    splatMap[zRes, xRes, 0] += 1f- range * steepnessNormalized;
                    splatMap[zRes, xRes, 1] += range * steepnessNormalized;
                }
            }   
            terrainData.SetAlphamaps(0, 0, splatMap);
        }
    }

    
   
}




