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
        public bool used;
        protected override void Initialize()
        {
            base.Initialize();
            used = false;
            this.CurrentSpeed = this.Speed;

            //Cargamos la entidad que se movera
            var entity = this.EntityManager.Find(this.EntityPath);

            //Cargamos su componente rigidbody
            this.rigidBodyTransform = entity.FindComponent<RigidBody3D>();
            rotation = Vector3.Zero;
            
            
        }
        protected override void Update(TimeSpan gameTime)
        {
           
            var input = WaveServices.Input.KeyboardState;
            

            /*Controlamos que mientras pulsemos el botón de salto y la posición sea menor que 1 el personaje
            saltara. Le ponemos un máximo para que no siga subiendo hasta el infinito*/
            if (input.Space == WaveEngine.Common.Input.ButtonState.Pressed && this.Transform.Position.Y<=1)
            {
                rigidBodyTransform.ApplyLinearImpulse(this.Transform.WorldTransform.Up *1.0f);
                

            }

            //Controlamos la dirección del desplazamiento dependiendo de la tecla pulsada
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

                rotation.Y -= (float)gameTime.TotalSeconds;
                
            }
            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                
                rotation.Y += (float)gameTime.TotalSeconds;
              
            }
            rigidBodyTransform.Rotation = rotation;
           
            //Actualizamos la rotación del personaje
            this.Transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rigidBodyTransform.Rotation.X, rigidBodyTransform.Rotation.Y, rigidBodyTransform.Rotation.Z);
            this.Transform.Rotation = rigidBodyTransform.Rotation;
            


        }
    }
}