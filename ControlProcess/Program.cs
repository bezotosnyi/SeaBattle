namespace ControlProcess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    using NamedPipeWrapper;

    using Сommon;

    public static class Program
    {
        /// <summary>
        /// Путь процессу своего поля
        /// </summary>
        private const string OwnFieldProcessName = @"..\..\..\OwnField\bin\Debug\OwnField.exe";

        /// <summary>
        /// Путь к процессу своего поля
        /// </summary>
        private const string EnemyFieldProcessName = @"..\..\..\EnemyField\bin\Debug\EnemyField.exe";

        /// <summary>
        /// Массив процессов
        /// </summary>
        private static readonly Process[] Process = new Process[]
                                                              {
                                                                  new Process
                                                                      {
                                                                          StartInfo = new ProcessStartInfo(
                                                                              OwnFieldProcessName, $"1")
                                                                      },
                                                                  new Process
                                                                      {
                                                                          StartInfo = new ProcessStartInfo(
                                                                              OwnFieldProcessName, $"2")
                                                                      },
                                                                  new Process
                                                                      {
                                                                          StartInfo = new ProcessStartInfo(
                                                                              EnemyFieldProcessName, $"1")
                                                                      },
                                                                  new Process
                                                                      {
                                                                          StartInfo = new ProcessStartInfo(
                                                                              EnemyFieldProcessName, $"2")
                                                                      }
                                                              };

        /// <summary>
        /// Именнованый канал
        /// </summary>
        private static readonly NamedPipeServer<PipeMessage> PipeServer = new NamedPipeServer<PipeMessage>("SeaBattle");

        /// <summary>
        /// Ходы игрока 1 для передачи по каналу
        /// </summary>
        private static readonly List<MoveData> MoveData1 = new List<MoveData>();

        /// <summary>
        /// Ходы игрока 2 для передачи по каналу
        /// </summary>
        private static List<MoveData> MoveData2 = new List<MoveData>();

        /// <summary>
        /// Корабли игрока 1 для передачи по каналу
        /// </summary>
        private static readonly List<ShipData> ShipData1 = new List<ShipData>();

        /// <summary>
        /// Корабли игрока 2 для передачи по каналу
        /// </summary>
        private static readonly List<ShipData> ShipData2 = new List<ShipData>();

        /// <summary>
        /// Мьютекс для синхронизации
        /// </summary>
        private static readonly Mutex Mtx = new Mutex();

        /// <summary>
        /// Корабли игрока 1
        /// </summary>
        private static List<Ship> shipsPlayer1 = new List<Ship>();

        /// <summary>
        /// Корабли игрока 2
        /// </summary>
        private static List<Ship> shipsPlayer2 = new List<Ship>();
        
        /// <summary>
        /// Игровое поле игрока 1
        /// </summary>
        private static bool[,] gameFieldPlayer1;

        /// <summary>
        /// Игровое поле игрока 2
        /// </summary>
        private static bool[,] gameFieldPlayer2;

        /// <summary>
        /// Игрок 1 закончил расстановку 
        /// </summary>
        private static bool done1 = false;

        /// <summary>
        /// Игрок 2 закончил расстановку 
        /// </summary>
        private static bool done2 = false;

        /// <summary>
        /// Очередь хода
        /// </summary>
        private static int orderMove = 0;

        /// <summary>
        /// Убито корблей игроком 1
        /// </summary>
        private static int kill1 = 0;

        /// <summary>
        /// Убито корблей игроком 2
        /// </summary>
        private static int kill2 = 0;

        /// <summary>
        /// Имеем победителя
        /// </summary>
        private static bool hasWinner = false;

        /// <summary>
        /// Обработчик закрытия консольного приложения
        /// </summary>
        private static SignalHandler signalHandler;

        // Главная функция
        public static void Main(string[] args)
        {
            // подпись на события закрытия консоли
            signalHandler += HandleConsoleSignal;
            ConsoleHelper.SetSignalHandler(signalHandler, true);

            // подпись на события каналов
            PipeServer.ClientConnected += PipeServerOnClientConnected;
            PipeServer.ClientMessage += PipeServerOnClientMessage;
            PipeServer.ClientDisconnected += PipeServerOnClientDisconnected;

            PipeServer.Start(); // запуск канала
            Console.WriteLine("Сервер запущен.");
            Console.WriteLine();
            
            // запуск процессов
            for (int i = 0; i < Process.Length; i++)
            {
                Process[i].Start();
                Console.WriteLine($"Процесс {i + 1} запущен.");
            }

            Console.WriteLine();
            Console.WriteLine("Для закрытия нажмите любою клавишу...");
            Console.WriteLine();

            Console.ReadKey();
            Console.WriteLine();

            // уничтожение процессов и остановка сервера
            for (int i = 0; i < Process.Length; i++)
            {
                Process[i].Kill();
                Console.WriteLine($"Процесс {i + 1} закрыт.");
            }

            Console.WriteLine();

            PipeServer.Stop();
            Console.WriteLine($"Сервер остановлен.");

            Thread.Sleep(1000);
        }

        // событие завершение работы консольного приложения
        private static void HandleConsoleSignal(ConsoleSignal consoleSignal)
        {
            // уничтожение процессов и остановка сервера
            for (int i = 0; i < Process.Length; i++)
            {
                Process[i].Kill();
                Console.WriteLine($"Процесс {i + 1} закрыт.");
            }

            Console.WriteLine();

            PipeServer.Stop();
            Console.WriteLine($"Сервер остановлен.");

            Thread.Sleep(1000);
        }

        // прием сообщений от клиентов
        private static void PipeServerOnClientMessage(NamedPipeConnection<PipeMessage, PipeMessage> connection, PipeMessage message)
        {
            switch (message.PipeMessageType)
            {
                case PipeMessageType.EndArrange:
                    EndArrange(message);
                    break;

                case PipeMessageType.Update:
                    Update(connection, message);
                    break;

                case PipeMessageType.SetMove:
                    SetMove(connection, message);
                    break;                    
            }
        }

        // установка хода
        private static void SetMove(NamedPipeConnection<PipeMessage, PipeMessage> connection, PipeMessage message)
        {
            // если у нас есть победитель
            if (hasWinner)
            {
                // отправка сообщения
                connection.PushMessage(
                    new PipeMessage()
                        {
                            PlayerId = 0,
                            PipeMessageType = PipeMessageType.Winn
                        });
                return;
            }

            // нет очереди хода
            if (orderMove == 0)
            {
                Console.WriteLine("Расстановка кораблей не закончена!");
                connection.PushMessage(
                    new PipeMessage()
                        {
                            PipeMessageType = PipeMessageType.Error, Message = $@"Расстановка кораблей не закончена!"
                        });
                return;
            }

            // первый игрок не готов
            if (!done1)
            {
                Console.WriteLine("Игрок 1 не готов!");
                connection.PushMessage(
                    new PipeMessage() { PipeMessageType = PipeMessageType.Error, Message = @"Игрок 1 не готов!" });
                return;
            }

            // второй игрок не готов
            if (!done2)
            {
                Console.WriteLine("Игрок 2 не готов!");
                connection.PushMessage(
                    new PipeMessage() { PipeMessageType = PipeMessageType.Error, Message = @"Игрок 2 не готов!" });
                return;
            }

            // ход первого игрока
            if (message.PlayerId == 1)
            {
                Console.WriteLine("Ходит игрок 1");

                // если не его очередь
                if (orderMove != 1)
                {
                    Console.WriteLine($@"Очередь ходить игрока {orderMove}");
                    connection.PushMessage(
                        new PipeMessage()
                            {
                                PipeMessageType = PipeMessageType.Error, Message = $@"Очередь ходить игрока {orderMove}"
                            });
                }
                else
                {
                    // если ход уже был
                    if (MoveData1.Contains(message.Index))
                    {
                        Console.WriteLine($"Ход {message.Index.ToString()} уже был");
                        connection.PushMessage(
                            new PipeMessage()
                                {
                                    PipeMessageType = PipeMessageType.Error,
                                    Message = $"Ход {message.Index.ToString()} уже был"
                            });
                        return;
                    }

                    // получение типа хода
                    var moveType = gameFieldPlayer2[message.Index.X, message.Index.Y] ? MoveType.Cross : MoveType.Circle;

                    // добавление в коллекцию ходов
                    MoveData1.Add(new MoveData(new Point(message.Index.X, message.Index.Y), moveType));

                    // проверка убийства корабля
                    CheckKillShip(1, shipsPlayer2, MoveData1);
                    
                    connection.PushMessage(
                        new PipeMessage()
                            {
                                PlayerId = 1,
                                PipeMessageType = PipeMessageType.Update,
                                MoveType = moveType,
                                Index = message.Index,
                                ShipData = ShipData1
                            });

                    Console.WriteLine($"Ход {message.Index.ToString()} {moveType.ToStringName().ToLower()}");

                    orderMove = moveType == MoveType.Cross ? 1 : 2;

                    if (HaveWinner())
                    {
                        Console.WriteLine($"Ход {message.Index.ToString()} приносит игроку 1 победу");
                        connection.PushMessage(
                            new PipeMessage()
                                {
                                    PlayerId = 1,
                                    PipeMessageType = PipeMessageType.Winn
                                });
                        return;
                    }

                    Console.WriteLine($"Очередь ходить игрока {orderMove}");
                }
            }
            else if (message.PlayerId == 2)
            {
                Console.WriteLine("Ходит игрок 2");

                if (orderMove != 2)
                {
                    Console.WriteLine($@"Очередь ходить игрока {orderMove}");
                    connection.PushMessage(
                        new PipeMessage()
                            {
                                PipeMessageType = PipeMessageType.Error, Message = $@"Очередь ходить игрока {orderMove}"
                            });
                }
                else
                {
                    if (MoveData2.Contains(message.Index))
                    {
                        Console.WriteLine($"Ход {message.Index.ToString()} уже был");
                        connection.PushMessage(
                            new PipeMessage()
                                {
                                    PipeMessageType = PipeMessageType.Error,
                                    Message = $@"Ход уже был"
                                });
                        return;
                    }
                    var moveType = gameFieldPlayer1[message.Index.X, message.Index.Y] ? MoveType.Cross : MoveType.Circle;
                    MoveData2.Add(new MoveData(new Point(message.Index.X, message.Index.Y), moveType));
                    CheckKillShip(2, shipsPlayer1, MoveData2);
                    
                    connection.PushMessage(
                        new PipeMessage()
                            {
                                PlayerId = 2,
                                PipeMessageType = PipeMessageType.Update,
                                MoveType = moveType,
                                Index = message.Index,
                                ShipData = ShipData2
                            });

                    Console.WriteLine($"Ход {message.Index.ToString()} {moveType.ToStringName().ToLower()}");

                    orderMove = moveType == MoveType.Cross ? 2 : 1;

                    if (HaveWinner())
                    {
                        Console.WriteLine($"Ход {message.Index.ToString()} приносит игроку 2 победу");
                        connection.PushMessage(
                            new PipeMessage()
                                {
                                    PlayerId = 2,
                                    PipeMessageType = PipeMessageType.Winn
                                });
                        return;
                    }

                    Console.WriteLine($"Очередь ходить игрока {orderMove}");
                }
            }
        }

        // проверка есть ли победитель (убиты все возможные корабли)
        private static bool HaveWinner()
        {
            if (kill1 == Constants.ShipCount() || kill2 == Constants.ShipCount())
                hasWinner = true;
            return hasWinner;
        }

        // проверка убит ли корабль и добавление его в коллекцию для передачи
        private static void CheckKillShip(int playerId, List<Ship> ships, List<MoveData> moveData)
        {
            RecalculateShipData(ships, moveData);

            foreach (var ship in ships)
            {
                if (ship.IsKilled)
                {
                    if (playerId == 1 && !ShipData1.Contains(ship.PastePoint))
                    {
                        ShipData1.Add(new ShipData(ship.PastePoint, ship.ShipType, ship.ShipOrientation));
                        kill1++;
                        Console.WriteLine($"Игрок 1 потопил {ship.ShipType.ToStringName().ToLower()} противника");
                        Console.WriteLine($"Утоплено кораблей {kill1}");
                    }
                    else if (playerId == 2 && !ShipData2.Contains(ship.PastePoint))
                    {
                        ShipData2.Add(new ShipData(ship.PastePoint, ship.ShipType, ship.ShipOrientation));
                        kill2++;
                        Console.WriteLine($"Игрок 2 потопил {ship.ShipType.ToStringName().ToLower()} противника");
                        Console.WriteLine($"Утоплено кораблей {kill2}");
                    }
                }
            }
        }

        // перерасчет точек кораблей
        private static void RecalculateShipData(List<Ship> ships, List<MoveData> moveData)
        {
            var find = false;

            foreach (var move in moveData)         
            {
                if (move.MoveType != MoveType.Cross) continue;

                foreach (var ship in ships.ToList())
                {
                    // if (ship.ShipPoints.All(x => x != move.Point)) break;

                    foreach (var shipPoint in ship.ShipPoints.ToList())
                    {
                        if (shipPoint == move.Point)
                        {
                            ship.ShipPoints.Remove(shipPoint);
                            find = true;
                        }

                        if (find) break;
                    }

                    if (find) break;
                }

                if (find) break;
            }
        }

        // обновление данных
        private static void Update(NamedPipeConnection<PipeMessage, PipeMessage> connection, PipeMessage message)
        {
            if (message.PlayerId == 1 && MoveData2.Count != 0)
            {
                Mtx.WaitOne();
                connection.PushMessage(
                    new PipeMessage() { PipeMessageType = PipeMessageType.Update, PlayerId = 1, MoveData = MoveData2 });
                Mtx.ReleaseMutex();
            }
            else if (message.PlayerId == 2 && MoveData1.Count != 0)
            {
                Mtx.WaitOne();
                connection.PushMessage(
                    new PipeMessage() { PipeMessageType = PipeMessageType.Update,  PlayerId = 2, MoveData = MoveData1 });
                Mtx.ReleaseMutex();
            }
        }

        // окончание игроком расстановки кораблей
        private static void EndArrange(PipeMessage message)
        {
            if (message.PlayerId == 1 && !done1)
            {
                gameFieldPlayer1 = message.GameField;
                shipsPlayer1 = message.ShipData.ToShipList().ToList();
                Console.WriteLine($"Получена расстановка кораблей игрока {message.PlayerId}");
                if (orderMove == 0)
                {
                    orderMove = 1;
                    Console.WriteLine($"Первый ходит игрок {message.PlayerId}");
                }
                done1 = true;
            }
            else if (message.PlayerId == 2 && !done2)
            {
                gameFieldPlayer2 = message.GameField;
                shipsPlayer2 = message.ShipData.ToShipList().ToList();
                Console.WriteLine($"Получена расстановка кораблей игрока {message.PlayerId}");
                if (orderMove == 0)
                {
                    orderMove = 2;
                    Console.WriteLine($"Первый ходит игрок {message.PlayerId}");
                }
                done2 = true;
            }
        }

        /// <summary>
        /// Подключения пользователя к серверу
        /// </summary>
        private static void PipeServerOnClientConnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            /*if (connection.Id > 2)
                return;

            Console.WriteLine($"Игрок {connection.Id} подключен к серверу.");*/
        }

        /// <summary>
        /// Отклчюение пользователя от сервера
        /// </summary>
        private static void PipeServerOnClientDisconnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            /*if (connection.Id > 2)
                return;

            Console.WriteLine($"Игрок {connection.Id} отключен от сервера.");*/
        }
    }
}