using Godot;
using System;
using AnimaParty.assets.scenes.table;

namespace AnimaParty.assets.script.ui;
public partial class ControlBoton3D : Node3D
{
    private Area3D _botonIncrementar;
    private Area3D _botonDecrementar;
    private Label3D _labelValor;
    private PlayerSelectScene _parentScript;
    private Camera3D _cam;

    public override void _Ready()
    {
        // Referencias a botones y label
        _botonIncrementar = GetNode<Area3D>("BtnR");
        _botonDecrementar = GetNode<Area3D>("BtnL");
        _labelValor = GetNode<Label3D>("Monitor/PlayerCount");

        // Referencia al script del parent
        _parentScript = GetParent<Node3D>() as PlayerSelectScene;

        // Referencia a la cámara principal
        _cam = GetViewport().GetCamera3D();

        ActualizarLabel();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("click")) // Asume que mapeaste el click izquierdo a la acción "click"
        {
            // Raycast desde la cámara
            var ray = new PhysicsRayQueryParameters3D();
            var from = _cam.ProjectRayOrigin(GetViewport().GetMousePosition());
            var to = from + _cam.ProjectRayNormal(GetViewport().GetMousePosition()) * 1000f;
            ray.To = to;
            ray.From = from;
            var space = GetWorld3D().DirectSpaceState;
            var result = space.IntersectRay(ray); // Colisiones en todas las capas

            if (result.Count > 0)
            {
                Node hitNode = result["collider"].As<Node>();

                if (hitNode == _botonIncrementar)
                {
                    _parentScript?.IncreasePlayers();
                }
                else if (hitNode == _botonDecrementar)
                {
                    _parentScript?.DecreasePlayers();
                }

                ActualizarLabel();
            }
        }
    }

    private void ActualizarLabel()
    {
        if (_labelValor != null && _parentScript != null)
        {
            _labelValor.Text = _parentScript.GetPlayerCount().ToString();
        }
    }
}
