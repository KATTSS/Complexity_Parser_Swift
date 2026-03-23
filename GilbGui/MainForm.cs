using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrCSharpLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GilbGui
{
    public class MainForm : Form
    {
        private readonly Color _formBackColor = Color.FromArgb(240, 242, 245);
        private readonly Color _cardBackColor = Color.White;
        private readonly Color _accentColor = Color.FromArgb(52, 58, 64);
        private readonly Color _accentHoverColor = Color.FromArgb(73, 80, 87);
        private readonly Color _textColorDark = Color.FromArgb(33, 37, 41);
        private readonly Color _textColorLight = Color.FromArgb(108, 117, 125);
        private readonly Color _borderColor = Color.FromArgb(233, 236, 239);

        private readonly Font _fontUIBold = new Font("Tahoma", 11, FontStyle.Bold);
        private readonly Font _fontCode = new Font("Lucida Console", 11);
        private readonly Font _fontMetricValue = new Font("Tahoma", 16, FontStyle.Bold);
        private readonly Font _fontMetricTitle = new Font("Tahoma", 12);

        private TextBox txtCode;
        private Button btnCalculate;
        private SplitContainer splitContainer;

        private Label lblAbsoluteComplexity;
        private Label lblRelativeComplexity;
        private Label lblMaxNesting;
        private Label lblTotalOperators;

        public MainForm()
        {
            Text = "Метрология - Метрика Джилба (Swift)";
            Width = 1200;
            Height = 780;
            MinimumSize = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = _formBackColor;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);

            BuildUI();

            Load += (s, e) => ApplySplitRatio();
            Resize += (s, e) => ApplySplitRatio();
        }

        private void ApplySplitRatio()
        {
            int leftWidth = (int)(this.ClientSize.Width * 0.55);
            splitContainer.SplitterDistance = leftWidth;
        }

        private void BuildUI()
        {
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 350,
                SplitterWidth = 8,
                BackColor = _formBackColor,
                Margin = new Padding(15)
            };

            var panelLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            var codeCard = CreateCardPanel();
            codeCard.Dock = DockStyle.Fill;
            var lblCodeHeader = CreateHeaderLabel("Исходный код программы (Swift)");

            txtCode = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = _fontCode,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = _cardBackColor,
                ForeColor = _textColorDark,
                Text = "func checkNestedConditions(y: Int, x: Int) -> String {\n    if y > 50 {\n        if x < 100 {\n            return \"Успех\"\n        }\n    }\n    return \"Провал\"\n}"
            };

            var txtWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BackColor = _cardBackColor };
            txtWrapper.Controls.Add(txtCode);

            codeCard.Controls.Add(lblCodeHeader);
            codeCard.Controls.Add(txtWrapper);
            txtWrapper.BringToFront();

            btnCalculate = new Button
            {
                Text = "РАССЧИТАТЬ МЕТРИКУ ДЖИЛБА",
                Dock = DockStyle.Bottom,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = _accentColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnCalculate.FlatAppearance.BorderSize = 0;
            btnCalculate.MouseEnter += (s, e) => btnCalculate.BackColor = _accentHoverColor;
            btnCalculate.MouseLeave += (s, e) => btnCalculate.BackColor = _accentColor;
            btnCalculate.Click += BtnCalculate_Click;

            var spacerBtn = new Panel { Dock = DockStyle.Bottom, Height = 15, BackColor = _formBackColor };

            panelLeft.Controls.Add(codeCard);
            panelLeft.Controls.Add(spacerBtn);
            panelLeft.Controls.Add(btnCalculate);

            var panelRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5, 15, 15, 15) };
            var metricsCard = CreateCardPanel();
            metricsCard.Dock = DockStyle.Fill;

            var lblMetricsHeader = CreateHeaderLabel("Результаты анализа Джилба");
            metricsCard.Controls.Add(lblMetricsHeader);

            var flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(20, 30, 20, 20),
                BackColor = _cardBackColor
            };

            lblAbsoluteComplexity = CreateMetricDisplay("Абсолютная сложность (CL):", "-", flowLayoutPanel);
            lblRelativeComplexity = CreateMetricDisplay("Относительная сложность (cl):", "-", flowLayoutPanel);
            lblMaxNesting = CreateMetricDisplay("Максимальная вложенность (CLI):", "-", flowLayoutPanel);
            lblTotalOperators = CreateMetricDisplay("Общее число операторов:", "-", flowLayoutPanel);

            metricsCard.Controls.Add(flowLayoutPanel);
            flowLayoutPanel.BringToFront();

            panelRight.Controls.Add(metricsCard);

            splitContainer.Panel1.Controls.Add(panelLeft);
            splitContainer.Panel2.Controls.Add(panelRight);
            Controls.Add(splitContainer);
        }

        private Label CreateMetricDisplay(string title, string initialValue, FlowLayoutPanel parent)
        {
            Panel container = new Panel
            {
                Width = 600,
                Height = 100,
                Margin = new Padding(0, 0, 0, 20)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = _fontMetricTitle,
                ForeColor = _textColorLight,
                Dock = DockStyle.Top,
                AutoSize = true
            };

            Label lblValue = new Label
            {
                Text = initialValue,
                Font = _fontMetricValue,
                ForeColor = _textColorDark,
                Dock = DockStyle.Bottom,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft
            };

            container.Controls.Add(lblValue);
            container.Controls.Add(lblTitle);
            parent.Controls.Add(container);

            return lblValue;
        }

        private Label CreateHeaderLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 45,
                Font = _fontUIBold,
                ForeColor = _textColorDark,
                BackColor = _cardBackColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
        }

        private Panel CreateCardPanel()
        {
            var panel = new Panel { BackColor = _cardBackColor, Padding = new Padding(1) };
            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(_borderColor, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };
            return panel;
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string code = txtCode.Text;

                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Введите код для анализа.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var input = new AntlrInputStream(code);
                var lexer = new Swift5Lexer(input);
                var tokens = new CommonTokenStream(lexer);
                var parser = new Swift5Parser(tokens);

                IParseTree tree = parser.top_level();

                var visitor = new SwiftGilbVisitor();
                visitor.Visit(tree);

                lblAbsoluteComplexity.Text = visitor.CL.ToString();
                lblRelativeComplexity.Text = visitor.Cl.ToString("F4");
                lblMaxNesting.Text = visitor.CLI.ToString();

                lblTotalOperators.Text = visitor.TotalOperators.ToString();

                // Подсветка результатов цветом при успешном расчете
                lblAbsoluteComplexity.ForeColor = _accentColor;
                lblRelativeComplexity.ForeColor = _accentColor;
                lblMaxNesting.ForeColor = _accentColor;
                lblTotalOperators.ForeColor = _accentColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка анализа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}