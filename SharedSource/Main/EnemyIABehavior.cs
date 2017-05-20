using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace TFG
{
    [DataContract]
    public class EnemyIABehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string EntityPath { get; set; }

        [DataMember]
        public float Speed { get; set; }

        [RequiredComponent]
        private RigidBody3D rigidBodyTransform;

        private Vector3 direction = Vector3.Zero;
        private float distance;
        private float lastDistance;        
        public bool used;
        private float CurrentSpeed;
        private Transform3D transform;
        private Entity player;
        private Entity owner;
        private Entity sphere;
        private Vector3 directionShoot;
        private Vector3 direcMax;
        private Vector3 direcMin;
        private float up;
        private int force;
        public int maxForce;
        private int directionY;


        protected override void Initialize()
        {
            base.Initialize();
            used = false;
            this.CurrentSpeed = this.Speed;
            distance = 1000;
            lastDistance = 1000;

            //Cargamos la entidad que se movera
            var entity = this.EntityManager.Find(this.EntityPath);
            player = entity;
            owner = entity;

            transform = entity.FindComponent<Transform3D>();
            //Cargamos su componente rigidbody
            this.rigidBodyTransform = entity.FindComponent<RigidBody3D>();

            //Configuración de los componentes de fuerza y direccón del disparo
            force = 0;
            maxForce = 50;
            direcMax = player.FindComponent<Transform3D>().WorldTransform.Up * 7;
            direcMin = player.FindComponent<Transform3D>().WorldTransform.Backward * 7;
            direcMax = direcMax + direcMin;
            directionShoot = direcMin;
            up = 0;


        }
        protected override void Update(TimeSpan gameTime)
        {
            owner = this.EntityManager.Find(this.EntityPath);
            //Actualizar la dirección de disparo y la dirección de la puntería en cada frame
            direcMin = owner.FindComponent<Transform3D>().WorldTransform.Backward * 7;
            up = directionShoot.Y;
            directionShoot = direcMin + new Vector3(0, up, 0);
            

            if (owner == null)
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

            var players = EntityManager.FindAllByTag("worm");
            lastDistance = 1000;
            player = null;

            //Calcular el player mas cercano
            player =calculateDistanceWorm(players);

            //Calcular la fuerza de disparo
            force = calculateForce();

            //Calcular la dirección de disparo
            directionShoot = calculateDirection();
            if (player == null)
            {
                return;
            }
            
            
            if (lastDistance > 15)
            {
                //Rotar la posición para mirar al personaje y disparar
                transform.LookAt(player.FindComponent<Transform3D>().Position*-1);
                rigidBodyTransform.Rotation = transform.Rotation;
                transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rigidBodyTransform.Rotation.X, rigidBodyTransform.Rotation.Y, rigidBodyTransform.Rotation.Z);
                this.Transform.Rotation = rigidBodyTransform.Rotation;
                
            }
            else
            {
                if (sphere == null)
                {
                    //Rotar la posición para mirar al personaje y disparar
                    transform.LookAt(player.FindComponent<Transform3D>().Position * -1);
                    rigidBodyTransform.Rotation = transform.Rotation;
                    transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rigidBodyTransform.Rotation.X, rigidBodyTransform.Rotation.Y, rigidBodyTransform.Rotation.Z);
                    this.Transform.Rotation = rigidBodyTransform.Rotation;

                    Shoot();
                }
                
            }
                
        }

        //Método para calcular la fuerza de disparo
        int calculateForce()
        {
            System.Random rad = new System.Random();
            force = rad.Next(15,20);
            return force;
        }

        //Método para calcular la direción del disparo
        Vector3 calculateDirection()
        {
            System.Random rad = new System.Random();
            directionY = rad.Next((int)direcMin.Y, (int)direcMax.Y);
            directionShoot += new Vector3(0, directionY, 0); 
            return directionShoot;
        }

        //Método para calcular la distancia del personaje mas cercano
        Entity calculateDistanceWorm(IEnumerable<Object> worms)
        {
            foreach(Entity w in worms)
            {
                    distance = (w.FindComponent<Transform3D>().Position - transform.Position).Length();
                    if (distance < lastDistance)
                    {
                        lastDistance = distance;
                        direction = w.FindComponent<Transform3D>().Position - transform.Position;
                        player = w;
                    }
            }
            return player;
        }

        /*Función de disparo que crea el proyectil el cual se lanza desde la posición del enemigo 
        * en la dirección en la que mira el player con la inclinación calculada anteriormente y 
        * con la fuerza calculada y reinicia los valores de dirección y fuerza */
        private void Shoot()
        {
           
            if (sphere == null)
            {
                sphere = new Entity("ball1")
                .AddComponent(new Transform3D() { Scale = new Vector3(0.5f), Position = this.owner.FindComponent<Transform3D>().Position + Vector3.Up * 2 })
                .AddComponent(new MaterialsMap(new StandardMaterial(Color.Gray, DefaultLayers.Opaque)))
                .AddComponent(Model.CreateSphere())
                .AddComponent(new SphereCollider3D())
                .AddComponent(new RigidBody3D() { Mass = 2, EnableContinuousContact = true })
                .AddComponent(new Damage())
                .AddComponent(new ModelRenderer());
                EntityManager.Add(sphere);

            }



            RigidBody3D rigidBody = sphere.FindComponent<RigidBody3D>();
            rigidBody.ResetPosition(this.owner.FindComponent<Transform3D>().Position + Vector3.Up * 2);

            directionShoot.Normalize();

            rigidBody.ApplyLinearImpulse(force * directionShoot);
            force = 0;
            directionShoot = direcMin;
            
        }
    }
}
