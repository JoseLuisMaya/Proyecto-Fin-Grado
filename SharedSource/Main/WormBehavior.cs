using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;

namespace TFG
{
    [DataContract]
    public class WormBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPath { get; set; }

        [DataMember]
        public float Speed { get; set; }

        private float CurrentSpeed;

        [RequiredComponent]
        private RigidBody3D rigidBodyTransform;

        private Vector3 rotation;
        protected override void Initialize()
        {
            base.Initialize();

            this.CurrentSpeed = this.Speed;

            var entity = this.EntityManager.Find(this.EntityPath);
            this.rigidBodyTransform = entity.FindComponent<RigidBody3D>();
            rotation = Vector3.Zero;
            
            
        }
        protected override void Update(TimeSpan gameTime)
        {
           
            var input = WaveServices.Input.KeyboardState;
            //  var localPosition = this.Transform.LocalPosition;


            if (input.Space == WaveEngine.Common.Input.ButtonState.Pressed && this.Transform.Position.Y<=1)
            {
                rigidBodyTransform.ApplyLinearImpulse(this.Transform.WorldTransform.Up *1.0f);
                

            }
            if (input.S == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rigidBodyTransform.ApplyLinearImpulse(this.Transform.WorldTransform.Forward *( this.CurrentSpeed * (float)gameTime.TotalSeconds));

            }
            if (input.W == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rigidBodyTransform.ApplyLinearImpulse(this.Transform.WorldTransform.Backward * this.CurrentSpeed * (float)gameTime.TotalSeconds);

            }
            if (input.D == WaveEngine.Common.Input.ButtonState.Pressed)
            {

                // rotation.X -= (float)gameTime.TotalSeconds;
                rotation.Y -= (float)gameTime.TotalSeconds;
                
            }
            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                
                rotation.Y += (float)gameTime.TotalSeconds;
              
            }
            rigidBodyTransform.Rotation = rotation;
           
            this.Transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rigidBodyTransform.Rotation.X, rigidBodyTransform.Rotation.Y, rigidBodyTransform.Rotation.Z);
            this.Transform.Rotation = rigidBodyTransform.Rotation;
            //this.Transform.LocalPosition = localPosition;
            


        }
    }
}