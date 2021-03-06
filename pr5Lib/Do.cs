using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace pr5Lib
{
    public abstract class Do
    {
        public static Stack<Do> Forw = new Stack<Do>();
        public abstract void Undo(ref List<Shape> splist);
        public static Stack<Do> Back = new Stack<Do>();
        public abstract void Redo(ref List<Shape> splist);
        public virtual int GetVal()
        {
            return -1;
        }

        public virtual void SetVal(int val)
        {
            return;
        }
    }

    public class DoSinMove : Do
    {
        private int _divx;
        private int _divy;
        private readonly int _;

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            int x = 0 - _divx;
            splist[_].X += _divx;
            _divx = x;
            int y = 0 - _divy;
            splist[_].Y += _divy;
            _divy = y;
        }

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            int x = 0 - _divx;
            splist[_].X += _divx;
            _divx = x;
            int y = 0 - _divy;
            splist[_].Y += _divy;
            _divy = y;
        }

        public DoSinMove(int dx, int dy, int i, int mult=1)
        {
            _divx = dx;
            _divy = dy;
            _ = i;
            Back.Push(this);
            Forw.Clear();
        }
    }
    
    public class DoMove : Do
    {
        private int _divx;
        private int _divy;

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            int x = 0 - _divx;
            for (int i = 0; i < splist.Count; i++) splist[i].X += _divx;
            _divx = x;
            int y = 0 - _divy;
            for (int i = 0; i < splist.Count; i++) splist[i].Y += _divy;
            _divy = y;
        }

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            int x = 0 - _divx;
            for (int i = 0; i < splist.Count; i++) splist[i].X += _divx;
            _divx = x;
            int y = 0 - _divy;
            for (int i = 0; i < splist.Count; i++) splist[i].Y += _divy;
            _divy = y;
        }

        public DoMove(int dx, int dy)
        {
            _divx = dx;
            _divy = dy;
            Back.Push(this);
            Forw.Clear();
        }
    }
    
    public class Add : Do
    {
        private Shape _sh;
        private readonly int _;

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            splist.Insert(_, _sh);
        }

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            splist.RemoveAt(_);
        }

        public Add(Shape sh, int i)
        {
            _sh = sh;
            _ = i;
            Back.Push(this);
            Forw.Clear();
        }
    }
    
    public class Remove : Do
    {
        private Shape _sh;
        private readonly int _;

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            splist.Insert(_, _sh);
        }

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            splist.RemoveAt(_);
        }

        public Remove(Shape sh, int i)
        {
            _sh = sh;
            _ = i;
            Back.Push(this);
            Forw.Clear();
        }
    }
    
    public class R : Do
    {
        public delegate void Ndo();
        public static event Ndo RNdo;
        
        private int _d;

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            int r = 0 - _d;
            Shape.R -= _d;
            _d = r;
            RNdo.Invoke();
        }

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            int r = 0 - _d;
            Shape.R -= _d;
            _d = r;
            RNdo.Invoke();
        }

        public R(int d)
        {
            _d = d;
            Back.Push(this);
            Forw.Clear();
        }

        public override int GetVal()
        {
            return _d;
        }

        public override void SetVal(int val)
        {
            _d = val;
        }
    }
    
    public class CColor : Do
    {
        private Color _coI;
        private Color _cnI;
        private Color _coL;
        private Color _cnL;

        public override void Redo(ref List<Shape> splist)
        {
            Back.Push(Forw.Pop());
            Shape.InsideColor = _cnI;
            Shape.LineColor = _cnL;
        }

        public override void Undo(ref List<Shape> splist)
        {
            Forw.Push(Back.Pop());
            Shape.InsideColor = _coI;
            Shape.LineColor = _coL;
        }

        public CColor(Color coI, Color cnI, Color coL, Color cnL)
        {
            _coI = coI;
            _cnI = cnI;
            _coL = coL;
            _cnL = cnL;
            Back.Push(this);
            Forw.Clear();
        }
    }

    public class Multi : Do
    {
        private int _m;
        public override void Redo(ref List<Shape> splist)
        {
            var f = Forw.Pop();
            for (int i = 0; i < _m; i++) Forw.Peek().Redo(ref splist);
            Back.Push(f);
        }

        public override void Undo(ref List<Shape> splist)
        {
            var b = Back.Pop();
            for (int i = 0; i < _m; i++) Back.Peek().Undo(ref splist);
            Forw.Push(b);
        }

        public Multi(int m)
        {
            _m = m;
            Back.Push(this);
            Forw.Clear();
        }
    }
}
