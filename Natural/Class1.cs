using System;
using System.Drawing;
using System.Windows.Forms;

namespace Natural1
{
    public class PixelGridForm : Form
    {
        private NumericUpDown widthInput;
        private NumericUpDown heightInput;
        private Panel[,] gridPanels;
        private Panel colorSelector;
        private Button clearButton;
        private Button generateButton;
        private int gridWidth = 2;
        private int gridHeight = 2;
        private Color selectedColor = Color.Yellow;

        public PixelGridForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Pixel Grid Generator";
            this.Size = new Size(300, 300);

            // Width Label and Input
            Label widthLabel = new Label() { Text = "Width", Location = new Point(10, 10) };
            this.Controls.Add(widthLabel);

            widthInput = new NumericUpDown() { Location = new Point(60, 10), Value = 2, Minimum = 1 };
            this.Controls.Add(widthInput);

            // Height Label and Input
            Label heightLabel = new Label() { Text = "Height", Location = new Point(140, 10) };
            this.Controls.Add(heightLabel);

            heightInput = new NumericUpDown() { Location = new Point(200, 10), Value = 2, Minimum = 1 };
            this.Controls.Add(heightInput);

            // Color Selector
            Label colorLabel = new Label() { Text = "Color", Location = new Point(10, 40) };
            this.Controls.Add(colorLabel);

            colorSelector = new Panel()
            {
                Size = new Size(30, 30),
                Location = new Point(60, 40),
                BackColor = selectedColor
            };
            colorSelector.Click += (s, e) =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedColor = colorDialog.Color;
                        colorSelector.BackColor = selectedColor;
                    }
                }
            };
            this.Controls.Add(colorSelector);

            // Clear Button
            clearButton = new Button() { Text = "Clear", Location = new Point(10, 80) };
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);

            // Generate Button
            generateButton = new Button() { Text = "Generate", Location = new Point(100, 80) };
            generateButton.Click += GenerateButton_Click;
            this.Controls.Add(generateButton);

            // Initial Grid
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            // Clear previous grid
            if (gridPanels != null)
            {
                foreach (var panel in gridPanels)
                {
                    this.Controls.Remove(panel);
                }
            }

            gridWidth = (int)widthInput.Value;
            gridHeight = (int)heightInput.Value;
            gridPanels = new Panel[gridWidth, gridHeight];

            int panelSize = 50;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Panel panel = new Panel()
                    {
                        Size = new Size(panelSize, panelSize),
                        Location = new Point(10 + x * panelSize, 120 + y * panelSize),
                        BackColor = Color.Gray,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    panel.Click += (s, e) =>
                    {
                        panel.BackColor = selectedColor;
                    };

                    gridPanels[x, y] = panel;
                    this.Controls.Add(panel);
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (var panel in gridPanels)
            {
                panel.BackColor = Color.Gray;
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            GenerateGrid();
        }

        [STAThread]
        public static void Main1()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PixelGridForm());
        }
    }
}