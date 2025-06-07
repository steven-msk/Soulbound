using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public interface IPlaceableItem {
	abstract void Place(Vector2 pos);
}
