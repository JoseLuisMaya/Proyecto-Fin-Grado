using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework.Services;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Common.Math;
using WaveEngine.Materials;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;

namespace TFG
{
    [DataContract]
    class ShootingBehavior : Behavior
    {
        private bool pressed;
        float force;
        public float maxForce;
        private bool increment;
        private bool decremet;

        private LineBatch3D lineForce;
        private float lineScale;
        Entity sphere;
        private Transform3D targetTransform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPath { get; set; }



        public ShootingBehavior() : base("ShootingBehavior")
        {
            
        }

        protected override void Initialize()
        {
            if (string.IsNullOrEmpty(this.EntityPath))
            {
                return;
            }

            var entity = this.EntityManager.Find(this.EntityPath);
            this.targetTransform = entity.FindComponent<Transform3D>();
            force = 0;
            maxForce = 50;
            increment = true;
            decremet = false;
            lineScale = 0.1f;
            
        }
        protected override void Update(TimeSpan gameTime)
        {
            if (targetTransform == null)
            {
                return;
            }
            if (WaveServices.Input.KeyboardState.Q == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                
                
                
                if (!pressed)
                {
                    pressed = true;

                }

                if (force <= maxForce && increment==true)
                {
                    force += 1f;
                }
                else
                {
                    increment = false;
                    decremet = true;
                }
                // else
                //{
                //    Shoot();
                //}
                if (force >= 0 && decremet == true)
                {
                    force -= 1f;
                }
                else
                {
                    decremet = false;
                    increment = true;
                }
              // force = pingpong(gameTime,maxForce-2)+2;
                    //this.RenderManager.LineBatch3D.DrawLine(targetTransform.Position, targetTransform.Position+ Vector3.Forward * force * lineScale, Color.Red);
                this.RenderManager.LineBatch3D.DrawPoint(targetTransform.Position, force * lineScale, Color.Red);


            }
            sphere = EntityManager.Find("ball1");
            if (sphere == null)
            {
                sphere = EntityManager.Find("ballFrag1");
            }

            if ((WaveServices.Input.KeyboardState.Q == WaveEngine.Common.Input.ButtonState.Release) && pressed == true && sphere == null)
            {
                Shoot();
            }

      
           
          
        }
        private void Shoot()
        {
            pressed = false;
            if (sphere == null)
            {
                sphere = new Entity("ball1")
                .AddComponent(new Transform3D() { Position = this.targetTransform.Position + Vector3.Up })
                .AddComponent(new MaterialsMap(new StandardMaterial(Color.Gray, DefaultLayers.Opaque)))
                .AddComponent(Model.CreateSphere())
                .AddComponent(new SphereCollider3D())
                .AddComponent(new RigidBody3D() { Mass = 5, EnableContinuousContact = true })
                .AddComponent(new ModelRenderer());
                EntityManager.Add(sphere);

            }



            RigidBody3D rigidBody = sphere.FindComponent<RigidBody3D>();
            rigidBody.ResetPosition(this.targetTransform.Position + Vector3.Up);

            // Vector3 direction = Camera.Transform.WorldTransform.Forward;
            Vector3 direction = Vector3.Up*5;
            //direction.Normalize();

            rigidBody.ApplyLinearImpulse(force * direction);
            force = 0;
        }

    private float pingpong(TimeSpan time, float length)
        {

            var L = 2 * length;
            var S = time.Seconds * 20;
            var T = time.Milliseconds% L;


            if (0 <= T && T < length){
                return T;
            }

            else
            {
                return L - T;
            }
    

            
        }
    }
}
