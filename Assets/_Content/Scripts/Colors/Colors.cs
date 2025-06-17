using System;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{
    public class Colors : Singleton<Colors>
    {
        private ColorLibrary _colorLibrary;

        public static Func<string, Color> Get => (colorId) => Instance.GetColor(colorId);

        private Color GetColor(string id)
        {
            if (_colorLibrary == null)
                _colorLibrary = Resources.Load<ColorLibrary>("ColorLibrary");
            return _colorLibrary.Get(id);
        }
    }
}
