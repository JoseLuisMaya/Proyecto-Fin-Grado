using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;

namespace TFG
{
    class MySceneBehavior : SceneBehavior
    {
        private int time = 15000;
        private int time1 = 1;
        private bool player = true;
        private bool enemy = false;
        private Entity pointCamera = null;
        private int numberOfPlayer = 0;
        private int numberOfEnemy = 0;
        private bool firts = true;
        private Entity cameraHolder = null;
        private Entity cameraGame = null;
        protected override void ResolveDependencies()
        {
            var ground = (this.Scene as MyScene).EntityManager.Find<Entity>("Ground");

        }

        protected override void Update(TimeSpan gameTime)
        {
            //Obtener el estado actual
            var state = (this.Scene as MyScene).CurrentState;

            var menu = (this.Scene as MenuScene);
            

            switch (state)
            {
                //Estado que inicializa los componentes del juego al empezar la partida
                case MyScene.State.Init:
                    cameraHolder = (this.Scene as MyScene).EntityManager.Find("cameraHolder");
                     var Players = (this.Scene as MyScene).EntityManager.FindAllByTag("worm");
                     var Enemies = (this.Scene as MyScene).EntityManager.FindAllByTag("Enemy");

                    //Desactivamos todos los personajes menos el personaje Worm1 el cual lo marcaremos como usado
                    foreach (Entity p in Players)
                    {

                        if (p.IsActive == true && !(p.Name.Equals("Worm1")))
                        {
                            p.IsActive = false;
                        }
                        if (p.Name.Equals("Worm1"))
                        {
                            p.FindComponent<WormBehavior>().used = true;
                        }
                        
                    }

                    //Desactivamos todos los enemigos
                    foreach (Entity e in Enemies)
                    {
                        if (e.IsActive == true)
                        {
                            e.IsActive = false;
                            
                        }
                       
                    }

                    //Actualizamos el estado del juego a Game
                    SetState(MyScene.State.Game);



                    break;

                case MyScene.State.Finish:
                    //Controlamos desde que estado viene para mostrar el menú dependiendo de si viene de GameOver o de Win
                    var lastState = (this.Scene as MyScene).lastState;

                    if (lastState.ToString().Equals("Win"))
                    {
                        ScreenContext MenuScene = new ScreenContext(new MenuScene());
                        WaveServices.ScreenContextManager.To(MenuScene);
                    }
                    else
                    {
                        ScreenContext GameOverMenu = new ScreenContext(new GameOverMenu());
                        WaveServices.ScreenContextManager.To(GameOverMenu);

                    }

                    break;


                case MyScene.State.Game:

                    var textBlock = (this.Scene as MyScene).textblockInit;
                    var textLife = (this.Scene as MyScene).textblockLife;
                    textBlock.IsVisible = true;
                    textLife.IsVisible = true;
                    time1 = time / 1000;
                    textBlock.Text = time1.ToString();
                    
                    float life = 0;
                     Players = (this.Scene as MyScene).EntityManager.FindAllByTag("worm");
                     Enemies = (this.Scene as MyScene).EntityManager.FindAllByTag("Enemy");
                    Entity worm = null;

                    //Comprobamos si es turno del jugador
                    if (player == true)
                    {
                        //Comprobamos el jugador activo y obtenemos su nivel de vida
                        foreach (Entity p in Players)
                        {
                            if (p.IsActive == true)
                            {
                                worm = p;
                                life = p.FindComponent<Life>().CurrentLife;
                                textLife.Text = life.ToString();
                                continue;
                            }
                        }
                    }
                    else
                    {

                        //Comprobamos si es el turno del enemigo
                        foreach (Entity e in Enemies)
                        {
                            //Comprobar el enemigo activo y obtenemos su vida
                            if (e.IsActive == true)
                            {
                                worm = e;
                                life = e.FindComponent<Life>().CurrentLife;
                                textLife.Text = life.ToString();
                                continue;
                            }
                        }
                    }

                    //Controlamos si el player a muerto antes de acabar el turno
                    if (worm == null)
                    {
                        time = 15000;
                        WaveServices.TimerFactory.RemoveTimer("Init");
                        textBlock.IsVisible = false;
                        //Cambiamos el estado a Change
                        SetState(MyScene.State.Change);
                        if (player == true)
                        {
                            player = false;
                            enemy = true;
                        }
                        else
                        {
                            player = true;
                            enemy = false;
                        }
                        
                        break;
                    }


                    textLife.Text = "LIFE: "+life.ToString();
                    //Decrementamos el tipo del turno
                    WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        time--;

                    });

                    //Controlamos cuando el contador llega a cero
                    if (time1 <= 0)
                    {
                        time = 15000;
                        WaveServices.TimerFactory.RemoveTimer("Init");
                        textBlock.IsVisible = false;
                        SetState(MyScene.State.Change);
                        if(player == true)
                        {
                            player = false;
                            enemy = true;
                        }
                        else
                        {
                            player = true;
                            enemy = false;
                        }
                        worm.IsActive = false;
                     
                    }


                    break;

                case MyScene.State.GameOver:
                    //Guardamos el ultimo estado y cambiamos de estado
                    lastState = (this.Scene as MyScene).CurrentState;
                    SetState(MyScene.State.Finish);
                    break;

