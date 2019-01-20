namespace OwnField
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Сommon;

    public partial class ArrangeShipForm : Form
    {
        /// <summary>
        /// Ссылка на главную форму
        /// </summary>
        private readonly MainForm mainForm;

        /// <summary>
        /// Точка клика
        /// </summary>
        private readonly Point pointClick;

        public ArrangeShipForm()
        {
            this.InitializeComponent();
        }

        public ArrangeShipForm(MainForm mainForm, Point pointClick)
        {
            this.InitializeComponent();
            this.mainForm = mainForm;
            this.pointClick = pointClick;

            this.SetShipCount();
            this.SetShipTypeEnabled();
            this.SetCheckShipType();
        }

        /// <summary>
        /// Установка количества возможных вставок кораблей
        /// </summary>
        private void SetShipCount()
        {
            this.label1.Text = (Constants.MaxOne - this.mainForm.GameField.PastedShip.One).ToString();
            this.label2.Text = (Constants.MaxTwo - this.mainForm.GameField.PastedShip.Two).ToString();
            this.label3.Text = (Constants.MaxThree - this.mainForm.GameField.PastedShip.Three).ToString();
            this.label4.Text = (Constants.MaxFour - this.mainForm.GameField.PastedShip.Four).ToString();
        }

        /// <summary>
        /// Установка возможности выбора типа корабля
        /// </summary>
        private void SetShipTypeEnabled()
        {
            if (this.label1.Text == "0")
                this.radioButton3.Enabled = false;
            if (this.label2.Text == "0")
                this.radioButton4.Enabled = false;
            if (this.label3.Text == "0")
                this.radioButton5.Enabled = false;
            if (this.label4.Text == "0")
                this.radioButton6.Enabled = false;
        }

        /// <summary>
        /// Установка выбора корабля для вставки
        /// </summary>
        private void SetCheckShipType()
        {
            if (this.radioButton3.Enabled)
            {
                this.radioButton3.Checked = true;
                return;
            }

            if (this.radioButton4.Enabled)
            {
                this.radioButton4.Checked = true;
                return;
            }

            if (this.radioButton5.Enabled)
            {
                this.radioButton5.Checked = true;
                return;
            }

            if (this.radioButton6.Enabled)
            {
                this.radioButton6.Checked = true;
            }
        }

        /// <summary>
        /// Клик на кнопку
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // получаем тип корабля
            var shipType = this.GetShipType();

            if (shipType == ShipType.None)
            {
                MessageBox.Show(
                    @"Выберите корабль!",
                    $@"Ошибка - [Игрок {this.mainForm.PlayerId}]",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // получем положение корабля
            var shipOrientation = this.GetShipOrientation();

            try
            {
                var can = this.mainForm.GameField.PasteShip(this.pointClick, shipType, shipOrientation);
                if (can) this.mainForm.Invalidate();
                else
                    MessageBox.Show(
                        @"В эту позицию невозможно вставить корабль!",
                        $@"Ошибка - [Игрок {this.mainForm.PlayerId}]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                var isMaxShips = this.mainForm.GameField.PastedShip.One == Constants.MaxOne
                           && this.mainForm.GameField.PastedShip.Two == Constants.MaxTwo
                           && this.mainForm.GameField.PastedShip.Three == Constants.MaxThree
                           && this.mainForm.GameField.PastedShip.Four == Constants.MaxFour;

                // расстановка кораблей закончена
                if (isMaxShips)
                {
                    this.mainForm.FinishArrangeShip = true;
                    MessageBox.Show(
                        @"Расстановка кораблей закончена!",
                        $@"Сообщение - [Игрок {this.mainForm.PlayerId}]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.mainForm.EndArrange();
                }

                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.StackTrace,
                    @"Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Получения типа корабля
        /// </summary>
        /// <returns>Тип корабля</returns>
        private ShipType GetShipType()
        {
            if (this.radioButton3.Checked)
                return ShipType.One;

            if (this.radioButton4.Checked)
                return ShipType.Two;

            if (this.radioButton5.Checked)
                return ShipType.Three;

            if (this.radioButton6.Checked)
                return ShipType.Four;

            return ShipType.None;
        }

        /// <summary>
        /// Получение положения корабля
        /// </summary>
        /// <returns>Положение корабля</returns>
        private ShipOrientation GetShipOrientation()
        {
            return this.radioButton1.Checked ? ShipOrientation.Horizontal :
                   this.radioButton2.Checked ? ShipOrientation.Vertical : ShipOrientation.None;
        }
    }
}
