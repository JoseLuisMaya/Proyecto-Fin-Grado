using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;

namespace TFG
{
    [DataContract]
   public class FollowBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" } )]

        public string EntityPath { get; set; }

        private Transform3D targetTransform;

        private Entity sphere;
    
        protected override void Initialize()
        {
            base.Initialize();
            if (string.IsNullOrEmpty(this.EntityPath))
            {
                return;
            }

            //Cargamos la entidad a seguir
            var entity = this.EntityManager.Find(this.EntityPath);
            this.targetTransform = entity.FindComponent<Transform3D>();
            sphere = this.EntityManager.Find("ball1");
        }
        protected override void Update(TimeSpan gameTime)
        {

            sphere = this.EntityManager.Find("ball1");
            var entity = this.EntityManager.Find(this.EntityPath);
            if(entity == null)
            {
                return;
            }
            this.targetTransform = entity.FindComponent<Transform3D>();
            /*Mientras que tengamos un objetivo que seguir la posición de la cámara seguirá la posición y 
            rotación de el objetivo*/

            if (this.targetTransform == null)
                {
                    return;
                }
            if (WaveServices.Input.KeyboardState.Q != WaveEngine.Common.Input.ButtonState.Pressed && sphere==null)
            {

                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);
                this.Transform.Position = Vector3.Lerp(this.Transform.Position, this.targetTransform.Position, lerp);
                this.Transform.Rotation = Vector3.Lerp(this.Transform.Rotation, this.targetTransform.Rotation, lerp);
            } 
        }
    }
}
    
