using UnityEngine;

namespace ThunderRoad
{
	public class InventoryLayout : MonoBehaviour
	{
		// this component will let you set the layout of the inventory grid, with spacing, padding, and number of columns and rows
		public int columns = 3;
		public int rows = 3;
		public float itemSize = 0.25f;
		public float spacing = 0.25f;

		public void SetLayout()
		{
			//get the number of children
			int childCount = transform.childCount;

			//organize the children into a grid
			for (var index = 0; index < childCount; index++)
			{
                int reversedIndex = childCount - (index + 1);
				//get the child
				var child = transform.GetChild(index);

				//calculate the column and row of this child
				int column = reversedIndex % columns;
				int row = reversedIndex / columns;

				//calculate the position of this child,  taking into account the itemsize, spacing between items and  that the grid is centered on the parent
                var position = new Vector3(
                    column * (itemSize + spacing) - (columns - 1) * (itemSize + spacing) / 2f,
                    row * (itemSize + spacing) - (rows - 1) * (itemSize + spacing) / 2f,
                    0f);

				//set the position of this child
				child.localPosition = position;
			}
		}

		void OnValidate()
		{
			SetLayout();
		}

		void Start()
		{
			SetLayout();
		}
	}
}