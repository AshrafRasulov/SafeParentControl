namespace SafeParentControl.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class TaskManagerGraphControl : UserControl
    {
        private Timer updateTimer;
        private List<int> safeTrafficPoints = new List<int>();
        private List<int> threatTrafficPoints = new List<int>();
        private int maxPoints = 50;

        public TaskManagerGraphControl()
        {
            // Включаем двойную буферизацию для плавности отрисовки без мерцания
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.FromArgb(20, 20, 20); // Темный фон в стиле диспетчера задач

            // Таймер обновления кадров графика (анимация движения)
            updateTimer = new Timer();
            updateTimer.Interval = 1000; // Каждую секунду
            updateTimer.Tick += (s, e) => 
            {
                // Имитация или добавление реальных данных трафика
                AddDataPoint(new Random().Next(0, 5), new Random().Next(0, 2));
            };
            updateTimer.Start();
        }

        public void AddDataPoint(int safeCount, int threatCount)
        {
            if (safeTrafficPoints.Count >= maxPoints) safeTrafficPoints.RemoveAt(0);
            if (threatTrafficPoints.Count >= maxPoints) threatTrafficPoints.RemoveAt(0);

            safeTrafficPoints.Add(safeCount);
            threatTrafficPoints.Add(threatCount);

            this.Invalidate(); // Перерисовка графика
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = this.Width;
            int h = this.Height;

            // Рисуем сетку диспетчера задач
            Pen gridPen = new Pen(Color.FromArgb(40, 40, 40), 1);
            int stepX = w / 10;
            int stepY = h / 5;

            for (int x = 0; x < w; x += stepX)
                g.DrawLine(gridPen, x, 0, x, h);
            for (int y = 0; y < h; y += stepY)
                g.DrawLine(gridPen, 0, y, w, y);

            if (safeTrafficPoints.Count < 2) return;

            // Рисуем линии графиков
            Pen safePen = new Pen(Color.LimeGreen, 2);   // Зеленая линия — безопасные / белый список
            Pen threatPen = new Pen(Color.Red, 2);       // Красная линия — угрозы / черный список

            float dx = (float)w / maxPoints;

            for (int i = 0; i < safeTrafficPoints.Count - 1; i++)
            {
                float x1 = i * dx;
                float x2 = (i + 1) * dx;

                // Масштабируем по высоте (максимум шкалы возьмем за 20)
                float y1Safe = h - (safeTrafficPoints[i] * (h / 20f));
                float y2Safe = h - (safeTrafficPoints[i + 1] * (h / 20f));

                float y1Threat = h - (threatTrafficPoints[i] * (h / 20f));
                float y2Threat = h - (threatTrafficPoints[i + 1] * (h / 20f));

                g.DrawLine(safePen, x1, y1Safe, x2, y2Safe);
                g.DrawLine(threatPen, x1, y1Threat, x2, y2Threat);
            }
        }
    }
}