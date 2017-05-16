using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Materials;

namespace TFG
{
    [DataContract]
    class Fragmentation : Behavior
    {

        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]

        public string Plane { get; set; }

        private Entity planeEntity;

        private bool collisioned;

        private bool collisionedFrag;

        private bool ballFrags;

        private Vector3 positionCol;

        private Entity ballEntity;

        protected override void Initialize()
        {
            base.Initialize();

            if (this.Plane != null)
            {
                this.planeEntity = this.EntityManager.Find(this.Plane);
            }

            collisioned = false;

            collisionedFrag = false;

            ballFrags = false;

            positionCol = Vector3.Zero;
        }

        protected override void Update(TimeSpan gameTime)
        {
            //Cargar el proyectil en ballEntity
            this.ballEntity = this.EntityManager.Find("ball1");

            if (this.planeEntity == null)
            {
                return;
            }
            //En caso de que ball1 no exista comprobar si existe ballFrag1 (Proyectil fragmentado)
            if (this.ballEntity == null)
            {
                this.ballEntity = EntityManager.Find("ballFrag1");
                if (ballEntity == null)
                {
                    return;
                }
                else
                {
                    ballFrags = true;
                }
            }

            //Guardar el delineado del colider del plano
            var planeBounding = planeEntity.FindComponent<BoxCollider3D>().BoundingBox;

            //Guardar el delineado del colider de el proyectil
            var collider = this.ballEntity.FindComponent<SphereCollider3D>().BoundingSphere;

            //Comprobar si el proyectil ha colisionado con el plano
            if (planeBounding.Intersects(ref collider))
            {
                positionCol = ballEntity.FindComponent<Transform3D>().Position;
                if (ballFrags == false)
                {
                    collisioned = true;
                }
                else
                {
                    collisionedFrag = true;
                }
            }
            //Obtener todos los objetos con un tag definido
            var tags = EntityManager.FindAllByTag("worm");
            var enemyTags = EntityManager.FindAllByTag("Enemy");

            //Para cada objeto con dicho tag, comprobar si ha colisionado el proyectil con dicho objeto
            checkCollision(tags, collider);
            checkCollision(enemyTags, collider);
            

            /*Si la colisión la hace los proyectiles fragmentados recuperamos cada fragmento y lo eliminamos*/
            if (collisionedFrag == true)
            {

                for (int i = 1; i < 5; i++)
                {
                    this.ballEntity = this.EntityManager.Find("ballFrag" + i);
                    EntityManager.Remove(ballEntity);
                    collisionedFrag = false;
                    ballFrags = false;
                }

            }

            /*Si ha colisionado el proyectil principal creamos 4 proyectiles para simular la fragmentación,
             eliminamos el proyectil principal y lanzamos cada fragmento en una dirección*/
            if (collisioned == true)
            {
                for (int i = 1; i < 5; i++)
                {
                    Entity sphere = new Entity("ballFrag" + i)
                            .AddComponent(new Transform3D() { Scale = new Vector3(1), Position = positionCol })
                            .AddComponent(new MaterialsMap(new StandardMaterial(Color.Gray, DefaultLayers.Opaque)))
                            .AddComponent(Model.CreateSphere())
                            .AddComponent(new SphereCollider3D())
                            .AddComponent(new RigidBody3D() { Mass = 1, EnableContinuousContact = true })
                            .AddComponent(new Damage())
                            .AddComponent(new ModelRenderer());
                    EntityManager.Add(sphere);

                    if (ballEntity != null)
                    {
                        EntityManager.Remove(ballEntity);
                        ballEntity = null;
                    }

                    RigidBody3D rigidBody = sphere.FindComponent<RigidBody3D>();
                    rigidBody.ResetPosition(positionCol);
                    Vector3 direction = Vector3.Up;
                    if (sphere.Name.Equals("ballFrag1"))
                    {
                        direction = Vector3.Up + Vector3.Right + new Vector3(1, 0, 0);
                    }
                    else if (sphere.Name.Equals("ballFrag2"))
                    {
                        direction = Vector3.Up + Vector3.Left + new Vector3(-1, 0, 0);
                    }
                    else if (sphere.Name.Equals("ballFrag3"))
                    {
                        direction = Vector3.Forward + Vector3.Up + new Vector3(0, 0, -1);
                    }
                    else if (sphere.Name.Equals("ballFrag4"))
                    {
                        direction = Vector3.Up + Vector3.Backward + new Vector3(0, 0, 1);
                    }

                    direction.Normalize();

                    rigidBody.ApplyLinearImpulse(5 * direction);
                }
                collisioned = false;

            }
        }
        void checkCollision(IEnumerable<Object> tags, BoundingSphere collider)
        {
            //Para cada objeto con dicho tag, comprobar si ha colisionado el proyectil con dicho objeto
            foreach (Entity tagi in tags)
            {
                var playerBounding = tagi.FindComponent<BoxCollider3D>().BoundingBox;
                if (playerBounding.Intersects(ref collider))
                {
                    positionCol = ballEntity.FindComponent<Transform3D>().Position;

                    if (ballFrags == false)
                    {
                        collisioned = true;
                    }
                    else
                    {
                        collisionedFrag = true;
                    }
                }
            }
        }
            


    }
}
