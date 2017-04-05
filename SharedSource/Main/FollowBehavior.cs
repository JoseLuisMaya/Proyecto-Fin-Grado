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
    class FollowBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" } )]

        public string EntityPath { get; set; }

        private Transform3D targetTransform;

        private PhysicsManager physicsManager;

        private Ray ray;

        private Line line;

    
        protected override void Initialize()
        {
            base.Initialize();
            if (string.IsNullOrEmpty(this.EntityPath))
            {
                return;
            }
            var entity = this.EntityManager.Find(this.EntityPath);
            this.targetTransform = entity.FindComponent<Transform3D>();

        }
        protected override void Update(TimeSpan gameTime)
        {
            // this.physicsManager = this.Owner.Scene.PhysicsManager;
            //this.ray.Position = this.Transform.Position + Vector3.Up;
            //this.ray.Direction = Vector3.Down;
            //RayCastResult3D result;

            //             this.physicsManager.RayCast3D(this.ray, out result);
            //          if (result.HitBody != null)
            //        {

            //          Vector3 hitPosition = result.HitData.Location + (Vector3.Up * 0.1f);
            //        Vector3 target = result.HitData.Location + result.HitData.Normal;
            //this.Transform.Position = target;

            // this.line.StartPoint = result.HitData.Location;
            // this.line.EndPoint = target;
            //this.line.Color = Color.Red;
            //this.RenderManager.LineBatch3D.DrawLine(ref line);

            //}





          

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
    
