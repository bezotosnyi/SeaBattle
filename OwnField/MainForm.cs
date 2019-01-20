namespace OwnField
{
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using NamedPipeWrapper;

    using Сommon;

    public partial class MainForm : Form
    {
        /// <summary>
        /// Игровое поле
        /// </summary>
        internal readonly GameField GameField;

        /// <summary>
        /// Id игрока
        /// </summary>
        internal readonly int PlayerId;

        /// <summary>
        /// Закончена ли расстановка
        /// </summary>
        internal bool FinishArrangeShip = false;

        /// <summary>
        /// Разметка поля
        /// </summary>
        private readonly GameLayout gameLayout;

        /// <summary>
        /// Поток обновления информации
        /// </summary>
        private readonly Thread updateThread;

        /// <summary>
        /// Именнованый канал
        /// </summary>
        private readonly NamedPipeClient<PipeMessage> pipeClient = new NamedPipeClient<PipeMessage>("SeaBattle");

        public MainForm()
        {
            this.gameLayout = new GameLayout();
            this.GameField = new GameField();
            this.InitializeComponent();
        }

        public MainForm(int playerId)
        {
            this.PlayerId = playerId;
            this.gameLayout = new GameLayout();
            this.GameField = new GameField();
            this.InitializeComponent();
            this.Text += $@" - [Игрок {playerId}]";
            this.updateThread = new Thread(this.UpdateGameField);
        }

        /// <summary>
        /// Обновление игрового поля каждые 10мс
        /// </summary>
        private void UpdateGameField()
        {
            while (true)
            {
                this.pipeClient.PushMessage(new PipeMessage()
                                                {
                                                    PipeMessageType = PipeMessageType.Update,
                                                    PlayerId = this.PlayerId
                                                });
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Закочена расстановка корблей
        /// </summary>
        public void EndArrange()
        {
            // отправка сообщения об расстановке
            this.pipeClient.PushMessage(new PipeMessage
                                            {
                                                PipeMessageType = PipeMessageType.EndArrange,
                                                PlayerId = this.PlayerId,
                                                GameField = this.GameField.GetGameField(),
                                                ShipData = this.GameField.GetShips()
                                            });

            // запуск потока обновления
            this.updateThread.Start();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            // получаем рабочую ширину и высоту экрана
            var resolution = Screen.PrimaryScreen.WorkingArea.Size;
            this.Width = resolution.Height / 2;
            this.Height = resolution.Height / 2;

            // чтобы окна были рядом и по левому краю
            this.Location = this.PlayerId == 1 ? new Point(resolution.Width - this.Width, 0) :
                                new Point(resolution.Width - this.Width, resolution.Height - this.Height);

            // расчет разметки и игрового поля
            this.gameLayout.CalculationGameLayout(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.GameField.CalculationGameField(this.ClientRectangle.Width, this.ClientRectangle.Height);

            // установка метода обработки сообщений от канала
            this.pipeClient.ServerMessage += this.PipeClientOnServerMessage;
            this.pipeClient.Start(); // запуск канала          
        }

        // рисование
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            this.gameLayout.Draw(e.Graphics); // игровое поле
            this.GameField.DrawShips(e.Graphics); // корабли
            this.GameField.DrawMoves(e.Graphics); // ходы
        }

        // изменение размеров формы
        private void MainForm_Resize(object sender, System.EventArgs e)
        {
            this.gameLayout.CalculationGameLayout(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.GameField.CalculationGameField(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.Invalidate();
        }

        // клик миши на форме
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            // если закончена расстановка
            if (this.FinishArrangeShip)
                return;

            // запуск формы для выбора корабля
            var arrangeShipForm = new ArrangeShipForm(this, e.Location);
            arrangeShipForm.ShowDialog();           
        }

        /// <summary>
        /// Обработка сообщений от сервера
        /// </summary>
        private void PipeClientOnServerMessage(
            NamedPipeConnection<PipeMessage, PipeMessage> connection,
            PipeMessage message)
        {
            switch (message.PipeMessageType)
            {
                // обновления
                case PipeMessageType.Update:
                    if (message.PlayerId == this.PlayerId)
                    {
                        foreach (var move in message.MoveData)
                        {
                            this.GameField.SetMove(move.Point, move.MoveType);
                        }
                    }
                    this.Invalidate();
                    break;

                // ошибка
                case PipeMessageType.Error:
                    MessageBox.Show(
                        message.Message,
                        $@"Ошибка - [Игрок {this.PlayerId}]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    break;

                // победа
                case PipeMessageType.Winn:
                    this.updateThread.Abort(); // остановка потока обновления
                    break;
            }
        }

        // запрет закрытия формы вручную
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}