                case MyScene.State.Win:
                    //Guardamos el ultimo estado y cambiamos de estado
                    lastState = (this.Scene as MyScene).CurrentState;
                    SetState(MyScene.State.Finish);
                    break;

                case MyScene.State.Change:

                    //Controlamos si es turno del jugador
                    if(player == true)
                    {
                     var players=   (this.Scene as MyScene).EntityManager.FindAllByTag("worm");
                        //Comprobación de si existen jugadores
                        if (IsEmpty(players)==true)
                        {
                            SetState(MyScene.State.GameOver);
                        }
                        else
                        {
                            
                            List<Entity> noUsed = new List<Entity>();
                            noUsed =calculateUsed(players, noUsed);
                            //Contamos el numero de jugadores disponibles en la partida que no han sido usados
                            numberOfPlayer = noUsed.Count();
                            
                            if (numberOfPlayer == 0)
                            {

                                //En caso de que existan jugadores en la partida pero todos hayan sido usado quitamos el usado a todos los jugadores
                                foreach (Entity c in players)
                                {
                                    var Behavior = c.FindComponent<WormBehavior>();
                                    if (Behavior.used == true)
                                    {
                                        Behavior.used = false;
                                    }
                                }
                                noUsed = calculateUsed(players, noUsed);
                                numberOfPlayer = noUsed.Count();
                            }

                            //Obtenemos el índice de un jugador de los no usados de manera aleatoria
                            int getIndex =IndexRandom(numberOfPlayer);

                            Entity selected =noUsed.ElementAt(getIndex);
                            selected.FindComponent<WormBehavior>().used = true;
                            //Activamos el player
                            selected.IsActive = true;
                            //Modificamos la cámara para el nuevo jugador
                            activeCamera(selected);
                            SetState(MyScene.State.Game);
                        }
                        
                    }
                    else
                    {
                        var enemies = (this.Scene as MyScene).EntityManager.FindAllByTag("Enemy");

                        //Comprobamos si existen enemigos
                        if (IsEmpty(enemies) == true)
                        {
                            SetState(MyScene.State.Win);
                        }
                        else
                        {

                            List<Entity> noUsed = new List<Entity>();
                            //Calculamos los enemigos no usados
                            noUsed =calculateEnemyUsed(enemies,noUsed);
                            numberOfEnemy = noUsed.Count();

                            if (numberOfEnemy == 0)
                            {
                                //Si todos los enemigos fueron usados desmarcamos todos los enemigos como usados
                                foreach (Entity c in enemies)
                                {
                                    var Behavior = c.FindComponent<EnemyIABehavior>();
                                    if (Behavior.used == true)
                                    {
                                        Behavior.used = false;
                                    }
                                }
                                noUsed =calculateEnemyUsed(enemies, noUsed);
                                numberOfEnemy = noUsed.Count();

                            }

                            //Obtenemos el índice del enemigo a seleccionar de forma aleatoria
                            int getIndex = IndexRandom(numberOfEnemy);

                            Entity selected = noUsed.ElementAt(getIndex);
                            selected.FindComponent<EnemyIABehavior>().used = true;
                            //Activamos al enemigo
                            selected.IsActive = true;
                            //Colocamos la cámara para ser utilizada por dicho enemigo
                            activeCamera(selected);
                            SetState(MyScene.State.Game);
                        }
                    }
                    break;
            }

        }

        //Método para actualizar los estados
        private void SetState(MyScene.State _State)
        {
            (this.Scene as MyScene).lastState = (this.Scene as MyScene).CurrentState;
            (this.Scene as MyScene).CurrentState = _State;
        }

        //Método para comprobar si existen personajes
        bool IsEmpty(IEnumerable en)
        {
            foreach (var c in en) { return false; }
            return true;
        }

        //Calculo del índice del personaje del siguiente turno
        int IndexRandom(int count)
        {
            System.Random rad = new System.Random();
            int index = rad.Next(0, count-1);
            return index;
        }

        //Método para calcular los personajes no utilizados
        List<Entity> calculateUsed(IEnumerable<Object> players, List<Entity> noUsed)
        {
            foreach (Entity c in players)
            {
                var Behavior = c.FindComponent<WormBehavior>();
                if (Behavior.used == false)
                {
                    noUsed.Add(c);
                }
            }
            return noUsed;
        }

        //Método para calcular los enemigos no utilizados
        List<Entity> calculateEnemyUsed(IEnumerable<Object> enemies, List<Entity> noUsed)
        {
            foreach (Entity c in enemies)
            {
                var Behavior = c.FindComponent<EnemyIABehavior>();
                if (Behavior.used == false)
                {
                    noUsed.Add(c);
                }
            }
            return noUsed;
        }

        //Método para cambiar la cámara de posición para la utilización de cada personaje
        void activeCamera(Entity e)
        {
            cameraHolder.FindComponent<Transform3D>().Position = e.FindComponent<Transform3D>().Position;
            cameraGame = cameraHolder.FindChild("Camera");
            pointCamera = cameraHolder.FindChild("pointCamera");
            cameraHolder.FindComponent<FollowBehavior>().EntityPath = e.Name;
            cameraGame.FindComponent<CameraCollision>().EntityPath = e.Name;
            cameraGame.FindComponent<CameraCollision>().EntityPathCamera = cameraGame.Name;
            cameraGame.FindComponent<CameraCollision>().EntityPathPointCamera = pointCamera.Name;
        }
        
    }
}
