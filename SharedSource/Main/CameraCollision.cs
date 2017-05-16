using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics3D;

namespace TFG
{
    [DataContract]
    class CameraCollision : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPath { get; set; }

        private Transform3D targetTransform;
        private String name;
        private String nameEnemy;
        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPathCamera { get; set; }

        private Transform3D camera;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPathPointCamera { get; set; }

        private Transform3D pointCamera;

        private PhysicsManager physicsManager;

        private Ray ray;

        

        protected override void Initialize()
        {
            base.Initialize();
            if (string.IsNullOrEmpty(this.EntityPath))
            {
                return;
            }
            if (string.IsNullOrEmpty(this.EntityPathCamera))
            {
                return;
            }
            if (string.IsNullOrEmpty(this.EntityPathPointCamera))
            {
                return;
            }
            var entity = this.EntityManager.Find(this.EntityPath);
            this.targetTransform = entity.FindComponent<Transform3D>();
            entity = this.EntityManager.Find(this.EntityPathCamera);
            this.camera = entity.FindComponent<Transform3D>();
            entity = this.EntityManager.Find(this.EntityPathPointCamera);
            this.pointCamera = entity.FindComponent<Transform3D>();
            name = null;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var entity = this.EntityManager.Find(this.EntityPath);
            if (entity == null)
            {
                return;
            }
            this.targetTransform = entity.FindComponent<Transform3D>();
            if (targetTransform == null)
            {
                return;
            }

            if (camera == null)
            {
                return;
            }

            this.physicsManager = this.Owner.Scene.PhysicsManager;
            
            //La posición del rayo es la posición de la cámara
            this.ray.Position = this.Transform.Position;
            
            //La dirección del rayo la calculamos restando la posición del personaje y la posición de la cámara
            this.ray.Direction = this.targetTransform.Position - this.Transform.Position;

            //Creación del rayo resultado
            RayCastResult3D result;
            this.physicsManager.RayCast3D(this.ray, out result);
            var worms = this.EntityManager.FindAllByTag("worm");
            var enemies = this.EntityManager.FindAllByTag("Enemy");
            
            foreach (Entity p in worms)
            {

                if (p.IsActive == true)
                {
                    name = p.Name;

                }
            }
            foreach (Entity e in enemies)
            {
                if(e.IsActive == true)
                {
                    nameEnemy = e.Name;
                }
            }
            //Comprobamos si el rayo ha colisionado con algún objeto
            if (result.HitBody != null)
            {
                /*Si el objeto ha colisionado con el personaje la posición de la cámara sera pointCamera
                que es donde estará la cámara mientras no colisione con otro objeto que no sea el player */
                if (result.HitBody.Owner.Name.Equals(name) || result.HitBody.Owner.Name.Equals(nameEnemy))
                {

                  this.camera.Position = this.pointCamera.Position;
                  
                }
                

                /*En caso de no colisionar con el personaje la cámara se coloca delante de el punto de 
                colisión para que el personaje nunca quede oculto*/
                else
                {
                    Vector3 hitPosition = result.HitData.Location;
                    camera.Position = hitPosition;
                 }
            }

          }

        }
    }
