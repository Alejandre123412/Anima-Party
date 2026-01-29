extends Node3D
class_name UniversalDoor3D

const DoorKey= preload("res://assets/scenes/modular/DoorKey.gd")

# ───────── ENUMS ─────────
enum DoorMode { SINGLE, DOUBLE }
enum DoorMotion { HINGE, SLIDE, VANISH, PHYSICS }

# ───────── EXPORTS ─────────
@export_category("Meshes")
@export var mesh_leaf_a: Mesh
@export var mesh_leaf_b: Mesh
@export var mesh_frame: Mesh

@export_category("Door Type")
@export var door_mode := DoorMode.SINGLE
@export var door_motion := DoorMotion.HINGE
@export var start_open := false

@export_category("Lock")
@export var is_locked := false
@export var required_key: DoorKey

@export_category("Movement")
@export var open_angle := 90.0
@export var open_distance := 2.5
@export var open_time := 0.6
@export var open_direction := 1

@export_category("Animation")
@export var ease_type: Tween.EaseType = Tween.EASE_OUT
@export var transition: Tween.TransitionType = Tween.TRANS_SINE

@export_category("Interaction")
@export var auto_close := false
@export var auto_close_delay := 1.5

@export_category("Presets")
@export var preset_name := ""

signal opened
signal closed
signal locked_interaction

# ───────── NODES ─────────
@onready var leaf_a := $Leaf_A
@onready var leaf_b := $Leaf_B
@onready var leaf_a_node = $Leaf_A/MeshInstance3D
@onready var leaf_b_node = $Leaf_B/MeshInstance3D
@onready var frame_node = $Frame/MeshInstance3D
@onready var col_a := $Collider_A
@onready var col_b := $Collider_B
@onready var hinge_physics := $PhysicsHinge

@onready var sfx_open := $Audio_Open
@onready var sfx_close := $Audio_Close
@onready var sfx_locked := $Audio_Locked

# ───────── STATE ─────────
var _open := false
var _tween: Tween

var _a_closed_pos: Vector3
var _b_closed_pos: Vector3
var _a_closed_rot: Vector3
var _b_closed_rot: Vector3

# ───────── READY ─────────
func _ready():
	_cache_closed_state()
	_apply_visibility()
	_apply_preset()
	
	if mesh_leaf_a:
		leaf_a_node.mesh = mesh_leaf_a
	if mesh_leaf_b:
		leaf_b_node.mesh = mesh_leaf_b
	if mesh_frame:
		frame_node.mesh = mesh_frame

	if start_open:
		_apply_state(1.0)
		_open = true

	update_colliders_size()
	_update_colliders_state()

# ───────── PRESETS ─────────
func _apply_preset():
	match preset_name:
		"sci_fi_slide":
			door_motion = DoorMotion.SLIDE
			open_distance = 3.0
			open_time = 0.3

		"wooden_hinge":
			door_motion = DoorMotion.HINGE
			open_angle = 100.0
			open_time = 0.7

# ───────── CACHE ─────────
func _cache_closed_state():
	_a_closed_pos = leaf_a.position
	_b_closed_pos = leaf_b.position
	_a_closed_rot = leaf_a.rotation_degrees
	_b_closed_rot = leaf_b.rotation_degrees

func _apply_visibility():
	leaf_b.visible = door_mode == DoorMode.DOUBLE
	col_b.disabled = door_mode != DoorMode.DOUBLE

# ───────── INTERACTION ─────────
func interact(actor = null):
	if is_locked:
		if not _has_key(actor):
			sfx_locked.play()
			locked_interaction.emit()
			return
		else:
			is_locked = false

	toggle()

func _has_key(actor) -> bool:
	if required_key == null:
		return true

	if actor == null:
		return false

	if not actor.has_method("has_key"):
		return false

	return actor.has_key(required_key.key_id)

# ───────── CONTROL ─────────
func toggle():
	if _open:
		close()
	else:
		open()

func open():
	if _open:
		return

	_open = true
	sfx_open.play()

	match door_motion:
		DoorMotion.PHYSICS:
			hinge_physics.motor_target_velocity = open_direction * 2.0
		_:
			_animate_to(1.0)

	_update_colliders_state()
	opened.emit()

	if auto_close:
		_auto_close()

func close():
	if not _open:
		return

	_open = false
	sfx_close.play()

	match door_motion:
		DoorMotion.PHYSICS:
			hinge_physics.motor_target_velocity = 0.0
		_:
			_animate_to(0.0)

	_update_colliders_state()
	closed.emit()

# ───────── ANIMATION ─────────
func _animate_to(t: float):
	if _tween:
		_tween.kill()

	_tween = create_tween()
	_tween.set_ease(ease_type)
	_tween.set_trans(transition)

	match door_motion:
		DoorMotion.HINGE:
			_tween.tween_property(
				leaf_a,
				"rotation_degrees",
				_a_closed_rot + Vector3(0, open_angle * open_direction * t, 0),
				open_time
			)

		DoorMotion.SLIDE:
			_tween.tween_property(
				leaf_a,
				"position",
				_a_closed_pos + Vector3(open_distance * open_direction * t, 0, 0),
				open_time
			)

		DoorMotion.VANISH:
			leaf_a.visible = t == 0.0

# ───────── APPLY STATE ─────────
func _apply_state(t: float):
	match door_motion:
		DoorMotion.HINGE:
			leaf_a.rotation_degrees = _a_closed_rot + Vector3(0, open_angle * open_direction * t, 0)
		DoorMotion.SLIDE:
			leaf_a.position = _a_closed_pos + Vector3(open_distance * open_direction * t, 0, 0)
		DoorMotion.VANISH:
			leaf_a.visible = t == 0.0

# ───────── COLLISION ─────────
func _update_colliders_state():
	# Desactiva/activa según estado de la puerta
	if col_a:
		col_a.disabled = _open
	if col_b:
		col_b.disabled = _open or door_mode != DoorMode.DOUBLE

func update_colliders_size():
	# Ajusta el collider al mesh
	if leaf_a_node.mesh and col_a:
		var aabb = leaf_a_node.mesh.get_aabb()
		col_a.shape.extents = aabb.size / 2
		col_a.translation = aabb.position + aabb.size/2
	if leaf_b_node.mesh and col_b:
		var aabb = leaf_b_node.mesh.get_aabb()
		col_b.shape.extents = aabb.size / 2
		col_b.translation = aabb.position + aabb.size/2


# ───────── AUTO CLOSE ─────────
func _auto_close():
	await get_tree().create_timer(auto_close_delay).timeout
	if _open:
		close()
