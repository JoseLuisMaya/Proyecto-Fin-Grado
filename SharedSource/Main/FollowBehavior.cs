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
   public class FollowBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" } )]

        public string EntityPath { get; set; }

        private Transform3D targetTransform;

    
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

        }
        protected override void Update(TimeSpan gameTime)
        {

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
                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);
                this.Transform.Position = Vector3.Lerp(this.Transform.Position, this.targetTransform.Position, lerp);
                this.Transform.Rotation = Vector3.Lerp(this.Transform.Rotation, this.targetTransform.Rotation, lerp);

        }
    }
}
    
