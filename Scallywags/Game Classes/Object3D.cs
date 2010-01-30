using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace Scallywags
{
    /** @class  Object3D
     *  @brief  base class for all the 3d game objects
     */
    public class Object3D
    {
        private Model m_model;          ///< The model used to display the object
                                        ///
        private Vector3 m_vLocation;    ///< The location of the object
        private Vector3 m_vEulerAngles; ///< The yaw pitch and roll of the object
        private Vector3 m_vScale;       ///< THe scale of the object
        protected Texture2D m_tDiffuse;    ///< The Diffuse map Texture
        protected Color m_Color;                                
        protected float m_Alpha;

        protected List< Effect >      m_lstEffects;     ///< Shaders to apply to this object                                                    
        protected List< Vector3 >     m_lstMaterials;   ///< Materials read from the model
        protected List< Texture2D >   m_lstTextures;    ///< Textures read from the model?                                         

        #region Properties

        /** @prop   Shaders
         *  @brief  list of shaders that will attempt to be applied to this object
         */
        public List< Effect > Shaders
        {
            get
            {
                return m_lstEffects;
            }
        }

        /** @prop   TheModel
         *  @brief  the model used to display the game object
         */
        public Model TheModel
        {
            get
            {
                return m_model;
            }
        }

        /** @prop   XScale
         *  @brief  the x scale of the object
         */
        public float XScale
        {
            get
            {
                return m_vScale.X;
            }
            set
            {
                m_vScale.X = value;
            }
        }

        /** @prop   YScale
         *  @brief  the y scale of the object
         */
        public float YScale
        {
            get
            {
                return m_vScale.Y;
            }
            set
            {
                m_vScale.Y = value;
            }
        }

        /** @prop   ZScale
         *  @brief  the z scale of the object
         */
        public float ZScale
        {
            get
            {
                return m_vScale.Z;
            }
            set
            {
                m_vScale.Z = value;
            }
        }

        /** @prop   Scale
         *  @brief  the uniform scale of the object
         */
        public float Scale
        {
            get
            {
                if (m_vScale.X == m_vScale.Y)
                    if (m_vScale.Y == m_vScale.Z)
                return m_vScale.X;
                return -1;
            }
            set
            {
                m_vScale = new Vector3(value, value, value);
            }
        }

        /** @prop   ScaleVec
         *  @brief  the scale vector
         */
        public Vector3 ScaleVec
        {
            get
            {
                return m_vScale;
            }
        }

        /** @prop   Location    
         *  @brief  the world location of the object
         */
        public Vector3 Location
        {
            get
            {
                return m_vLocation;
            }
            set
            {
                m_vLocation = value;
            }
        }

        /** @prop   X
         *  @brief  the x location of the object
         */
        public float X
        {
            get
            {
                return m_vLocation.X;
            }
            set
            {
                m_vLocation.X = value;
            }
        }

        public float Alpha
        {
            get
            {
                return m_Alpha;
            }
            set
            {
                m_Alpha = value;
            }
        }

        /** @prop   Y
         *  @brief  the y location of the object
         */
        public float Y
        {
            get
            {
                return m_vLocation.Y;
            }
            set
            {
                m_vLocation.Y = value;
            }
        }

        /** @prop   Z
         *  @brief  the z location of the object
         */
        public float Z
        {
            get
            {
                return m_vLocation.Z;
            }
            set
            {
                m_vLocation.Z = value;
            }
        }

        /** @prop   Yaw
         *  @brief  the rotation on the Y axis (nose up/down)
         */
        public float Yaw
        {
            get
            {
                return m_vEulerAngles.Y;
            }
            set
            {
                m_vEulerAngles.Y = value;
            }
        }

        /** @prop   Pitch
         *  @brief  the rotation on the X axis (left/right)
         */
        public float Pitch
        {
            get
            {
                return m_vEulerAngles.X;
            }
            set
            {
                m_vEulerAngles.X = value;
            }
        }

        /** @prop   Roll
         *  @brief  the rotation on the Z axis, rolling
         */
        public float Roll
        {
            get
            {
                return m_vEulerAngles.Z;
            }
            set
            {
                m_vEulerAngles.Z = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }

        #endregion

        /** @fn     Object3D( Model model )
         *  @brief  constructor
         *  @param  model [in] the model to use to display the game object
         */
        public Object3D( Model model )
        {
            m_model = model;

            //int numVerts = m_model.Meshes[0].MeshParts[0].NumVertices;      ///This could be anything
            //int vertSize = m_model.Meshes[0].MeshParts[0].VertexStride;     ///This should be 32
            m_Alpha = 1.0f;

            m_vLocation     = Vector3.Zero;
            m_vEulerAngles  = Vector3.Zero;
            m_vScale        = Vector3.One;
            m_tDiffuse = null;
            m_Color = Color.TransparentWhite;

            m_lstEffects    = new List< Effect >();
            m_lstMaterials  = new List< Vector3 >();
            m_lstTextures = new List< Texture2D >();

            //Attempt to get the materials and texture data from the model
            ExtractMaterials();
        }

        /** @fn     Object3D( Model model )
 *  @brief  constructor
 *  @param  model [in] the model to use to display the game object
 */
        public Object3D(Model model, Texture2D DiffuseMap)
        {
            m_model = model;

            //int numVerts = m_model.Meshes[0].MeshParts[0].NumVertices;      ///This could be anything
            //int vertSize = m_model.Meshes[0].MeshParts[0].VertexStride;     ///This should be 32
            m_Alpha = 1.0f;

            m_vLocation = Vector3.Zero;
            m_vEulerAngles = Vector3.Zero;
            m_vScale = Vector3.One;
            m_tDiffuse = DiffuseMap;
            m_Color = Color.TransparentWhite;

            m_lstEffects = new List<Effect>();
            m_lstMaterials = new List<Vector3>();
            m_lstTextures = new List<Texture2D>();

            //Attempt to get the materials and texture data from the model
            ExtractMaterials();
        }

        public Object3D()
        {
            m_model = null;
            m_Alpha = 1.0f;

            m_vLocation = Vector3.Zero;
            m_vEulerAngles = Vector3.Zero;
            m_vScale = Vector3.One;
            m_tDiffuse = null;
            m_Color = Color.TransparentWhite;

            m_lstEffects = new List<Effect>();
            m_lstMaterials = new List<Vector3>();
            m_lstTextures = new List<Texture2D>();
        }

        /** @fn     void SetModel( Model mdl )
         *  @brief  allow inherting objects to change the model on the fly
         *  @param  mdl [in] the new model to draw
         */
        protected void SetModel( Model mdl )
        {
            m_model = mdl;
        }

        /** @fn     void ExtractMaterials()
         *  @brief  get the texture and diffuse material information from the model loaded with the default content processor
         */
        private void ExtractMaterials()
        {
            
            foreach (ModelMesh mesh in m_model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    Vector3 vDiffuse = part.Effect.Parameters["DiffuseColor"].GetValueVector3();
                    m_lstMaterials.Add(vDiffuse);

                    Texture2D tex = part.Effect.Parameters["BasicTexture"].GetValueTexture2D();

                    tex = part.Effect.Parameters["BasicTexture"].GetValueTexture2D();
                    m_lstTextures.Add(tex);
                }
            }
        }

        /** @fn     void SwapShader()
        *  @brief  Swaps the existing Shader with a given one
        */
        public void SwapShader(Effect effect)
        {
            m_lstEffects.Clear();
            m_lstEffects.Add(effect);
        }

        /** @fn     void Draw(Matrix matView, Matrix matProjection)
         *  @brief  draw the game object
         *  @param  device [in] the rendering device
         *  @param  matView [in] the active view matrix
         *  @param  matProjection [in] the active projection
         */
        public virtual void Draw( GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            //dunno - animation related
            //Matrix[] matTransforms = new Matrix[m_model.Bones.Count];
            //m_model.CopyAbsoluteBoneTransformsTo(matTransforms);

            //create the model's world transform matrix
            Matrix matWorld = Matrix.Identity *                 //clear
                Matrix.CreateScale(m_vScale) *                  //scale
                Matrix.CreateFromYawPitchRoll(m_vEulerAngles.Y, m_vEulerAngles.X, m_vEulerAngles.Z) *   //Rotation
                Matrix.CreateTranslation(m_vLocation);          //translate

            if( m_lstEffects.Count > 0  )
            {
                string OldTech = m_lstEffects[0].CurrentTechnique.Name;
                if (Settings.DETECT_EDGES == false)
                {
                    if (m_tDiffuse != null)
                    {
                        m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques["DiffuseTech"];
                    }
                    DrawWithCustomShaders( device, matWorld, matView, matProjection, CameraPosition );
                    m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques[OldTech];
                }
                else
                {
                    if(m_lstEffects[0].CurrentTechnique.Name == "WaterTech")
                        m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques["NormalDepthWater"];
                    else
                        m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques["NormalDepth"];
                    DrawWithCustomShaders( device, matWorld, matView, matProjection, CameraPosition );
                    m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques[OldTech];

                }
            }
            else
            {
                foreach (ModelMesh mesh in m_model.Meshes)
                {
                    ///////////////////////////////
                    //Draw with default shader
                    foreach (BasicEffect eff in mesh.Effects)
                    {
                        eff.EnableDefaultLighting();
                        eff.World = matWorld;//matTransforms[mesh.ParentBone.Index] * matWorld;
                        eff.View = matView;
                        eff.Projection = matProjection;

                        mesh.Draw();
                    }
                }
            }
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the object
         */
        public virtual void Update(float fElapsedTime)
        {
            //Not pure virtual..
            
        }

        /** @fn     void DrawWithCustomShaders()
         *  @brief  draw the model with any custom shaders
         *  @param  device [in] the rendering device
         *  @param  matWorld [in] the world matrix transformation
         *  @param  matView [in] the view matrix
         *  @param  matProj [in] the projection matrix
         */
        private void DrawWithCustomShaders( GraphicsDevice device, Matrix matWorld, Matrix matView, Matrix matProj, Vector3 CameraPosition )
        {
            foreach (Effect effect in m_lstEffects)
            {
                //hacks here...
                if (effect.Parameters["g_matTransform"] != null)
                {
                    Matrix matTransform = matWorld * matView * matProj;
                    effect.Parameters["g_matTransform"].SetValue(matTransform);
                }

                ////////////////////////////
                //Basic Effect parameters
                if (effect.Parameters["World"] != null)
                    effect.Parameters["World"].SetValue(matWorld);

                if (effect.Parameters["View"] != null)
                    effect.Parameters["View"].SetValue(matView);

                if (effect.Parameters["Projection"] != null)
                    effect.Parameters["Projection"].SetValue(matProj);

                if (effect.Parameters["g_Alpha"] != null)
                    effect.Parameters["g_Alpha"].SetValue(m_Alpha);

                //Material colour
                if (effect.Parameters["DiffuseColor"] != null && m_Color != Color.TransparentWhite)
                    effect.Parameters["DiffuseColor"].SetValue(m_Color.ToVector3());
                else if (effect.Parameters["DiffuseColor"] != null && m_lstMaterials.Count > 0)
                    effect.Parameters["DiffuseColor"].SetValue(m_lstMaterials[0]);

                if (effect.Parameters["EyePosition"] != null)
                    effect.Parameters["EyePosition"].SetValue(CameraPosition);

                if (effect.Parameters["DiffuseMap"] != null && m_tDiffuse != null)
                    effect.Parameters["DiffuseMap"].SetValue(m_tDiffuse);

                //Active texture
                if (effect.Parameters["BasicTexture"] != null && m_lstTextures.Count > 0)
                    effect.Parameters["BasicTexture"].SetValue(m_lstTextures[0]);

                ///////////////////////////////////////////
                //Render the mesh with the current effect

                effect.Begin();

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    //mesh.Draw();
                    DrawMesh(device);

                    pass.End();
                }

                effect.End();
            }//end shader loop
        }//end function

        /** @fn     void DrawMesh( GraphicsDevice device )
         *  @brief  draw the mesh
         *  @param  device [in] the graphics device
         */
        public void DrawMesh( GraphicsDevice device )
        {
            foreach( ModelMesh mesh in m_model.Meshes )
            {
                VertexBuffer vb = mesh.VertexBuffer;
                IndexBuffer ib  = mesh.IndexBuffer;
                
                device.Indices      = ib;
               
                foreach( ModelMeshPart part in mesh.MeshParts )
                {
                    device.VertexDeclaration = part.VertexDeclaration;
                    device.Vertices[0].SetSource(vb, 0, part.VertexStride );

                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount ); 
                }
            }
        }
    }
}