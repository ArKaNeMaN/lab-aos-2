﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SupplierAndConsumer
{
    public partial class MainWindow
    {
        private readonly Consumer _con = new();
        private readonly Supplier _sup = new();

        private readonly InfBuffer _buffer;

        public MainWindow()
        {
            InitializeComponent();

            _buffer = new InfBuffer(OnBufferChanged, 6);

            _con.Speed = 500;
            _con.SetBuffer(_buffer);

            _sup.Speed = 500;
            _sup.SetBuffer(_buffer);
        }

        private void OnBufferChanged(InfBuffer buffer, InfBufferActionType type, int? index)
        {
            switch (type)
            {
                case InfBufferActionType.Init:
                    ClearItems();
                    AddItems(buffer.GetTotal());
                    break;

                case InfBufferActionType.Store:
                    AddItems(1);
                    break;

                case InfBufferActionType.Take:
                    RemoveItems(1);
                    break;
            }
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            _buffer.Reset();
            _con.Start();
            _sup.Start();
        }

        private void OnApplySpeed(object sender, RoutedEventArgs e)
        {
            _sup.Speed = int.Parse(Speed1Field.Text);
            _con.Speed = int.Parse(Speed2Field.Text);
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            _con.Stop();
            _sup.Stop();
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            _con.Reset();
            _sup.Reset();
            _buffer.Reset();
        }

        private void UpdateItems(int num)
        {
            var cur = ItemsGrid.Children.Count;
            if (num > cur)
            {
                AddItems(num - cur);
            }
            else if (num < cur)
            {
                RemoveItems(cur - num);
            }
        }

        private readonly Random rnd = new();
        private int _latestRemovedItem = -1;

        private void AddItems(int num = 1)
        {
            for (var i = 0; i < num; i++)
            {
                ItemsGrid.Children.Add(new Rectangle
                {
                    Width = 23,
                    Height = 23,
                    Fill = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(100, 255),
                        (byte)rnd.Next(100, 255),
                        (byte)rnd.Next(100, 255)
                    ))
                    // Stroke = Brushes.Chartreuse,
                });
            }
        }

        private void RemoveItems(int num = 1)
        {
            for (var i = 0; i < num; i++)
            {
                _latestRemovedItem++;
                var rect = ((Rectangle)ItemsGrid.Children[_latestRemovedItem]);
                rect.Stroke = rect.Fill;
                rect.Fill = Brushes.Black;
            }
            // ItemsGrid.Children.RemoveRange(0, num);
            // ItemsGrid.Children.RemoveRange(ItemsGrid.Children.Count - (num - 1), num);
        }

        private void ClearItems()
        {
            ItemsGrid.Children.Clear();
        }
    }
}