namespace EnemyField
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using NamedPipeWrapper;

    using Сommon;

    public partial class Form1 : Form
    {
        /// <summary>
        /// Разметка поля
        /// </summary>
        private readonly GameLayout gameLayout;

        /// <summary>
        /// Игровое поле
        /// </summary>
        private readonly GameField gameField;

        /// <summary>
        /// Id игровка
        /// </summary>
        private readonly int playerId;

        /// <summary>
        /// Именнованый канал
        /// </summary>
        private readonly NamedPipeClient<PipeMessage> pipeClient = new NamedPipeClient<PipeMessage>("SeaBattle");

        /// <summary>
        /// Победа?
        /// </summary>
        private bool winn = false;

        public Form1()
        {
            this.gameLayout = new GameLayout();
            this.gameField = new GameField();
            this.InitializeComponent();
        }

        public Form1(int playerId)
        {
            this.playerId = playerId;
            this.gameLayout = new GameLayout();
            this.gameField = new GameField();
            this.InitializeComponent();
            this.Text += $" - [Игрок {playerId}]";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // получаем рабочую ширину и высоту экрана
            var resolution = Screen.PrimaryScreen.WorkingArea.Size;
            this.Width = resolution.Height / 2;
            this.Height = resolution.Height / 2;

            // чтобы окна были рядом и по левому краю
            this.Location = this.playerId == 1 ? new Point(resolution.Width - 2 * this.Width, 0) :
                                new Point(resolution.Width - 2 * this.Width, resolution.Height - this.Height);

            this.gameLayout.CalculationGameLayout(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.gameField.CalculationGameField(this.ClientRectangle.Width, this.ClientRectangle.Height);
            
            // установка метода обработки сообщений от канала
            this.pipeClient.ServerMessage += this.PipeClientOnServerMessage;
            this.pipeClient.Start(); // запуск канала
        }

        // рисование
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.gameLayout.Draw(e.Graphics);
            this.gameField.DrawMoves(e.Graphics);
            this.gameField.DrawEnemyShips(e.Graphics);
        }

        // изменения размера
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.gameLayout.CalculationGameLayout(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.gameField.CalculationGameField(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.Invalidate();
        }

        // клик мииш
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.winn)
                return;

            var result = this.gameField.CheckingHitWithGettingIndex(e.Location);
            if (!result.Item1)
            {
                MessageBox.Show(
                    @"Эта позиция не является местом расстановки корабля!",
                    $@"Ошибка - [Игрок {this.playerId}]",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // отправка клика
            this.pipeClient.PushMessage(new PipeMessage()
                                            {
                                                PipeMessageType = PipeMessageType.SetMove,
                                                PlayerId = this.playerId,
                                                Index = result.Item2
            });
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
                // обновление
                case PipeMessageType.Update:
                    this.Update(message);
                    this.Invalidate();
                    break;

                // ошибка
                case PipeMessageType.Error:
                    MessageBox.Show(
                        message.Message,
                        $@"Ошибка - [Игрок {this.playerId}]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    break;

                // победа
                case PipeMessageType.Winn:
                    this.winn = true;
                    if (message.PlayerId != 0)
                        MessageBox.Show(
                            @"Победа!",
                            $@"Сообщение - [Игрок {message.PlayerId}]",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    break;
            }
        }

        // метод обновления поля
        private void Update(PipeMessage message)
        {
            if (message.PlayerId == this.playerId)
            {
                this.gameField.SetMove(message.Index, message.MoveType);
                foreach (var ship in message.ShipData)
                {
                    this.gameField.PasteEnemyShip(ship.PastePoint, ship.ShipType, ship.ShipOrientation);
                }
                this.Invalidate();
            }
        }

        // запрет ручного закрытия формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
