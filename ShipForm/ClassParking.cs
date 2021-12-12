﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipForm
{
    public class Parking<T,W> : IEnumerator<T>, IEnumerable<T>
        where T : class, ITransport
        where W : IDop
    {
        private readonly List<T> _places;
        private readonly int maxCount;
        private readonly int pictureWidth;
        private readonly int pictureHeight;
        private readonly int _placeSizeWidth = 210;
        private readonly int _placeSizeHeight = 160;

        public int currentIndex;

        public T Current => _places[currentIndex];

        object IEnumerator.Current => _places[currentIndex];

        public Parking(int picWidth, int picHeight)
        {
            int width = picWidth / _placeSizeWidth;
            int height = picHeight / _placeSizeHeight;
            maxCount = width * height;
            _places = new List<T>();
            pictureWidth = picWidth;
            pictureHeight = picHeight;
            currentIndex = -1;
        }
        public static bool operator +(Parking<T,W> p, T ship)
        {
            if (p._places.Count >= p.maxCount)
            {
                throw new ParkingOverflowException();
            }
            if (p._places.Contains(ship))
            {
                throw new ParkingAlreadyHaveException();
            }
            p._places.Add(ship);
            return true;
        }
        public static T operator -(Parking<T,W> p, int index)
        {
            if (index < -1 || index >= p._places.Count)
            {
                throw new ParkingNotFoundException(index);
            }
            T temp = p._places[index];
            p._places.RemoveAt(index);
            return temp;
        }
        public static bool operator >(Parking<T, W> p, Parking<T, W> p2)
        {
            if (p._places.Count > p2._places.Count) return true;
            return false;
        }
        public static bool operator <(Parking<T, W> p, Parking<T, W> p2)
        {
            if (p2._places.Count > p._places.Count) return true;
            return false;
        }
        public void Draw(Graphics g)
        {
            int changeHeight = 10;
            int width = pictureWidth / _placeSizeWidth;
            DrawMarking(g);
            for (int i = 0; i < _places.Count; i++)
            {
                _places[i].SetPosition(i % width * _placeSizeWidth + changeHeight,
                    i / width * _placeSizeHeight + changeHeight, pictureWidth,
                    pictureHeight);
                _places[i].DrawTransport(g);
            }
        }
       
        private void DrawMarking(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 3);
            for (int i = 0; i < pictureWidth / _placeSizeWidth; i++)
            {
                for (int j = 0; j < pictureHeight / _placeSizeHeight + 1; ++j)
                {//линия рамзетки места            
                    g.DrawLine(pen, i * _placeSizeWidth, j * _placeSizeHeight + 3, i * _placeSizeWidth + _placeSizeWidth / 2, j * _placeSizeHeight + 3);
                }
                g.DrawLine(pen, i * _placeSizeWidth, 0, i * _placeSizeWidth, (pictureHeight / _placeSizeHeight) * _placeSizeHeight);
            }
        }
        public T this[int ind]
        {
            get
            {
                if(ind>-1 && ind<_places.Count)
                return _places[ind];
                return null;
            }
            set
            {
                if (ind > -1 && ind < _places.Count)
                    _places[ind] = value;
            }
        }
        public T GetNext(int index)
        {
            if (index < 0 || index >= _places.Count)
            {
                return null;
            }
            return _places[index];
        }
        public void Clear()
        {
            _places.Clear();
        }

        public void Sort() => _places.Sort((IComparer<T>)new ShipComparer());
        public void Dispose(){}

        public bool MoveNext()
        {
            currentIndex++;
            if (currentIndex >= _places.Count)
            {
                Reset();
                return false;
            }
            return true;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
