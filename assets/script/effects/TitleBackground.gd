extends Control

@onready var container: Node2D=$ShapesContainer
@onready var timer: Timer=$SpawnTimer

func _ready():
	
	timer.wait_time = 0.5
	timer.autostart = true
	timer.timeout.connect(spawn_shape)

func spawn_shape():
	#container.z_index=-10
	var shape = Node2D.new()
	container.add_child(shape)

	var rect = ColorRect.new()
	shape.add_child(rect)

	# Posición inicial (Control -> size)
	var x = randf_range(0, size.x)
	var y = size.y + 100
	shape.position = Vector2(x, y)

	# Tamaño
	var s = randf_range(25, 90)
	rect.size = Vector2(s, s)

	# Velocidad
	shape.set_meta("speed", randf_range(40, 90))

	# Opacidad
	var alpha = randf_range(0.25, 0.45)

	var palette: = [
		Color(0.9, 0.6, 0.9, alpha),   # rosa mágico
		Color(0.6, 0.8, 1.0, alpha),   # azul claro
		Color(0.6, 1.0, 0.8, alpha),   # verde menta
		Color(1.0, 0.9, 0.6, alpha)    # dorado suave
	]

	var type = randi() % palette.size()
	rect.color=palette[type]

func _process(delta):
	for shape in container.get_children():
		var speed = shape.get_meta("speed")
		shape.position.y -= speed * delta
		if shape.position.y < -100:
			shape.queue_free()
