using Core;

public interface BaseNode : IStackable, IClickable, Interactable, IMousePress
{
	public NodePlaneHandler nodePlaneManager { get; set; }

	public void init(NodePlaneHandler nodePlane) { }

	public CardStack processCardStack { get; set; }

	public int id { get; set; }

	public bool isActive { get; set; }
}
