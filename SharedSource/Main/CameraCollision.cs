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

        private Line line;

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
        }

        protected override void Update(TimeSpan gameTime)
        {

            if (targetTransform == null)
            {
                return;
            }

            if (camera == null)
            {
                return;
            }

            this.physicsManager = this.Owner.Scene.PhysicsManager;
            this.ray.Position = this.Transform.Position;
            this.ray.Direction = this.targetTransform.Position - this.Transform.Position;
            RayCastResult3D result;
            this.physicsManager.RayCast3D(this.ray, out result);

            if (result.HitBody != null)
            {
                if (result.HitBody.Owner.Name.Equals("Worm"))
                {

                  //Vector3 hitPosition = result.HitData.Location + (Vector3.Up * 0.1f);
                  //Vector3 target = result.HitData.Location + result.HitData.Normal;
                  //Vector3 target = result.HitData.Location + this.targetTransform.Position;
                    this.camera.Position = this.pointCamera.Position;
                  //this.line.StartPoint = hitPosition;
                  //this.line.Color = Color.Red;
                  //this.RenderManager.LineBatch3D.DrawLine(ref line);

                }
                else
                {
                    Vector3 hitPosition = result.HitData.Location;
                    camera.Position = hitPosition;
                 }
            }

          }

        }
    }
