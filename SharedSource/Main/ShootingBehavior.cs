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
        private  Vector3 direcMax;
        private  Vector3 direcMin;
        private bool increment;
        private bool decremet;
        private bool incrementDirection;
        private bool decremetDirection;
        private Vector3 direction;
        private Vector3 showAimMin;
        private Vector3 showAim;
        private float lineScale;
        private float up;
        Entity sphere;
        private Transform3D targetTransform;
        private Transform3D cameraTransform;
        

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPath { get; set; }

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityCamera { get; set; }



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
            entity = this.EntityManager.Find(this.EntityCamera);
            cameraTransform = entity.FindComponent<Transform3D>();
            force = 0;
            maxForce = 50;
            increment = true;
            decremet = false;
            incrementDirection = true;
            decremetDirection = false;
            lineScale = 0.1f;
            direcMax = targetTransform.WorldTransform.Up*7;
            direcMin = targetTransform.WorldTransform.Backward*7;
            direction = direcMin;
            showAimMin = targetTransform.WorldTransform.Left * 7;
            showAim = showAimMin;
            up = 0;
           
        }
        protected override void Update(TimeSpan gameTime)
        {
            
            //Actualizar la dirección de disparo y la dirección de la puntería en cada frame
            direcMin = targetTransform.WorldTransform.Backward * 7;
            showAimMin = targetTransform.WorldTransform.Left * 7;
            up = direction.Y;
            direction = direcMin+new Vector3(0,up,0);
            showAim = showAimMin + new Vector3(0, up, 0);

            if (targetTransform == null)
            {
                return;
            }

            //Buscar si el proyectil esta creado
            sphere = EntityManager.Find("ball1");

            //Si el proyectil no esta creado buscar un proyectil fragmentado
            if (sphere == null)
            {
                sphere = EntityManager.Find("ballFrag1");
            }
            //Si tenemos pulsada la tecla de disparo y no hay un proyectil actualmente lanzado, aumentar y disminuir la fuerza de disparo y la inclinación
            if (WaveServices.Input.KeyboardState.Q == WaveEngine.Common.Input.ButtonState.Pressed && sphere == null)
            { 
                if (!pressed)
                {
                    pressed = true;
                    
                }


                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);
                this.cameraTransform.Position = Vector3.Lerp(this.cameraTransform.Position, this.targetTransform.Position+Vector3.Up*-1f, lerp);
                this.cameraTransform.Rotation = Vector3.Lerp(this.cameraTransform.Rotation, this.targetTransform.Rotation, lerp);
                this.cameraTransform.Owner.FindChild("Camera").FindComponent<Transform3D>().LookAt(targetTransform.Position);
               
                //Incremento y decremento de la fuerza oscilando entre un mínimo y un máximo
                if (force <= maxForce && increment==true)
                {
                    force += 1f;
                }
                else
                {
                    increment = false;
                    decremet = true;
                }
                
                if (force >= 0 && decremet == true)
                {
                    force -= 1f;
                }
                else
                {
                    decremet = false;
                    increment = true;
                }

                //Incremento y decremento de la dirección de disparo y de puntería
                if (direction.Y < direcMax.Y && incrementDirection == true )
                {
                    direction += new Vector3(0, 0.01f, 0);
                    showAim += new Vector3(0, 0.01f, 0);
                }
                else
                {
                    incrementDirection = false;
                    decremetDirection = true;
                }
                if(direction.Y > direcMin.Y && decremetDirection == true )
                {
                    direction += new Vector3(0, -0.01f, 0);
                    showAim += new Vector3(0, -0.01f, 0);
                }
                else
                {
                    decremetDirection = false;
                    incrementDirection = true;
                }

                //Dibujar las lineas de fuerza y de la inclinación del disparo
                this.RenderManager.LineBatch3D.DrawPoint(targetTransform.Position, force * lineScale, Color.Red);
                RenderManager.LineBatch3D.DrawLine(targetTransform.Position, targetTransform.Position+showAim, Color.Yellow);
            }
            

            //Controlar que se ha dejado de pulsar la tecla de disparo y no hay un proyectil ya lanzado y llamar a la Función Shoot
            if ((WaveServices.Input.KeyboardState.Q == WaveEngine.Common.Input.ButtonState.Release) && pressed == true && sphere == null)
            {
                Shoot();
             
            }      
          
        }

        /*Función de disparo que crea el proyectil el cual se lanza desde la posición del personaje 
        * en la dirección en la que mira el player con la inclinación calculada anteriormente y 
        * con la fuerza calculada y reinicia los valores de dirección y fuerza */
        private void Shoot()
        {
            pressed = false;
            if (sphere == null)
            {
                sphere = new Entity("ball1")
                .AddComponent(new Transform3D() { Scale = new Vector3(0.5f), Position = this.targetTransform.Position+Vector3.Up*2 })
                .AddComponent(new MaterialsMap(new StandardMaterial(Color.Gray, DefaultLayers.Opaque)))
                .AddComponent(Model.CreateSphere())
                .AddComponent(new SphereCollider3D())
                .AddComponent(new RigidBody3D() { Mass = 2, EnableContinuousContact = true })
                .AddComponent(new Damage())
                .AddComponent(new ModelRenderer());
                EntityManager.Add(sphere);

            }



            RigidBody3D rigidBody = sphere.FindComponent<RigidBody3D>();
            rigidBody.ResetPosition(this.targetTransform.Position+Vector3.Up*2);
            
            direction.Normalize();

            rigidBody.ApplyLinearImpulse(force * direction);
            force = 0;
            direction = direcMin;
            showAim = showAimMin;
        }
    }
}
